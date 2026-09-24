using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class EventRepository(ProConnectDbContext context) : IEventRepository
    {
        public async Task AddAsync(Event @event)
        {
            await context.Events.AddAsync(@event);
        }

        public async Task<Event?> GetByIdAsync(Guid id)
        {
            return await context.Events
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<Event?> GetWithDetailsAsync(Guid id)
        {
            return await context.Events
                .Include(e => e.Company)
                .Include(e => e.RecruiterProfile)
                    .ThenInclude(r => r.User)
                .Include(e => e.Speakers.OrderBy(s => s.DisplayOrder))
                .Include(e => e.AgendaItems.OrderBy(a => a.DisplayOrder))
                .Include(e => e.Registrations.Where(r => r.Status == EventRegistrationStatus.Registered))
                    .ThenInclude(r => r.ProfessionalProfile)
                        .ThenInclude(p => p.User)
                .FirstOrDefaultAsync(e => e.Id == id && !e.IsDeleted);
        }

        public async Task<PageResponse<Event>> SearchAsync(
            PageRequest request,
            bool usePaging,
            string? searchTerm,
            EventType? eventType,
            EventLocationType? locationType,
            string? location,
            DateTime? startDateFrom,
            DateTime? startDateTo)
        {
            var query = context.Events
                .AsNoTracking()
                .Include(e => e.Company)
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.Title.Contains(searchTerm) ||
                    e.Description.Contains(searchTerm));
            }

            if (eventType.HasValue)
            {
                query = query.Where(e => e.EventType == eventType.Value);
            }

            if (locationType.HasValue)
            {
                query = query.Where(e => e.LocationType == locationType.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(e => e.Location != null && e.Location.Contains(location));
            }

            if (startDateFrom.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= startDateTo.Value);
            }

            var ordered = query.OrderBy(e => e.StartDateTime).AsQueryable();

            var totalCount = await ordered.CountAsync();

            if (!usePaging)
            {
                var allItems = await ordered.ToListAsync();

                return new PageResponse<Event>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<Event>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<PageResponse<Event>> GetByRecruiterProfileIdAsync(
    PageRequest request,
    bool usePaging,
    Guid recruiterProfileId,
    EventStatus? status,
    EventType? eventType)
        {
            var query = context.Events
                .Include(e => e.Registrations)
                .Where(e => e.RecruiterProfileId == recruiterProfileId && !e.IsDeleted)
                .AsQueryable();

            if (status.HasValue)
            {
                query = query.Where(e => e.Status == status.Value);
            }

            if (eventType.HasValue)
            {
                query = query.Where(e => e.EventType == eventType.Value);
            }

            var ordered = query.OrderByDescending(e => e.DateCreated).AsQueryable();

            var totalCount = await ordered.CountAsync();

            if (!usePaging)
            {
                var allItems = await ordered.ToListAsync();

                return new PageResponse<Event>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<Event>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public void Update(Event @event)
        {
            context.Events.Update(@event);
        }

        public void Delete(Event @event)
        {
            context.Events.Remove(@event);
        }

        public async Task<PageResponse<Event>> SearchAsync(
    PageRequest request,
    bool usePaging,
    string? searchTerm,
    List<EventType>? eventTypes,
    EventLocationType? locationType,
    string? location,
    DateTime? startDateFrom,
    DateTime? startDateTo)
        {
            var query = context.Events
                .AsNoTracking()
                .Include(e => e.Company)
                .Include(e => e.Registrations)
                .Where(e => !e.IsDeleted && e.Status == EventStatus.Published)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(e =>
                    e.Title.Contains(searchTerm) ||
                    e.Description.Contains(searchTerm));
            }

            if (eventTypes is { Count: > 0 })
            {
                query = query.Where(e => eventTypes.Contains(e.EventType));
            }

            if (locationType.HasValue)
            {
                query = query.Where(e => e.LocationType == locationType.Value);
            }

            if (!string.IsNullOrWhiteSpace(location))
            {
                query = query.Where(e => e.Location != null && e.Location.Contains(location));
            }

            if (startDateFrom.HasValue)
            {
                query = query.Where(e => e.StartDateTime >= startDateFrom.Value);
            }

            if (startDateTo.HasValue)
            {
                query = query.Where(e => e.StartDateTime <= startDateTo.Value);
            }

            var ordered = query.OrderBy(e => e.StartDateTime).AsQueryable();

            var totalCount = await ordered.CountAsync();

            if (!usePaging)
            {
                var allItems = await ordered.ToListAsync();

                return new PageResponse<Event>
                {
                    Items = allItems,
                    TotalCount = totalCount,
                    PageNumber = 1,
                    PageSize = totalCount
                };
            }

            var items = await ordered
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<Event>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task AddSpeakerAsync(EventSpeaker speaker)
        {
            await context.EventSpeakers.AddAsync(speaker);
        }

        public async Task AddAgendaItemAsync(EventAgendaItem item)
        {
            await context.EventAgendaItems.AddAsync(item);
        }
    }
}