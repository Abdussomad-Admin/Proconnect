using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Contract.Settings;
using MediatR;
using Microsoft.AspNetCore.Http;

namespace Application.Commands.Event
{
    public class AddEventSpeaker
    {
        public record AddEventSpeakerCommand(
            Guid UserId,
            Guid EventId,
            string Name,
            string? Title,
            string? Company,
            string? LinkedInUrl,
            IFormFile? Photo
        ) : IRequest<Result<string>>;

        public class AddEventSpeakerHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IFileStorage fileStorage,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddEventSpeakerCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddEventSpeakerCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetWithDetailsAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<string>.Failure("You are not authorized to edit this event");

                string? photoUrl = null;

                if (request.Photo is not null)
                {
                    var uploadResult = await fileStorage.UploadAsync(request.Photo, "event-speakers", cancellationToken);
                    photoUrl = uploadResult.Url;
                }

                var speaker = new Domain.Entities.EventSpeaker
                {
                    EventId = @event.Id,
                    Name = request.Name,
                    Title = request.Title,
                    Company = request.Company,
                    PhotoUrl = photoUrl,
                    LinkedInUrl = request.LinkedInUrl,
                    DisplayOrder = @event.Speakers.Count
                };

                await eventRepository.AddSpeakerAsync(speaker);

                @event.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(speaker.Id.ToString(), "Speaker added successfully");
            }
        }
    }
}