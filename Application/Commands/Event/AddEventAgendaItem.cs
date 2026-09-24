using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class AddEventAgendaItem
    {
        public record AddEventAgendaItemCommand(
            Guid UserId,
            Guid EventId,
            string Title,
            string? Description,
            DateTime StartTime,
            DateTime EndTime
        ) : IRequest<Result<string>>;

        public class AddEventAgendaItemHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<AddEventAgendaItemCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                AddEventAgendaItemCommand request,
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

                if (request.EndTime <= request.StartTime)
                    return Result<string>.Failure("Agenda item end time must be after start time");

                var item = new Domain.Entities.EventAgendaItem
                {
                    EventId = @event.Id,
                    Title = request.Title,
                    Description = request.Description,
                    StartTime = request.StartTime,
                    EndTime = request.EndTime,
                    DisplayOrder = @event.AgendaItems.Count
                };

                await eventRepository.AddAgendaItemAsync(item);

                @event.DateModified = DateTime.UtcNow;

                await unitOfWork.SaveAsync();

                return Result<string>.Success(item.Id.ToString(), "Agenda item added successfully");
            }
        }
    }
}