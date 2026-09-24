using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Event
{
    public class UpdateEvent
    {
        public record UpdateEventCommand(
            Guid UserId,
            Guid EventId,
            string Title,
            string Description,
            EventType EventType,
            string? Category,
            DateTime StartDateTime,
            DateTime EndDateTime,
            EventLocationType LocationType,
            string? Location,
            string? OnlineMeetingLink,
            EventVisibility Visibility,
            EventRegistrationType RegistrationType,
            int? AttendeeLimit
        ) : IRequest<Result<string>>;

        public class UpdateEventHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateEventCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateEventCommand request,
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

                if (@event.Status == EventStatus.Cancelled || @event.Status == EventStatus.Completed)
                    return Result<string>.Failure("This event can no longer be edited");

                if (request.EndDateTime <= request.StartDateTime)
                    return Result<string>.Failure("End date/time must be after start date/time");

                @event.Title = request.Title;
                @event.Description = request.Description;
                @event.EventType = request.EventType;
                @event.Category = request.Category;
                @event.StartDateTime = request.StartDateTime;
                @event.EndDateTime = request.EndDateTime;
                @event.LocationType = request.LocationType;
                @event.Location = request.Location;
                @event.OnlineMeetingLink = request.OnlineMeetingLink;
                @event.Visibility = request.Visibility;
                @event.RegistrationType = request.RegistrationType;
                @event.AttendeeLimit = request.AttendeeLimit;
                @event.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success("updated", "Event updated successfully");
            }
        }
    }
}