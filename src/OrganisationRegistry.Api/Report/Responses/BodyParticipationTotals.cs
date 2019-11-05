namespace OrganisationRegistry.Api.Report.Responses
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search.Filtering;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Person;

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

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="context"></param>
        ///  <param name="bodyId"></param>
        /// <param name="filteringHeader"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipationTotals> Search(
            OrganisationRegistryContext context,
            Guid bodyId,
            FilteringHeader<BodyParticipationFilter> filteringHeader)
        {
            // No checkboxes are enabled
            if (!filteringHeader.Filter.EntitledToVote && !filteringHeader.Filter.NotEntitledToVote)
                return new List<BodyParticipationTotals>();

            var bodySeatGenderRatioBodyItem = context.BodySeatGenderRatioBodyList
                .Include(body => body.LifecyclePhaseValidities)
                .Include(body => body.PostsPerType)
                .Where(body => body.LifecyclePhaseValidities.Any(y =>
                    y.RepresentsActivePhase &&
                    (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                    (!y.ValidTo.HasValue || y.ValidFrom >= DateTime.Today)))
                .SingleOrDefault(post => post.BodyId == bodyId);
            //  .Where(post => post.OrganisationIsActive) TODO: Discuss with Thomas

           if (bodySeatGenderRatioBodyItem == null)
               return new List<BodyParticipationTotals>();

            var activePostsPerTypeQuery = bodySeatGenderRatioBodyItem
                .PostsPerType
                .Where(post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today));

            // One of the checkboxes is checked
            if (filteringHeader.Filter.EntitledToVote ^ filteringHeader.Filter.NotEntitledToVote)
                if (filteringHeader.Filter.EntitledToVote)
                    activePostsPerTypeQuery = activePostsPerTypeQuery.Where(x => x.EntitledToVote);
                else if (filteringHeader.Filter.NotEntitledToVote)
                    activePostsPerTypeQuery = activePostsPerTypeQuery.Where(x => !x.EntitledToVote);

            var bodySeatGenderRatioPostsPerTypeItems = activePostsPerTypeQuery.ToList();

            var activePostsPerType =
                bodySeatGenderRatioPostsPerTypeItems
                    .GroupBy(x => new
                    {
                        x.BodyId
                    })
                    .ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems.Select(item => item.BodySeatId);

            var activeMandates =
                context.BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .Where(mandate => mandate.BodyId == bodyId)
                    .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                    .Where(mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today));

            var activeAssignments =
                activeMandates
                    .SelectMany(mandate => mandate.Assignments)
                    .Where(assignment =>
                        (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                        (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today))
                    .ToList();

            var bodySeatGenderRatioTotals = activePostsPerType
                .Select(g =>
                {
                    var totalCount = g.Count();
                    var assignedCount = activeAssignments.Count;
                    return new BodyParticipationTotals
                    {
                        MaleCount = activeAssignments.Count(x => x.Sex == Sex.Male),
                        FemaleCount = activeAssignments.Count(x => x.Sex == Sex.Female),
                        UnknownCount = activeAssignments.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                });

            return bodySeatGenderRatioTotals;
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipationTotals> Map(
            IEnumerable<BodyParticipationTotals> results)
        {
            var participations = new List<BodyParticipationTotals>();

            foreach (var result in results)
            {
                if (result.AssignedCount > 0)
                {
                    result.MalePercentage = Math.Round((decimal)result.MaleCount / result.AssignedCount, 2);
                    result.FemalePercentage = Math.Round((decimal)result.FemaleCount / result.AssignedCount, 2);
                    result.UnknownPercentage = Math.Round((decimal)result.UnknownCount / result.AssignedCount, 2);
                }

                participations.Add(result);
            }

            return participations;
        }
    }
}
