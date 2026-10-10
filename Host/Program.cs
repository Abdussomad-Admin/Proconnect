using Application.Commands.Authentication;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Infrastructure.Authentication;
using Infrastructure.BackgroundServices;
using Infrastructure.HealthChecks;
using Infrastructure.Hubs;
using Infrastructure.Persistence.Context;
using Infrastructure.Persistence.Repositories;
using Infrastructure.Services;
using Infrastructure.Storage.Cloudinary;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.IdentityModel.Tokens.Jwt;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

// ==========================================
// Framework Services
// ==========================================

builder.Services.AddEndpointsApiExplorer();


builder.Services.AddHttpContextAccessor();

builder.Services.AddSignalR();

builder.Services.AddAuthorization();


// ==========================================
// MediatR
// ==========================================

builder.Services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(typeof(Login).Assembly);
});

builder.Services.AddControllers()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
    });


builder.Services.AddDbContext<ProConnectDbContext>(config =>
    config.UseMySQL(builder.Configuration.GetConnectionString("DefaultConnection")!)
          .LogTo(Console.WriteLine, LogLevel.Information)
          .EnableSensitiveDataLogging());


// ==========================================
// AutoMapper (if using AutoMapper)
// ==========================================

// builder.Services.AddAutoMapper(typeof(MappingProfile).Assembly);


// ==========================================
// Authentication
// ==========================================

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();

builder.Services.AddScoped<ITokenServices, TokenService>();

//builder.Services.AddScoped<ICurrentUser, CurrentUser>();


// ==========================================
// Email
// ==========================================

builder.Services.AddScoped<IEmailService, EmailService>();


// ==========================================
// Unit Of Work
// ==========================================

builder.Services.AddScoped<IUnitOfWork, UnitOfWork>();


// ==========================================
// Repositories
// ==========================================

// User
builder.Services.AddScoped<IUserRepository, UserRepository>();

builder.Services.AddScoped<ISkillRepository, SkillRepository>();


// Professional Profile

builder.Services.AddScoped<IProfessionalProfileRepository, ProfessionalProfileRepository>();


builder.Services.AddScoped<IPortfolioLinkRepository, PortfolioLinkRepository>();

// Recruiter Profile
builder.Services.AddScoped<IRecruiterProfileRepository, RecruiterProfileRepository>();

// Company
builder.Services.AddScoped<ICompanyRepository, CompanyRepository>();

// Experience
builder.Services.AddScoped<IExperienceRepository, ExperienceRepository>();

// Education
builder.Services.AddScoped<IEducationRepository, EducationRepository>();

builder.Services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();

builder.Services.AddScoped<IUserFollowRepository, UserFollowRepository>();

builder.Services.AddScoped<IPostLikeRepository, PostLikeRepository>();

builder.Services.AddScoped<IPostRepository, PostRepository>();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();

builder.Services.AddScoped<IFileUploadRepository, FileUploadRepository>();

builder.Services.AddScoped<IConversationRepository, ConversationRepository>();

builder.Services.AddScoped<IReportRepository, ReportRepository>();

builder.Services.AddScoped<IJobRepository, JobRepository>();

builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

builder.Services.AddScoped<ISavedJobRepository, SavedJobRepository>();


builder.Services.AddScoped<IConversationParticipantRepository, ConversationParticipantRepository>();

builder.Services.AddScoped<IJobSkillRepository, JobSkillRepository>();

builder.Services.AddScoped<IMessageRepository, MessageRepository>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<IDepartmentRepository, DepartmentRepository>();

builder.Services.AddScoped<IAdminDashboardRepository, AdminDashboardRepository>();



builder.Services.AddScoped<ICompanyReviewRepository, CompanyReviewRepository>();

builder.Services.AddScoped<ISavedJobSearchRepository, SavedJobSearchRepository>();

builder.Services.AddScoped<ISavedCandidateRepository, SavedCandidateRepository>();

builder.Services.AddHostedService<JobAlertMatchingService>();


// Skill
//builder.Services.AddScoped<ISkillRepository, SkillRepository>();

// Professional Skill
builder.Services.AddScoped<IProfessionalSkillRepository, ProfessionalSkillRepository>();

// Project
builder.Services.AddScoped<IProjectRepository, ProjectRepository>();

// Certificate
builder.Services.AddScoped<ICertificateRepository, CertificateRepository>();

builder.Services.AddScoped<ILocationMatcher, LocationMatcher>();
builder.Services.AddScoped<IMatchScoreCalculator, MatchScoreCalculator>();

//// Job
//builder.Services.AddScoped<IJobRepository, JobRepository>();

//// Job Application
//builder.Services.AddScoped<IJobApplicationRepository, JobApplicationRepository>();

//// Notification
//builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

//// Message
//builder.Services.AddScoped<IMessageRepository, MessageRepository>();

//// User Connection
//builder.Services.AddScoped<IUserConnectionRepository, UserConnectionRepository>();

//Audit Log
builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();

builder.Services.AddScoped<INotificationRepository, NotificationRepository>();

builder.Services.AddScoped<INotificationService, NotificationService>();

builder.Services.AddScoped<IAuditLogRepository, AuditLogRepository>();


builder.Services.AddScoped<IEventRepository, EventRepository>();

builder.Services.AddScoped<IEventRegistrationRepository, EventRegistrationRepository>();

builder.Services.AddScoped<ISavedEventRepository, SavedEventRepository>();


builder.Services.AddScoped<IAnalyticsEventRepository, AnalyticsEventRepository>();


builder.Services.AddHostedService<ScheduledJobPublisher>();

builder.Services.Configure<JwtSetiings>(builder.Configuration.GetSection("Jwt"));

JwtSecurityTokenHandler.DefaultInboundClaimTypeMap.Clear();

var jwtSettings = builder.Configuration.GetSection("Jwt").Get<JwtSetiings>();


builder.Services.AddHealthChecks()
    .AddDbContextCheck<ProConnectDbContext>("database")
    .AddCheck<CloudinaryHealthCheck>("storage")
    .AddCheck<SmtpHealthCheck>("email")
    .AddCheck<SignalRHealthCheck<ChatHub>>("/chatHub")
    .AddCheck<SignalRHealthCheck<NotificationHub>>("/notificationsHub");



builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme).AddJwtBearer(opt =>
{
    opt.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = jwtSettings!.Issuer,
        ValidAudience = jwtSettings.Audience,
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings.SecretKey))
    };

    opt.Events = new JwtBearerEvents
    {
        OnMessageReceived = context =>
        {
            var accessToken = context.Request.Query["access_token"];
            var path = context.HttpContext.Request.Path;

            if (!string.IsNullOrEmpty(accessToken) &&
                (path.StartsWithSegments("/chatHub") || path.StartsWithSegments("/notificationHub")))
            {
                context.Token = accessToken;
            }

            return Task.CompletedTask;
        }
    };
});

builder.Services.AddScoped<IPasswordHasher<User>, PasswordHasher<User>>();


builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo
    {
        Title = "Proconnect Api",
        Version = "v1",
        Description = "Api for professional networking"
    });

    options.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        In = ParameterLocation.Header,
        Description = "JWT Authorization header using the bearer scheme. Enter your token in the text input below."
    });

    options.AddSecurityRequirement(document =>
    new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});




builder.Services.Configure<EmailSettings>(
    builder.Configuration.GetSection("EmailSettings"));

builder.Services.Configure<FileSettings>(
    builder.Configuration.GetSection("FileSettings"));

builder.Services.Configure<AppSettings>(
    builder.Configuration.GetSection("AppSettings"));

builder.Services.Configure<CloudinarySettings>(
    builder.Configuration.GetSection("Cloudinary"));

builder.Services.AddScoped<IFileStorage, CloudinaryFileStorage>();



// ==========================================
// CORS
// ==========================================

builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend", policy =>
    {
        policy
            .WithOrigins(
                "http://localhost:5500",
                "https://abdussomad-admin.github.io")
            .AllowAnyHeader()
            .AllowAnyMethod()
            .AllowCredentials();
    });
});





var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}


app.UseCors("AllowFrontend");
app.UseHttpsRedirection();
app.UseAuthentication();
app.UseAuthorization();

app.MapHub<NotificationHub>("/notificationHub");
app.MapHub<ChatHub>("/chatHub");
app.MapControllers();

app.Run();
