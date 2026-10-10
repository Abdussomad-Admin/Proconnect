using Application.Common.Dtos;
using Application.Common.Repositories;
using Application.Services.Interfaces;
using Domain.Enums;
using MediatR;

namespace Application.Commands.Networking
{
    public class AcceptConnectionRequest
    {
        public record AcceptConnectionRequestCommand(Guid UserId, Guid ConnectionId) : IRequest<Result<ConnectionResponse>>;

        public class AcceptConnectionRequestHandler(
            IUserConnectionRepository connectionRepository,
            IUserRepository userRepository,
            INotificationService notificationService,
            IUnitOfWork unitOfWork) : IRequestHandler<AcceptConnectionRequestCommand, Result<ConnectionResponse>>
        {
            public async Task<Result<ConnectionResponse>> Handle(AcceptConnectionRequestCommand request, CancellationToken cancellationToken)
            {
                var connection = await connectionRepository.GetByIdAsync(request.ConnectionId);

                if (connection is null)
                {
                    return Result<ConnectionResponse>.Failure("Connection request not found");
                }

                if (connection.RecieverId != request.UserId)
                {
                    return Result<ConnectionResponse>.Failure("You are not authorized to respond to this request");
                }

                if (connection.ConnectionStatus != ConnectionStatus.Pending)
                {
                    return Result<ConnectionResponse>.Failure("This request has already been responded to");
                }

                connection.ConnectionStatus = ConnectionStatus.Accepted;
                connection.DateUpdated = DateTime.UtcNow;

                connectionRepository.Update(connection);
                await unitOfWork.SaveAsync();

                var accepter = await userRepository.GetByIdAsync(request.UserId);
                var accepterName = accepter is not null ? $"{accepter.FirstName} {accepter.LastName}".Trim() : "Someone";

                await notificationService.SendNotificationAsync(
                    recipientUserId: connection.SenderId,
                    actorUserId: request.UserId,
                    actorName: accepterName,
                    actorAvatarUrl: accepter?.ProfilePictureUrl,
                    title: "Connection request accepted",
                    message: $"{accepterName} accepted your connection request",
                    type: NotificationType.ConnectionAccepted,
                    sourceEntityType: NotificationSourceEntityType.ConnectionRequest,
                    sourceEntityId: connection.Id,
                    actionUrl: $"/profile.html?userId={request.UserId}",
                    createdBy: request.UserId.ToString());

                return Result<ConnectionResponse>.Success(
                    new ConnectionResponse(connection.Id, connection.SenderId, connection.RecieverId, connection.ConnectionStatus, connection.DateCreated),
                    "Connection request accepted");
            }
        }
    }
}