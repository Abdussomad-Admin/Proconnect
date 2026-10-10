using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;
using MediatR;
using Microsoft.AspNetCore.Identity;

namespace Application.Commands.Authentication
{
    public class Login
    {
        public record LoginCommand(string Login, string Password) : IRequest<Result<LoginResponse>>;

        public class LoginHandler(IPasswordHasher<User> passwordHasher, 
            IUserRepository userRepository,
            IProfessionalProfileRepository professionalProfile,
            IRecruiterProfileRepository recruiterProfile,
            ITokenServices tokenServices) : IRequestHandler<LoginCommand, Result<LoginResponse>>
        {
            public async Task<Result<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
            {
                var getUser = await userRepository.GetByEmailAsync(request.Login);

                getUser ??= await userRepository.GetByUserNameAsync(request.Login);


                if (getUser is null)
                {
                    return Result<LoginResponse>.Failure("Unauthorized");
                }

                if (!getUser.IsVerified)
                {
                    return Result<LoginResponse>.Failure("Please verify your email");
                }

                if (getUser.Status == UserStatus.Deactivated)
                {
                    return Result<LoginResponse>.Failure("Account has been deactivated.");
                }

                var profileId = getUser.Id.ToString();

                if (getUser.Role.ToLower() != "admin")
                {
                    var professionProfile = await professionalProfile.GetByUserIdAsync(Guid.Parse(profileId));

                    if (professionProfile is not null)
                    {
                        profileId = professionProfile.Id.ToString();
                    }
                    else
                    {
                        var recruiterProfileData = await recruiterProfile.GetByUserIdAsync(Guid.Parse(profileId));

                        if (recruiterProfileData is null)
                        {
                            return Result<LoginResponse>.Failure("Profile not found.");
                        }

                        profileId = recruiterProfileData.Id.ToString();
                    }
                }


                var verificationResult = passwordHasher.VerifyHashedPassword(getUser, getUser.HashedPassword, request.Password);

                if (verificationResult == PasswordVerificationResult.Failed)
                {
                    return Result<LoginResponse>.Failure("Unauthorized");
                }

               

                var response = new LoginResponse
                {
                    Id = getUser.Id,
                    Email = getUser.Email,
                    Role = getUser.Role,
                    ProfileId = profileId,
                    UserName = getUser.UserName
                };

                var token = tokenServices.GenerateToken(response);

                return Result<LoginResponse>.Success(response, token);
            }
        }
    }
}
