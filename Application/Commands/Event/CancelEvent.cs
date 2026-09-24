using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class CancelEvent
    {
        public record CancelEventCommand(
            Guid UserId,
            Guid EventId,
            string CancellationReason
        ) : IRequest<Result<string>>;

        public class CancelEventHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IEventRegistrationRepository eventRegistrationRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork)
            : IRequestHandler<CancelEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                CancelEventCommand request,
                CancellationToken cancellationToken)
            {
                var recruiterProfile = await recruiterProfileRepository.GetByUserIdAsync(request.UserId);

                if (recruiterProfile is null || recruiterProfile.CompanyId is null)
                    return Result<string>.Failure("Recruiter profile not found");

                var @event = await eventRepository.GetByIdAsync(request.EventId);

                if (@event is null)
                    return Result<string>.Failure("Event not found");

                if (@event.CompanyId != recruiterProfile.CompanyId)
                    return Result<string>.Failure("You are not authorized to cancel this event");

                if (@event.Status != EventStatus.Published)
                    return Result<string>.Failure("Only published events can be cancelled");

                @event.Status = EventStatus.Cancelled;
                @event.CancelledAt = DateTime.UtcNow;
                @event.CancellationReason = request.CancellationReason;
                @event.DateModified = DateTime.UtcNow;


                await unitOfWork.SaveAsync();

                var registrationsPage = await eventRegistrationRepository.GetByEventIdAsync(
                    new PageRequest(),
                    usePaging: false,
                    eventId: @event.Id,
                    status: EventRegistrationStatus.Registered);

                foreach (var registration in registrationsPage.Items)
                {
                    await notificationService.SendNotificationAsync(
                        recipientUserId: registration.ProfessionalProfile.UserId,
                        actorUserId: recruiterProfile.UserId,
                        actorName: @event.Title,
                        actorAvatarUrl: null,
                        title: "Event cancelled",
                        message: $"\"{@event.Title}\" has been cancelled. Reason: {request.CancellationReason}",
                        type: NotificationType.EventCancelled,
                        sourceEntityType: NotificationSourceEntityType.Event,
                        sourceEntityId: @event.Id,
                        actionUrl: $"/event-details.html?id={@event.Id}",
                        createdBy: request.UserId.ToString());
                }

                return Result<string>.Success("cancelled", "Event cancelled and attendees notified");
            }
        }
    }
}