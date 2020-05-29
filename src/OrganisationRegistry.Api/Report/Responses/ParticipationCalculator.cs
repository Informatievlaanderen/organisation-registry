namespace OrganisationRegistry.Api.Report.Responses
{
    using System;

    public static class ParticipationCalculator
    {
        private static readonly decimal Lower;
        private static readonly decimal Upper;

        static ParticipationCalculator()
        {
            Lower = Math.Floor(1m / 3 * 100) / 100;
            Upper = Math.Ceiling(2m / 3 * 100) / 100;
        }

        public static BodyParticipationCompliance CalculateCompliance(int totalCount, decimal percentage)
        {
            return totalCount <= 1
                ? BodyParticipationCompliance.Unknown
                : percentage >= Lower && percentage <= Upper
                    ? BodyParticipationCompliance.Compliant
                    : BodyParticipationCompliance.NonCompliant;
        }

        public static decimal CalculatePercentage(int count, decimal assignedCount)
        {
            return Math.Round(count / assignedCount, 2);
        }
    }
}
