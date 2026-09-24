using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class PublishEvent
    {
        public record PublishEventCommand(Guid UserId, Guid EventId) : IRequest<Result<string>>;

        public class PublishEventHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<PublishEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                PublishEventCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<string>.Failure("You are not authorized to publish this event");

                if (@event.Status != EventStatus.Draft)
                    return Result<string>.Failure("Only draft events can be published");

                @event.Status = EventStatus.Published;
                @event.PublishedAt = DateTime.UtcNow;
                @event.DateModified = DateTime.UtcNow;


                await unitOfWork.SaveAsync();

                return Result<string>.Success("published", "Event published successfully");
            }
        }
    }
}