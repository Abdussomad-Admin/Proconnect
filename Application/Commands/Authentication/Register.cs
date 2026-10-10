using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Constant;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Options;
using System.Security.Cryptography;

namespace Application.Commands.Authentication
{
    public class Register
    {
        public record RegisterCommand(
            string FirstName,
            string LastName,
            string UserName,
            string Email,
            string Tel,
            string Password,
            string ConfirmPassword
        ) : IRequest<Result<Guid>>;

        public class RegisterHandler(
            IUserRepository userRepository,
            IPasswordHasher<User> passwordHasher,
            IUnitOfWork unitOfWork,
            IEmailService emailService,
            IOptions<AppSettings> appSettings,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<RegisterCommand, Result<Guid>>
        {
            private readonly AppSettings settings = appSettings.Value;
            public async Task<Result<Guid>> Handle(
                RegisterCommand request,
                CancellationToken cancellationToken)
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return Result<Guid>.Failure(
                        "Passwords do not match");
                }


                var emailExists =
                    await userRepository.ExistsByEmailAsync(request.Email);

                var getuser = await userRepository.GetByEmailAsync(request.Email);


                if (emailExists)
                {
                    if (getuser is not null)
                    {
                        if (!getuser.IsVerified)
                        {
                            return Result<Guid>.Failure(
                                "User Created but due to an error not verified");
                        }
                    }
                    
                    return Result<Guid>.Failure(
                        "Email already exists");
                }


                var token =
                RandomNumberGenerator
                .GetInt32(100000, 999999)
                .ToString();

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = request.Email,
                    Tel = request.Tel,
                    Role = Roles.User,
                    VerificationToken = token,
                    VerificationTokenExpiry =
                    DateTime.UtcNow.AddMinutes(5),
                    IsVerified = false,
                    Status = UserStatus.Active,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = request.Email,
                    Bio = "I'm still an ordinary user with no role for now"
                };

                user.HashedPassword =
                    passwordHasher.HashPassword(user, request.Password);
               
                await userRepository.CreateAsync(user);

                await auditLogRepository.AddAsync(
                    new AuditLog
                    {
                        UserId = user.Id,

                        Action = "Register",

                        CreatedBy = user.Id.ToString(),

                        Description =
                            $"New user registered: {user.UserName}"
                    });



                await unitOfWork.SaveAsync();


                var emailResult = await emailService.SendEmailAsync(
                new EmailRequest
                {
                    To = user.Email,
                    Subject = "Verify Your ProConnect Account",
                    Body = EmailTemplates.VerificationEmail(
                        user.UserName,
                        user.Email,
                        token,
                        settings.FrontendUrl)
                });


                if (!emailResult.Status)
                {
                    return Result<Guid>.Failure(
                        $"Account created but verification email could not be sent. Please request a new code.{emailResult.Message}");
                }

                return Result<Guid>.Success(
                    user.Id,
                    "registered");
            }
        }
    }
}