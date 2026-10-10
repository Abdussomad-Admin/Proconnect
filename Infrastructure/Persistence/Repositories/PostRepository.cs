using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Application.Common.Repositories;
using Domain.Entities;
using Domain.Enums;
using Infrastructure.Persistence.Context;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public class PostRepository(ProConnectDbContext context) : IPostRepository
    {
        public async Task AddAsync(Post post)
        {
            await context.Posts.AddAsync(post);
        }

        public async Task<Post?> GetByIdAsync(Guid id)
        {
            return await context.Posts
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == id && !p.IsDeleted);
        }

        public async Task<PageResponse<Post>> GetFeedByAuthorIdsAsync(
            IEnumerable<Guid> authorIds,
            PageRequest request,
            bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => authorIds.Contains(p.UserId) && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<PageResponse<Post>> GetAdminListAsync(AdminPostFilter filter, PageRequest pageRequest, bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.Reports)
                .AsQueryable();

            if (!string.IsNullOrWhiteSpace(filter.Search))
                query = query.Where(p => p.Content.Contains(filter.Search) || p.User.UserName.Contains(filter.Search));

            if (filter.Status == "Removed")
                query = query.Where(p => p.IsDeleted);
            else if (filter.Status == "Flagged")
                query = query.Where(p => !p.IsDeleted && p.Reports.Any(r => r.ReportStatus == ReportStatus.Pending));
            else if (filter.Status == "Active")
                query = query.Where(p => !p.IsDeleted && !p.Reports.Any(r => r.ReportStatus == ReportStatus.Pending));
            else
                query = query.Where(p => !p.IsDeleted);

            if (filter.PostedFrom.HasValue)
                query = query.Where(p => p.DateCreated >= filter.PostedFrom.Value);

            if (filter.PostedTo.HasValue)
                query = query.Where(p => p.DateCreated < filter.PostedTo.Value);

            query = query.OrderByDescending(p => p.DateCreated);

            if (usePaging)
            {
                var offset = query.Skip((pageRequest.PageNumber - 1) * pageRequest.PageSize).Take(pageRequest.PageSize);

                return new PageResponse<Post>
                {
                    Items = await offset.ToListAsync(),
                    TotalCount = await query.CountAsync(),
                    PageNumber = pageRequest.PageNumber,
                    PageSize = pageRequest.PageSize
                };
            }

            var count = await query.CountAsync();

            return new PageResponse<Post>
            {
                Items = await query.ToListAsync(),
                TotalCount = count,
                PageNumber = 1,
                PageSize = count
            };
        }

        public async Task<PageResponse<Post>> GetAllPublicAsync(PageRequest request, bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => p.Visibility == Visibility.Public && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<PageResponse<Post>> GetByUserIdAsync(Guid userId, PageRequest request, bool usePaging)
        {
            var query = context.Posts
                .AsNoTracking()
                .Include(p => p.User)
                .Include(p => p.Attachments.Where(a => !a.IsDeleted))
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.User)
                .Include(p => p.OriginalPost)
                    .ThenInclude(op => op!.Attachments.Where(a => !a.IsDeleted))
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .OrderByDescending(p => p.DateCreated)
                .AsQueryable();

            return await PaginateAsync(query, request, usePaging);
        }

        public async Task<int> GetCommentsCountAsync(Guid postId)
        {
            return await context.Comments
                .Where(c => c.PostId == postId)
                .CountAsync();
        }

        public async Task<int> GetSharesCountAsync(Guid postId)
        {
            return await context.Posts
                .Where(p => p.OriginalPostId == postId && !p.IsDeleted)
                .CountAsync();
        }

        public void Update(Post post)
        {
            context.Posts.Update(post);
        }

        public async Task<List<IPostRepository.DateCountDto>> GetPostCountTrendAsync(
            Guid userId, DateTime start, DateTime end)
        {
            return await context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeleted
                    && p.DateCreated >= start && p.DateCreated <= end)
                .GroupBy(p => p.DateCreated.Date)
                .Select(g => new IPostRepository.DateCountDto { Date = g.Key, Count = g.Count() })
                .OrderBy(x => x.Date)
                .ToListAsync();
        }

        public async Task<int> GetReactionCountAsync(Guid userId, DateTime start, DateTime end)
        {
            return await context.PostLikes
                .AsNoTracking()
                .Where(pl => pl.Post.UserId == userId && !pl.IsDeleted
                    && pl.DateCreated >= start && pl.DateCreated <= end)
                .CountAsync();
        }

        public async Task<int> GetCommentCountForUserPostsAsync(Guid userId, DateTime start, DateTime end)
        {
            return await context.Comments
                .AsNoTracking()
                .Where(c => c.Post.UserId == userId
                    && c.DateCreated >= start && c.DateCreated <= end)
                .CountAsync();
        }

        public async Task<int> GetShareCountForUserPostsAsync(Guid userId, DateTime start, DateTime end)
        {
            return await context.Posts
                .AsNoTracking()
                .Where(p => p.OriginalPost != null && p.OriginalPost.UserId == userId && !p.IsDeleted
                    && p.DateCreated >= start && p.DateCreated <= end)
                .CountAsync();
        }

        public async Task<List<Guid>> GetPostIdsByUserAsync(Guid userId)
        {
            return await context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeleted)
                .Select(p => p.Id)
                .ToListAsync();
        }

        public async Task<Dictionary<Guid, string>> GetContentExcerptsByIdsAsync(List<Guid> postIds)
        {
            if (postIds.Count == 0) return new Dictionary<Guid, string>();

            return await context.Posts
                .AsNoTracking()
                .Where(p => postIds.Contains(p.Id))
                .Select(p => new { p.Id, p.Content })
                .ToDictionaryAsync(x => x.Id, x => x.Content);
        }

        public async Task<List<IPostRepository.TopPostDto>> GetTopPostsByEngagementAsync(
            Guid userId, DateTime start, DateTime end, int take)
        {
            var posts = await context.Posts
                .AsNoTracking()
                .Where(p => p.UserId == userId && !p.IsDeleted
                    && p.DateCreated >= start && p.DateCreated <= end)
                .Select(p => new
                {
                    p.Id,
                    p.Content,
                    p.DateCreated,
                    ReactionCount = p.PostLikes.Count(pl => !pl.IsDeleted),
                    CommentCount = p.Comments.Count()
                })
                .ToListAsync();

            var postIds = posts.Select(p => p.Id).ToList();
            var shareCounts = await context.Posts
                .AsNoTracking()
                .Where(p => p.OriginalPostId != null && postIds.Contains(p.OriginalPostId.Value) && !p.IsDeleted)
                .GroupBy(p => p.OriginalPostId!.Value)
                .Select(g => new { PostId = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.PostId, x => x.Count);

            return posts
                .Select(p => new IPostRepository.TopPostDto
                {
                    PostId = p.Id,
                    Content = p.Content,
                    DateCreated = p.DateCreated,
                    ReactionCount = p.ReactionCount,
                    CommentCount = p.CommentCount,
                    ShareCount = shareCounts.TryGetValue(p.Id, out var sc) ? sc : 0
                })
                .OrderByDescending(p => p.ReactionCount + p.CommentCount + p.ShareCount)
                .Take(take)
                .ToList();
        }

        private static async Task<PageResponse<Post>> PaginateAsync(
            IQueryable<Post> query,
            PageRequest request,
            bool usePaging)
        {
            if (!usePaging)
            {
                var allItems = await query.ToListAsync();

                return new PageResponse<Post>
                {
                    Items = allItems,
                    TotalCount = allItems.Count,
                    PageNumber = 1,
                    PageSize = allItems.Count
                };
            }

            var totalCount = await query.CountAsync();

            var items = await query
                .Skip((request.PageNumber - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            return new PageResponse<Post>
            {
                Items = items,
                TotalCount = totalCount,
                PageNumber = request.PageNumber,
                PageSize = request.PageSize
            };
        }

        public async Task<Dictionary<ReactionType, int>> GetReactionBreakdownAsync(Guid userId, DateTime start, DateTime end)
        {
            return await context.PostLikes
                .AsNoTracking()
                .Where(pl => pl.Post.UserId == userId && !pl.IsDeleted
                    && pl.DateCreated >= start && pl.DateCreated <= end)
                .GroupBy(pl => pl.ReactionType)
                .Select(g => new { Type = g.Key, Count = g.Count() })
                .ToDictionaryAsync(x => x.Type, x => x.Count);
        }
    }
}