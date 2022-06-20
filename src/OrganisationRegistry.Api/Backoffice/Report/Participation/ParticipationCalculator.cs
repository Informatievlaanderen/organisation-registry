namespace OrganisationRegistry.Api.Backoffice.Report.Participation;

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
        => totalCount <= 1
            ? BodyParticipationCompliance.Unknown
            : percentage >= Lower && percentage <= Upper
                ? BodyParticipationCompliance.Compliant
                : BodyParticipationCompliance.NonCompliant;

    public static decimal CalculatePercentage(int count, decimal assignedCount)
    {
        if (assignedCount == 0)
            return decimal.Zero;

        return Math.Round(count / assignedCount, 2);
    }
}
