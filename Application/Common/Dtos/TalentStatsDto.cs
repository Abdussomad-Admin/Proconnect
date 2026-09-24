namespace Application.Common.Dtos
{
    public record TopSkillDto(string SkillName, int CandidateCount);

    public class TalentStatsDto
    {
        public int TotalCandidates { get; set; }

        public int NewThisWeek { get; set; }

        public double AverageMatchRatePercent { get; set; }
        public int HiresThisMonth { get; set; }

        public List<TopSkillDto> TopSkills { get; set; } = new();
    }
}