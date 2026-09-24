using Application.Common.Dtos;
using Application.Common.Repositories;
using MediatR;

namespace Application.Commands.Event
{
    public class RemoveEventAgendaItem
    {
        public record RemoveEventAgendaItemCommand(Guid UserId, Guid EventId, Guid AgendaItemId) : IRequest<Result<string>>;

        public class RemoveEventAgendaItemHandler(
            IRecruiterProfileRepository recruiterProfileRepository,
            IEventRepository eventRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<RemoveEventAgendaItemCommand, Result<string>>
        {
            public async Task<Result<string>> Handle(
                RemoveEventAgendaItemCommand request,
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

                @event.AgendaItems.Remove(item);
                @event.DateModified = DateTime.UtcNow;


                await unitOfWork.SaveAsync();

                return Result<string>.Success("removed", "Agenda item removed successfully");
            }
        }
    }
}