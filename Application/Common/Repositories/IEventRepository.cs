using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IEventRepository
    {
        Task AddAsync(Event @event);

        Task AddSpeakerAsync(EventSpeaker speaker);

        Task AddAgendaItemAsync(EventAgendaItem item);

        Task<Event?> GetByIdAsync(Guid id);

        Task<Event?> GetWithDetailsAsync(Guid id);

        Task<PageResponse<Event>> SearchAsync(
            PageRequest request,
            bool usePaging,
            string? searchTerm,
            List<EventType>? eventTypes,
            EventLocationType? locationType,
            string? location,
            DateTime? startDateFrom,
            DateTime? startDateTo);

        Task<PageResponse<Event>> GetByRecruiterProfileIdAsync(
            PageRequest request,
            bool usePaging,
            Guid recruiterProfileId,
            EventStatus? status,
            EventType? eventType);

        void Update(Event @event);

        void Delete(Event @event);
    }
}