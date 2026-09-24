namespace Domain.Entities
{
    public class Company
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public string Name { get; set; } = default!;

        public string Industry { get; set; } = default!;

        public string Description { get; set; } = default!;

        public string? Website { get; set; } = default!;

        public string Email { get; set; } = default!;

        public string PhoneNumber { get; set; } = default!;

        public string? Logo { get; set; } = default!;

        public bool IsVerified { get; set; }

        public DateTime? VerifiedAt { get; set; }

        public int? FoundedYear { get; set; }

        public string CompanySize { get; set; } = default!;

        public string CompanyType { get; set; } = default!;

        public string? Headquarters { get; set; }

        public string? LogoUrl { get; set; }

        public string? LogoPublicId { get; set; }

        public string? CoverImageUrl { get; set; }

        public string? CoverImagePublicId { get; set; }

        public string? LinkedInUrl { get; set; }

        public string? TwitterUrl { get; set; }

        public string? FacebookUrl { get; set; }

        public string? InstagramUrl { get; set; }

        public string? InvitationCode { get; set; }

        public DateTime? InvitationCodeExpiry { get; set; }

        public string? Locations { get; set; }

        public string? Strengths { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();

        public ICollection<RecruiterProfile> RecruiterProfiles { get; set; } = new HashSet<RecruiterProfile>();

        public ICollection<Event> Events { get; set; } = new HashSet<Event>();
    }
}