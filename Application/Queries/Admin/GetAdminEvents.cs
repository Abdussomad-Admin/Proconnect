using Application.Common.Dtos;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminEvents
    {
        public record GetAdminEventsQuery(
            string? Search,
            EventStatus? Status,
            string? Location,
            DateTime? StartFrom,
            DateTime? StartTo,
            PageRequest PageRequest,
            bool UsePaging) : IRequest<Result<PageResponse<AdminEventListItem>>>;

        public class GetAdminEventsHandler(IEventRepository eventRepository)
            : IRequestHandler<GetAdminEventsQuery, Result<PageResponse<AdminEventListItem>>>
        {
            public async Task<Result<PageResponse<AdminEventListItem>>> Handle(
                GetAdminEventsQuery request, CancellationToken cancellationToken)
            {
                var filter = new AdminEventFilter(request.Search, request.Status, request.Location, request.StartFrom, request.StartTo);

                var events = await eventRepository.GetAdminListAsync(filter, request.PageRequest, request.UsePaging);

                var response = new PageResponse<AdminEventListItem>
                {
                    Items = events.Items.Select(e => new AdminEventListItem(
                        e.Id,
                        e.Title,
                        e.CompanyId,
                        e.Company.Name,
                        e.Company.LogoUrl,
                        e.EventType.ToString(),
                        e.LocationType.ToString(),
                        e.Location,
                        e.StartDateTime,
                        e.EndDateTime,
                        e.Status.ToString(),
                        e.Registrations.Count(r => r.Status == EventRegistrationStatus.Registered),
                        e.AttendeeLimit)).ToList(),
                    TotalCount = events.TotalCount,
                    PageNumber = events.PageNumber,
                    PageSize = events.PageSize
                };

                return Result<PageResponse<AdminEventListItem>>.Success(response, "Events retrieved successfully");
            }
        }
    }

    public record AdminEventListItem(
        Guid EventId,
        string Title,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        string EventType,
        string LocationType,
        string? Location,
        DateTime StartDateTime,
        DateTime EndDateTime,
        string Status,
        int RegisteredCount,
        int? AttendeeLimit);
}