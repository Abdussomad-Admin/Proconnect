using Application.Common.Filters;
using Application.Services.Interfaces;
using Domain.Entities;
using Domain.Enums;

namespace Infrastructure.Services
{
    public class MatchScoreCalculator(ILocationMatcher locationMatcher) : IMatchScoreCalculator
    {
        public double Calculate(ProfessionalProfile candidate, TalentSearchFilter filter, Company? company)
        {
            double score = 0;
            double weightUsed = 0;

            if (filter.SkillIds is { Count: > 0 })
            {
                var matchedCount = candidate.ProfessionalSkills
                    .Count(ps => !ps.IsDeleted && filter.SkillIds.Contains(ps.SkillId));

                score += 0.5 * ((double)matchedCount / filter.SkillIds.Count);
                weightUsed += 0.5;
            }

            var availabilityScore = candidate.AvailabilityStatus switch
            {
                AvailabilityStatus.Available => 1.0,
                AvailabilityStatus.OpenToOffers => 0.6,
                _ => 0.2
            };
            score += 0.2 * availabilityScore;
            weightUsed += 0.2;

            if (company is not null)
            {
                score += 0.2 * locationMatcher.Score(candidate.User.Location, company.Locations);
                weightUsed += 0.2;
            }

            // Experience always contributes now — previously this whole factor
            // was skipped when no years filter was active on the search, which
            // meant a 5-year candidate and a 0-year candidate scored identically
            // on any unfiltered browse.
            var totalYears = TotalYearsExperience(candidate);

            double experienceScore;
            if (filter.MinYearsExperience.HasValue || filter.MaxYearsExperience.HasValue)
            {
                // A years filter is active on this search — reward candidates
                // who actually fall inside the requested band.
                var withinRange =
                    (!filter.MinYearsExperience.HasValue || totalYears >= filter.MinYearsExperience.Value) &&
                    (!filter.MaxYearsExperience.HasValue || totalYears <= filter.MaxYearsExperience.Value);
                experienceScore = withinRange ? 1.0 : 0.0;
            }
            else
            {
                // No years filter — more experience is generally better, capped
                // at 10 years so a 20-year veteran doesn't dwarf everyone else.
                // Adjust the cap if 10 isn't the right ceiling for your market.
                experienceScore = Math.Min(totalYears, 10) / 10.0;
            }

            score += 0.1 * experienceScore;
            weightUsed += 0.1;

            if (weightUsed == 0) return 0;

            return Math.Round(score / weightUsed * 100, 0);
        }

        private static int TotalYearsExperience(ProfessionalProfile candidate) =>
            candidate.Experiences
                .Where(e => !e.IsDeleted)
                .Sum(e => ((e.IsCurrentJob ? DateTime.UtcNow : e.EndDate) - e.StartDate).Days / 365);
    }
}