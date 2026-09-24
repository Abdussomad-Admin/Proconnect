using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Event
{
    public class UploadEventCoverImage
    {
        public record UploadEventCoverImageCommand(
            Guid UserId,
            Guid EventId,
            IFormFile File
        ) : IRequest<Result<string>>;

        public class UploadEventCoverImageHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UploadEventCoverImageCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UploadEventCoverImageCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<string>.Failure("You are not authorized to edit this event");

                if (!string.IsNullOrEmpty(@event.CoverImagePublicId))
                {
                    await fileStorage.DeleteAsync(@event.CoverImagePublicId, cancellationToken);
                }

                var uploadResult = await fileStorage.UploadAsync(request.File, "event-covers", cancellationToken);

                @event.CoverImageUrl = uploadResult.Url;
                @event.CoverImagePublicId = uploadResult.PublicId;
                @event.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(uploadResult.Url, "Cover image uploaded successfully");
            }
        }
    }
}