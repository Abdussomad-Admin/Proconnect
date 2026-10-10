using Domain.Enums;

namespace Domain.Entities
{
    public class AnalyticsEvent
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public AnalyticsEventType EventType { get; set; }

        public AnalyticsSubjectType SubjectType { get; set; }

        public Guid SubjectId { get; set; }

        public Guid? ActorUserId { get; set; }

        public User? ActorUser { get; set; }

        public ReferrerSource? ReferrerSource { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
    }
}

    public enum AnalyticsSubjectType
    {
        ProfessionalProfile = 1,
        Post = 2,
        Job = 3
    }