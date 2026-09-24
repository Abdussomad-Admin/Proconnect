using Application.Services.Interfaces;

namespace Infrastructure.Services
{
    public class LocationMatcher : ILocationMatcher
    {
        public double Score(string? candidateLocation, string? companyLocation)
        {
            if (string.IsNullOrWhiteSpace(candidateLocation) || string.IsNullOrWhiteSpace(companyLocation))
                return 0.0;

            var candidateTokens = Normalize(candidateLocation);
            var companyTokens = Normalize(companyLocation);

            if (candidateTokens.Count == 0 || companyTokens.Count == 0)
                return 0.0;

            if (candidateTokens.SetEquals(companyTokens))
                return 1.0;

            return candidateTokens.Overlaps(companyTokens) ? 0.5 : 0.0;
        }

        private static HashSet<string> Normalize(string location) =>
            location
                .Split([',', ';'], StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries)
                .Select(t => t.ToLowerInvariant())
                .ToHashSet();
    }
}