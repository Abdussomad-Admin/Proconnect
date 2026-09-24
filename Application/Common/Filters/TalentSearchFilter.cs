using Domain.Enums;

namespace Application.Common.Filters
{
    public class TalentSearchFilter
    {
        public List<Guid>? SkillIds { get; set; }

        public int? MinYearsExperience { get; set; }

        public int? MaxYearsExperience { get; set; }
        public string? EducationLevel { get; set; }
        public string? Location { get; set; }

        public AvailabilityStatus? Availability { get; set; }
        public string? Keyword { get; set; }
    }
}