namespace Domain.Entities
{
    public class Department
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid CompanyId { get; set; }

        public Company Company { get; set; } = default!;

        public string Name { get; set; } = default!;

        public string? Description { get; set; }

        public bool IsDeleted { get; set; }

        public string CreatedBy { get; set; } = default!;

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;

        public DateTime DateModified { get; set; }

        public ICollection<Job> Jobs { get; set; } = new HashSet<Job>();

        public ICollection<SavedJobSearch> SavedJobSearches { get; set; } = new HashSet<SavedJobSearch>();
    }
}