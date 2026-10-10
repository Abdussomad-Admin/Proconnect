using Application.Common.Dtos.Admin;
using Application.Common.Pagenation;
using Domain.Entities;
using Domain.Enums;

namespace Application.Common.Repositories
{
    public interface IPostRepository
    {
        Task AddAsync(Post post);

        Task<Post?> GetByIdAsync(Guid id);

        Task<PageResponse<Post>> GetFeedByAuthorIdsAsync(IEnumerable<Guid> authorIds, PageRequest request, bool usePaging);

        Task<PageResponse<Post>> GetAllPublicAsync(PageRequest request, bool usePaging);

        Task<PageResponse<Post>> GetByUserIdAsync(Guid userId, PageRequest request, bool usePaging);

        Task<Dictionary<ReactionType, int>> GetReactionBreakdownAsync(Guid userId, DateTime start, DateTime end);

        Task<PageResponse<Post>> GetAdminListAsync(AdminPostFilter filter, PageRequest pageRequest, bool usePaging);

        Task<int> GetCommentsCountAsync(Guid postId);

        Task<int> GetSharesCountAsync(Guid postId);

        void Update(Post post);

        Task<List<DateCountDto>> GetPostCountTrendAsync(Guid userId, DateTime start, DateTime end);

        Task<int> GetReactionCountAsync(Guid userId, DateTime start, DateTime end);

        Task<int> GetCommentCountForUserPostsAsync(Guid userId, DateTime start, DateTime end);

        Task<int> GetShareCountForUserPostsAsync(Guid userId, DateTime start, DateTime end);

        Task<List<TopPostDto>> GetTopPostsByEngagementAsync(Guid userId, DateTime start, DateTime end, int take);

        Task<List<Guid>> GetPostIdsByUserAsync(Guid userId);

        Task<Dictionary<Guid, string>> GetContentExcerptsByIdsAsync(List<Guid> postIds);

        public class DateCountDto
        {
            public DateTime Date { get; set; }
            public int Count { get; set; }
        }

        public class TopPostDto
        {
            public Guid PostId { get; set; }
            public string Content { get; set; } = default!;
            public DateTime DateCreated { get; set; }
            public int ReactionCount { get; set; }
            public int CommentCount { get; set; }
            public int ShareCount { get; set; }
        }
    }
}