using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Constant;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
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
            IAuditLogRepository auditLogRepository,
            ILogger<RegisterHandler> logger)
            : IRequestHandler<RegisterCommand, Result<Guid>>
        {
            private readonly AppSettings settings = appSettings.Value;

            private const string EmailFailedMessage =
                "Account created but verification email could not be sent. Please request a new code.";

            public async Task<Result<Guid>> Handle(
                RegisterCommand request,
                CancellationToken cancellationToken)
            {
                if (request.Password != request.ConfirmPassword)
                {
                    return Result<Guid>.Failure("Passwords do not match");
                }

                var email = request.Email.Trim();

                var existingUser = await userRepository.GetByEmailAsync(email);

                if (existingUser is not null)
                {
                    if (existingUser.IsVerified)
                    {
                        return Result<Guid>.Failure("Email already exists");
                    }
                    var newToken = GenerateToken();

                    existingUser.FirstName = request.FirstName;
                    existingUser.LastName = request.LastName;
                    existingUser.UserName = request.UserName;
                    existingUser.Tel = request.Tel;
                    existingUser.HashedPassword =
                        passwordHasher.HashPassword(existingUser, request.Password);
                    existingUser.VerificationToken = newToken;
                    existingUser.VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5);
                    await unitOfWork.SaveAsync();

                    return await SendVerificationAsync(existingUser, newToken);
                }

                // ---- New user ----
                var token = GenerateToken();

                var user = new User
                {
                    FirstName = request.FirstName,
                    LastName = request.LastName,
                    UserName = request.UserName,
                    Email = email,
                    Tel = request.Tel,
                    Role = Roles.User,
                    VerificationToken = token,
                    VerificationTokenExpiry = DateTime.UtcNow.AddMinutes(5),
                    IsVerified = false,
                    Status = UserStatus.Active,
                    DateCreated = DateTime.UtcNow,
                    CreatedBy = email,
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
                        Description = $"New user registered: {user.UserName}"
                    });

                await unitOfWork.SaveAsync();

                return await SendVerificationAsync(user, token);
            }

            private static string GenerateToken() =>
                RandomNumberGenerator.GetInt32(100000, 1000000).ToString();

            private async Task<Result<Guid>> SendVerificationAsync(User user, string token)
            {
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
                    logger.LogError(
                        "Verification email failed for user {UserId}: {Reason}",
                        user.Id,
                        emailResult.Message);

                    return Result<Guid>.Failure(EmailFailedMessage);
                }

                return Result<Guid>.Success(user.Id, "registered");
            }
        }
    }
}