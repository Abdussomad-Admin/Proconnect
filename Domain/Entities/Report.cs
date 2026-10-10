using Domain.Enums;

namespace Domain.Entities
{
    public class Report
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ReporterId { get; set; }

        public User Reporter { get; set; } = default!;

        public Guid ReportedUserId { get; set; }

        public User ReportedUser { get; set; } = default!;
        
        public Guid? PostId { get; set; }

        public Post? Post { get; set; } = default!;

        public string ContentType { get; set; } = default!;

        public bool IsDeleted { get; set; }

        public ReportReason Reason { get; set; }

        public string CreatedBy { get; set; } = default!;

        public ReportStatus ReportStatus { get; set; } = ReportStatus.Pending;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}
