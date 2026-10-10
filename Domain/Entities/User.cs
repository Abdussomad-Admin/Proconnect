using Domain.Enums;

namespace Domain.Entities
{
    public class User
    {
        public Guid Id { get; set; } = Guid.NewGuid();  

        public string FirstName { get; set; } = default!;

        public string LastName { get; set; } = default!;

        public string UserName { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string HashedPassword { get; set; } = default!;

        public string? Tel { get; set; }

        public string Role { get; set; } = default!;

        public string? ProfilePictureUrl { get; set; }

        public string? ProfilePicturePublicId { get; set; }

        public string Bio { get; set; } = default!;

        public string? Location { get; set; } = default!;

        public bool IsVerified { get; set; }

        public UserStatus Status { get; set; } = UserStatus.PendingVerification;

        public string? VerificationToken { get; set; }

        public DateTime? VerificationTokenExpiry { get; set; }

        public string? PasswordResetToken { get; set; }

        public DateTime? PasswordResetTokenExpiry { get; set; }

        public string? RefreshToken { get; set; }

        public DateTime? RefreshTokenExpiry { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public ProfessionalProfile? ProfessionalProfile { get; set; }

        public RecruiterProfile? RecruiterProfile { get; set; }

        public ICollection<FileUpload> Files { get; set; } = new HashSet<FileUpload>();

        public ICollection<AuditLog> AuditLogs { get; set; } = new HashSet<AuditLog>();
        
        public ICollection<Report> ReportsSubmitted { get; set; } = new HashSet<Report>();

        public ICollection<Report> ReportsReceived { get; set; } = new HashSet<Report>();

        public ICollection<Notification> Notifications { get; set; } = new HashSet<Notification>();

        public ICollection<UserFollow> Following { get; set; } = new HashSet<UserFollow>();

        public ICollection<UserFollow> Followers { get; set; } = new HashSet<UserFollow>();

        public ICollection<Post> Posts { get; set; } = new HashSet<Post>();

        public ICollection<Comment> Comments { get; set; } = new HashSet<Comment>();

        public ICollection<PostLike> PostLikes { get; set; } = new HashSet<PostLike>(); 
        
        public ICollection<UserConnection> SentConnections { get; set; } = new HashSet<UserConnection>();

        public ICollection<UserConnection> ReceivedConnections { get; set; } = new HashSet<UserConnection>();

        public ICollection<Message> SentMessages { get; set; } = new HashSet<Message>();

        public ICollection<ConversationParticipant> Participants { get; set; } = new HashSet<ConversationParticipant>();

        public ICollection<AnalyticsEvent> AnalyticsEvents { get; set; } = new HashSet<AnalyticsEvent>();
    }
}
