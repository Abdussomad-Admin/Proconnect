using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Professional
{
    public class AddCertificate
    {
        public record AddCertificateCommand(
            Guid ProfessionalProfileId,
            string Name,
            string IssuingOrganization,
            DateTime IssueDate,
            DateTime? ExpireDate,
            string CredentialId,
            string CredentialUrl
        ) : IRequest<Result<string>>;



        public class AddCertificateHandler(
            ICertificateRepository certificateRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddCertificateCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddCertificateCommand request,
                CancellationToken cancellationToken)
            {
                var certificate = new Certificate
                {
                    ProfessionalProfileId = request.ProfessionalProfileId,

                    Name = request.Name,

                    IssuingOrganization = request.IssuingOrganization,

                    IssueDate = request.IssueDate,

                    ExpireDate = request.ExpireDate,

                    CredentialId = request.CredentialId,

                    CreatedBy = request.ProfessionalProfileId.ToString(),

                    CredentialUrl = request.CredentialUrl,

                    DateCreated = DateTime.UtcNow
                };


                await certificateRepository.CreateAsync(certificate);


                await unitOfWork.SaveAsync();


                return Result<string>.Success(
                    "Certificate added successfully",
                    "created");
            }
        }
    }
}