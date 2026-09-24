using Domain.Enums;

namespace Domain.Entities
{
    public class SavedJobSearch
    {
        public Guid Id { get; set; } = Guid.NewGuid();

        public Guid ProfessionalProfileId { get; set; }
        public ProfessionalProfile ProfessionalProfile { get; set; } = default!;

        public string? Keyword { get; set; }
        public string? Location { get; set; }
        public Guid? DepartmentId { get; set; }
        public Department? Department { get; set; }
        public EmploymentType? EmploymentType { get; set; }
        public WorkPlaceType? WorkPlaceType { get; set; }
        public ExperienceLevel? ExperienceLevel { get; set; }
        public decimal? MinSalary { get; set; }

        public bool EmailNotificationsEnabled { get; set; } = true;

        public DateTime? LastNotifiedAt { get; set; }

        public DateTime DateCreated { get; set; } = DateTime.UtcNow;
        public bool IsDeleted { get; set; }
        public string CreatedBy { get; set; } = default!;
    }
}