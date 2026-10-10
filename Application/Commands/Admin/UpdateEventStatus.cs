using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateEventStatus
    {
        public record UpdateEventStatusCommand(
            Guid EventId,
            Guid ActingAdminId,
            EventStatus NewStatus,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdateEventStatusHandler(
            IEventRepository eventRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateEventStatusCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateEventStatusCommand request, CancellationToken cancellationToken)
            {
                var ev = await eventRepository.GetByIdAsync(request.EventId);

                if (ev is null)
                    return Result<object>.Failure("Event not found");

                if (ev.Status == request.NewStatus)
                    return Result<object>.Failure($"Event is already {request.NewStatus}");

                var previousStatus = ev.Status;
                ev.Status = request.NewStatus;

                if (request.NewStatus == EventStatus.Published)
                    ev.PublishedAt = DateTime.UtcNow;

                if (request.NewStatus == EventStatus.Cancelled)
                {
                    ev.CancelledAt = DateTime.UtcNow;
                    ev.CancellationReason = request.Reason;
                }

                ev.DateModified = DateTime.UtcNow;
                eventRepository.Update(ev);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"Changed event '{ev.Title}' status from {previousStatus} to {request.NewStatus}"
                        : $"Changed event '{ev.Title}' status from {previousStatus} to {request.NewStatus}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"Event {request.Action.ToLower()}ed successfully");
            }
        }
    }
}