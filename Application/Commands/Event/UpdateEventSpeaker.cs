using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class UpdateEventSpeaker
    {
        public record UpdateEventSpeakerCommand(
            Guid UserId,
            Guid EventId,
            Guid SpeakerId,
            string Name,
            string? Title,
            string? Company,
            string? LinkedInUrl
        ) : IRequest<Result<string>>;

        public class UpdateEventSpeakerHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateEventSpeakerCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateEventSpeakerCommand request,
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

                var speaker = @event.Speakers.FirstOrDefault(s => s.Id == request.SpeakerId);

                if (speaker is null)
                    return Result<string>.Failure("Speaker not found");

                speaker.Name = request.Name;
                speaker.Title = request.Title;
                speaker.Company = request.Company;
                speaker.LinkedInUrl = request.LinkedInUrl;

                @event.DateModified = DateTime.UtcNow;


                await unitOfWork.SaveAsync();

                return Result<string>.Success("updated", "Speaker updated successfully");
            }
        }
    }
}