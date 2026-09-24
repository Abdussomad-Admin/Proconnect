using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class RemoveEventSpeaker
    {
        public record RemoveEventSpeakerCommand(Guid UserId, Guid EventId, Guid SpeakerId) : IRequest<Result<string>>;

        public class RemoveEventSpeakerHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RemoveEventSpeakerCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                RemoveEventSpeakerCommand request,
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

                @event.Speakers.Remove(speaker);
                @event.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success("removed", "Speaker removed successfully");
            }
        }
    }
}