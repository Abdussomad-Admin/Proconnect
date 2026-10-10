using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.Extensions.Options;

namespace Application.Commands.Authentication
{
    public class VerifyEmail
    {
        public record VerifyEmailCommand(
            string Email,
            string Token)
            : IRequest<Result<string>>;



        public class VerifyEmailHandler(
            IUserRepository userRepository,
            IEmailService emailService,
            IUnitOfWork unitOfWork, IOptions<AppSettings> appSettings,
            IAuditLogRepository auditLogRepository)
            : IRequestHandler<VerifyEmailCommand, Result<string>>
        {
            private readonly AppSettings settings = appSettings.Value;
            public async Task<Result<string>> Handle(
                VerifyEmailCommand request,
                CancellationToken cancellationToken)
            {
                var user = await userRepository.GetByEmailAsync(request.Email);

                if (user is null)

                    return Result<string>.Failure("User not found");

                if (user.IsVerified)

                    return Result<string>.Failure("Email already verified");

                if (!string.Equals(user.VerificationToken,
                        request.Token,
                        StringComparison.Ordinal))

                    return Result<string>.Failure("Invalid verification code");

                if (user.VerificationTokenExpiry < DateTime.UtcNow)

                    return Result<string>.Failure(
                        "Verification code has expired. Please request a new one");

                user.IsVerified = true;

                user.Status = UserStatus.Active;

                user.VerificationToken = null;

                user.VerificationTokenExpiry = null;

                user.DateModified = DateTime.UtcNow;


                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = user.Id,

                    Action = "Verify Email",

                    Description = $"Verify Email : {user.FirstName}({user.UserName})",

                    CreatedBy = user.Id.ToString()
                });

                await unitOfWork.SaveAsync();

                var emailResult = await emailService.SendEmailAsync( 
                    new EmailRequest
                    {
                        To = user.Email,
                        Subject = "Welcome to Proconnect 🎉",
                        Body = EmailTemplates.WelcomeEmail(user.UserName ?? "User", user.Email, settings.FrontendUrl)
                    }); 


                return Result<string>.Success(
                    "Email verified successfully! You can now login.", "verified");
            }
        }
    }
}