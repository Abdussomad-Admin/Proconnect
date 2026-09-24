using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class UpdateEventAgendaItem
    {
        public record UpdateEventAgendaItemCommand(
            Guid UserId,
            Guid EventId,
            Guid AgendaItemId,
            string Title,
            string? Description,
            DateTime StartTime,
            DateTime EndTime
        ) : IRequest<Result<string>>;

        public class UpdateEventAgendaItemHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateEventAgendaItemCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                UpdateEventAgendaItemCommand request,
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

                var item = @event.AgendaItems.FirstOrDefault(a => a.Id == request.AgendaItemId);

                if (item is null)
                    return Result<string>.Failure("Agenda item not found");

                if (request.EndTime <= request.StartTime)
                    return Result<string>.Failure("Agenda item end time must be after start time");

                item.Title = request.Title;
                item.Description = request.Description;
                item.StartTime = request.StartTime;
                item.EndTime = request.EndTime;

                @event.DateModified = DateTime.UtcNow;


                await unitOfWork.SaveAsync();

                return Result<string>.Success("updated", "Agenda item updated successfully");
            }
        }
    }
}