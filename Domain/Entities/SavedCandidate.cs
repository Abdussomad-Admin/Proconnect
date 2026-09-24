namespace Domain.Entities
{
    public class SavedCandidate
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid RecruiterProfileId { get; set; }

        public RecruiterProfile RecruiterProfile { get; set; } = default!;

        public Guid ProfessionalProfileId { get; set; }

        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public string CreatedBy { get; set; } = default!;

        public bool IsDeleted { get; set; }
    }
}