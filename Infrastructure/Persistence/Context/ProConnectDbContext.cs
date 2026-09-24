using Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Context
{
    public class ProConnectDbContext : DbContext
    {
        public ProConnectDbContext(DbContextOptions<ProConnectDbContext> options) : base(options)
        {
        }

        public DbSet<AuditLog> AuditLogs { get; set; }

        public DbSet<AnalyticsEvent> AnalyticsEvents { get; set; }

        public DbSet<Certificate> Certificates { get; set; }

        public DbSet<Comment> Comments { get; set; }

        public DbSet<Company> Companies { get; set; }

        public DbSet<Conversation> Conversations { get; set; }

        public DbSet<CompanyReview> CompanyReviews { get; set; }

        public DbSet<ConversationParticipant> ConversationParticipants { get; set; }

        public DbSet<Department> Departments { get; set; }

        public DbSet<Education> Educations { get; set; }

        public DbSet<Experience> Experiences { get; set; }

        public DbSet<Event> Events { get; set; }

        public DbSet<EventAgendaItem>  EventAgendaItems { get; set; }

        public DbSet<EventRegistration> EventRegistrations { get; set; }

        public DbSet<EventSpeaker> EventSpeakers { get; set; }

        public DbSet<FileUpload> FileUploads { get; set; }

        public DbSet<Job> Jobs { get; set; }

        public DbSet<JobApplication> JobApplications { get; set; }

        public DbSet<JobSkill> JobSkills { get; set; }

        public DbSet<Message> Messages { get; set; }

        public DbSet<Notification> Notifications { get; set; }

        public DbSet<Post> Posts { get; set; }

        public DbSet<PostLike> PostLikes { get; set; }

        public DbSet<ProfessionalProfile> ProfessionalProfiles { get; set; }

        public DbSet<ProfessionalSkill> ProfessionalSkills { get; set; }

        public DbSet<SavedJobSearch> SavedJobSearches { get; set; }

        public DbSet<Project> Projects { get; set; }

        public DbSet<PortfolioLink> PortfolioLinks { get; set; }

        public DbSet<RecruiterProfile> RecruiterProfiles { get; set; }

        public DbSet<Report> Reports { get; set; }

        public DbSet<SavedJob> SavedJobs { get; set; }

        public DbSet<SavedEvent> SavedEvents { get; set; }

        public DbSet<SavedCandidate> SavedCandidates { get; set; }

        public DbSet<Skill> Skills { get; set; }

        public DbSet<User> Users { get; set; }

        public DbSet<UserFollow> UserFollows { get; set; }

        public DbSet<UserConnection> UserConnections { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.ApplyConfigurationsFromAssembly(
                typeof(ProConnectDbContext).Assembly);

            var user = new User
            {
                Id = Guid.Parse("c117635d-96e0-409b-9fae-72976ec9c42a"),
                Email = "admin@gmail.com",
                Role = "admin",
                CreatedBy = "system",
                UserName = "Administrator",
                IsVerified = true,
                Bio = "I am the administrator of this platform.",
                FirstName = "Ajibike",
                LastName = "Abdussomad",
                Location = "Ogun State, Nigeria",
            };

            string password = $"admin123";
            user.HashedPassword = new PasswordHasher<User>().HashPassword(user, password);

            var conversationId = Guid.Parse("b235f2ed-bb4e-4bd2-a03d-0e3c17aaf2e2");

            var conversation = new Conversation
            {
                Id = conversationId,
                Title = "Soulshelf Group Chat",
                CreatedBy = user.Id.ToString()
            };

            var userConversation = new ConversationParticipant
            {
                ConversationId = conversationId,
                UserId = user.Id,
                CreatedBy = user.Id.ToString()
            };

            modelBuilder.Entity<User>()
            .Property(u => u.Id)
            .HasConversion(
            id => id.ToString().ToLowerInvariant(),
            str => Guid.Parse(str));

            modelBuilder.Entity<User>().HasData(user);
            modelBuilder.Entity<Conversation>().HasData(conversation);
            modelBuilder.Entity<ConversationParticipant>().HasData(userConversation);
        }
    }
}
