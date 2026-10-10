using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Enums;
using MediatR;

namespace Application.Queries.Admin
{
    public class GetAdminEventDetails
    {
        public record GetAdminEventDetailsQuery(Guid EventId) : IRequest<Result<AdminEventDetailsResponse>>;

        public class GetAdminEventDetailsHandler(IEventRepository eventRepository)
            : IRequestHandler<GetAdminEventDetailsQuery, Result<AdminEventDetailsResponse>>
        {
            public async Task<Result<AdminEventDetailsResponse>> Handle(
                GetAdminEventDetailsQuery request, CancellationToken cancellationToken)
            {
                var e = await eventRepository.GetWithDetailsAsync(request.EventId);

                if (e is null)
                    return Result<AdminEventDetailsResponse>.Failure("Event not found");

                var response = new AdminEventDetailsResponse(
                    e.Id,
                    e.Title,
                    e.Description,
                    e.CoverImageUrl,
                    e.CompanyId,
                    e.Company.Name,
                    e.Company.LogoUrl,
                    e.EventType.ToString(),
                    e.Category,
                    e.StartDateTime,
                    e.EndDateTime,
                    e.LocationType.ToString(),
                    e.Location,
                    e.OnlineMeetingLink,
                    e.Visibility.ToString(),
                    e.RegistrationType.ToString(),
                    e.AttendeeLimit,
                    e.Status.ToString(),
                    e.PublishedAt,
                    e.CancelledAt,
                    e.CancellationReason,
                    e.RecruiterProfile.User.FirstName + " " + e.RecruiterProfile.User.LastName,
                    e.Registrations.Count(r => r.Status == EventRegistrationStatus.Registered),
                    e.Speakers.Count,
                    e.AgendaItems.Count,
                    e.DateCreated);

                return Result<AdminEventDetailsResponse>.Success(response, "Event details retrieved successfully");
            }
        }
    }

    public record AdminEventDetailsResponse(
        Guid EventId,
        string Title,
        string Description,
        string? CoverImageUrl,
        Guid CompanyId,
        string CompanyName,
        string? CompanyLogoUrl,
        string EventType,
        string? Category,
        DateTime StartDateTime,
        DateTime EndDateTime,
        string LocationType,
        string? Location,
        string? OnlineMeetingLink,
        string Visibility,
        string RegistrationType,
        int? AttendeeLimit,
        string Status,
        DateTime? PublishedAt,
        DateTime? CancelledAt,
        string? CancellationReason,
        string PostedByRecruiter,
        int RegisteredCount,
        int SpeakersCount,
        int AgendaItemsCount,
        DateTime CreatedDate);
}