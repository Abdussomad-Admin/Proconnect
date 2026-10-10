using Application.Common.Dtos;
using Application.Common.Repositories;
using Domain.Entities;
using MediatR;

namespace Application.Commands.Admin
{
    public class UpdatePostVisibility
    {
        public record UpdatePostVisibilityCommand(
            Guid PostId,
            Guid ActingAdminId,
            bool IsDeleted,
            string Action,
            string? Reason) : IRequest<Result<object>>;

        public class UpdatePostVisibilityHandler(
            IPostRepository postRepository,
            IAuditLogRepository auditLogRepository,
            IUnitOfWork unitOfWork)
            : IRequestHandler<UpdatePostVisibilityCommand, Result<object>>
        {
            public async Task<Result<object>> Handle(UpdatePostVisibilityCommand request, CancellationToken cancellationToken)
            {
                var post = await postRepository.GetByIdAsync(request.PostId);

                if (post is null)
                    return Result<object>.Failure("Post not found");

                if (post.IsDeleted == request.IsDeleted)
                    return Result<object>.Failure(request.IsDeleted ? "Post is already removed" : "Post is not removed");

                post.IsDeleted = request.IsDeleted;
                post.DateUpdated = DateTime.UtcNow;
                postRepository.Update(post);

                await auditLogRepository.AddAsync(new AuditLog
                {
                    UserId = request.ActingAdminId,
                    Action = request.Action,
                    Description = string.IsNullOrWhiteSpace(request.Reason)
                        ? $"{request.Action} post by {post.User.UserName}"
                        : $"{request.Action} post by {post.User.UserName}: {request.Reason}",
                    CreatedBy = request.ActingAdminId.ToString()
                });

                await unitOfWork.SaveAsync();

                return Result<object>.Success(new { }, $"Post {request.Action.ToLower()}d successfully");
            }
        }
    }
}