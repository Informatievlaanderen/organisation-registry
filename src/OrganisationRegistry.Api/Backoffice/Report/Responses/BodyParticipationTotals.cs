namespace OrganisationRegistry.Api.Backoffice.Report.Responses
{
    using System.Collections.Generic;
    using System.Linq;

    public class BodyParticipationTotals
    {
        public decimal MalePercentage { get; set; }
        public decimal FemalePercentage { get; set; }
        public decimal UnknownPercentage { get; set; }

        public int MaleCount { get; set; }
        public int FemaleCount { get; set; }
        public int UnknownCount { get; set; }

        public int TotalCount { get; set; }

        public int AssignedCount { get; set; }
        public int UnassignedCount { get; set; }

        public BodyParticipationCompliance Compliance { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static BodyParticipationTotals Map(
            IEnumerable<BodyParticipation> results)
        {
            var bodyParticipations =
                BodyParticipation
                    .Map(results)
                    .ToList();

            var total = new BodyParticipationTotals
            {
                MaleCount = bodyParticipations.Sum(x => x.MaleCount),
                FemaleCount = bodyParticipations.Sum(x => x.FemaleCount),
                UnknownCount = bodyParticipations.Sum(x => x.UnknownCount),
                AssignedCount = bodyParticipations.Sum(x => x.AssignedCount),
                UnassignedCount = bodyParticipations.Sum(x => x.UnassignedCount),
                TotalCount = bodyParticipations.Sum(x => x.TotalCount),
                Compliance = BodyParticipationCompliance.Unknown,
            };

            if (total.AssignedCount <= 0)
                return total;

            total.Compliance = bodyParticipations.All(participation =>
                participation.TotalCompliance == BodyParticipationCompliance.Compliant)
                ? BodyParticipationCompliance.Compliant
                : BodyParticipationCompliance.NonCompliant;

            total.MalePercentage = ParticipationCalculator.CalculatePercentage(total.MaleCount, total.AssignedCount);
            total.FemalePercentage = ParticipationCalculator.CalculatePercentage(total.FemaleCount, total.AssignedCount);
            total.UnknownPercentage = ParticipationCalculator.CalculatePercentage(total.UnknownCount, total.AssignedCount);

            return total;
        }

    }
}
