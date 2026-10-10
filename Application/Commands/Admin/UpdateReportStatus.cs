using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdateReportStatus
    {
        public record UpdateReportStatusCommand(
            Guid ReportId,
            Guid ActingAdminId,
            ReportStatus NewStatus,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdateReportStatusHandler(
            IReportRepository reportRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdateReportStatusCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdateReportStatusCommand request, CancellationToken cancellationToken)
            {
                var report = await reportRepository.GetByIdAsync(request.ReportId);

                if (report is null)
                    return Result<object>.Failure("Report not found");

                if (report.ReportStatus == request.NewStatus)
                    return Result<object>.Failure($"Report is already {request.NewStatus}");

                var previousStatus = report.ReportStatus;
                report.ReportStatus = request.NewStatus;
                reportRepository.Update(report);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"Changed report {report.Id} status from {previousStatus} to {request.NewStatus}"
                        : $"Changed report {report.Id} status from {previousStatus} to {request.NewStatus}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"Report {request.Action.ToLower()}d successfully");
            }
        }
    }
}