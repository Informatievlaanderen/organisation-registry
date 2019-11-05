namespace OrganisationRegistry.Api.Report.Responses
{
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Reporting;
    using OrganisationRegistry.Person;

    public class FormalFrameworkParticipation
    {
        [ExcludeFromCsv] public Guid OrganisationId { get; set; }
        [DisplayName("Entiteit")] public string OrganisationName { get; set; }

        [ExcludeFromCsv] public Guid BodyId { get; set; }
        [DisplayName("Orgaan")] public string BodyName { get; set; }

        [ExcludeFromCsv] public Guid BodySeatTypeId { get; set; }
        [DisplayName("Post")] public string BodySeatTypeName { get; set; }

        [DisplayName("Percentage Man")] public decimal MalePercentage { get; set; }
        [DisplayName("Percentage Vrouw")] public decimal FemalePercentage { get; set; }
        [DisplayName("Percentage Onbekend")] public decimal UnknownPercentage { get; set; }

        [DisplayName("Aantal Man")] public int MaleCount { get; set; }
        [DisplayName("Aantal Vrouw")] public int FemaleCount { get; set; }
        [DisplayName("Aantal Onbekend")] public int UnknownCount { get; set; }

        [DisplayName("Totaal Aantal")] public int TotalCount { get; set; }

        [ExcludeFromCsv] public int AssignedCount { get; set; }
        [ExcludeFromCsv] public int UnassignedCount { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="formalFrameworkId"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkParticipation> Search(
            OrganisationRegistryContext context,
            Guid formalFrameworkId)
        {
            var bodiesForFormalFramework = context
                .BodyFormalFrameworkList
                .Where(x => x.FormalFrameworkId == formalFrameworkId &&
                            (!x.ValidFrom.HasValue || x.ValidFrom.Value <= DateTime.Now) &&
                            (!x.ValidTo.HasValue || x.ValidTo.Value >= DateTime.Now))
                .Select(x => x.BodyId);

            var activeBodiesQuery = context.BodySeatGenderRatioBodyList
                .Include(item => item.LifecyclePhaseValidities)
                .Include(item => item.PostsPerType)
                .ThenInclude(item => item.Body)
                .Where(body => bodiesForFormalFramework.Contains(body.BodyId))
                .Where(body => body.OrganisationIsActive)
                .Where(body => body.LifecyclePhaseValidities.Any(y =>
                    y.RepresentsActivePhase &&
                    (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                    (!y.ValidTo.HasValue || y.ValidFrom >= DateTime.Today))); // TODO: do we really need all these wheres that check validity?

            var activePostsQueries =
                context.BodySeatGenderRatioPostsPerTypeList
                    .Include(post => post.Body)
                    .Where(post => activeBodiesQuery.Select(body => body.BodyId).Contains(post.BodyId))
                    .Where(post =>
                        (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                        (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today))
                    .Where(post => post.EntitledToVote);

            var bodySeatGenderRatioPostsPerTypeItems = activePostsQueries.ToList();

            var activePostsPerType = bodySeatGenderRatioPostsPerTypeItems
                .GroupBy(x => new
                {
                    x.BodyId,
                    x.Body.BodyName,
                    x.Body.OrganisationId,
                    x.Body.OrganisationName,
                    x.BodySeatTypeId,
                    x.BodySeatTypeName
                })
                .ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems.Select(item => item.BodySeatId);

            var activeMandates =
                context.BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .Where(mandate => bodiesForFormalFramework.Contains(mandate.BodyId))
                    .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                    .Where(mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                    .ToList();

            var activeAssignments =
                activeMandates
                    .GroupBy(mandate => new
                    {
                        mandate.BodyId,
                        mandate.BodySeatTypeId
                    })
                    .ToDictionary(
                        mandates => CreateCombinedKey(mandates.Key.BodyId, mandates.Key.BodySeatTypeId),
                        mandates => mandates
                            .SelectMany(mandate => mandate.Assignments)
                            .Where(assignment =>
                                (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                                (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today))
                            .ToList());

            var groupedResults = activePostsPerType
                .Select(g =>
                {
                    var bodySeatTypeId = CreateCombinedKey(g.Key.BodyId, g.Key.BodySeatTypeId);
                    var assignmentsPerType =
                        activeAssignments.ContainsKey(bodySeatTypeId) ?
                            activeAssignments[bodySeatTypeId] :
                            new List<BodySeatGenderRatioAssignmentItem>();

                    var totalCount = g.Count();
                    var assignedCount = assignmentsPerType.Count();
                    return new FormalFrameworkParticipation
                    {
                        BodyId = g.Key.BodyId,
                        BodyName = g.Key.BodyName,
                        OrganisationId = g.Key.OrganisationId ?? Guid.Empty,
                        OrganisationName = g.Key.OrganisationName,
                        BodySeatTypeId = g.Key.BodySeatTypeId,
                        BodySeatTypeName = g.Key.BodySeatTypeName,

                        MaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Male),
                        FemaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Female),
                        UnknownCount = assignmentsPerType.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                });

            return groupedResults;
        }

        private static Tuple<Guid, Guid> CreateCombinedKey(Guid bodyId, Guid bodySeatTypeId)
        {
            return new Tuple<Guid, Guid>(bodyId, bodySeatTypeId);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<FormalFrameworkParticipation> Map(
            IEnumerable<FormalFrameworkParticipation> results)
        {
            var participations = new List<FormalFrameworkParticipation>();

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

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<FormalFrameworkParticipation> Sort(
            IEnumerable<FormalFrameworkParticipation> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodySeatTypeName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "bodyname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyName).ThenBy(x => x.BodySeatTypeName)
                        : results.OrderByDescending(x => x.BodyName).ThenBy(x => x.BodySeatTypeName);
                case "malepercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.MalePercentage)
                        : results.OrderByDescending(x => x.MalePercentage);
                case "femalepercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.FemalePercentage)
                        : results.OrderByDescending(x => x.FemalePercentage);
                case "unknownpercentage":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.UnknownPercentage)
                        : results.OrderByDescending(x => x.UnknownPercentage);
                case "bodyseattypename":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodySeatTypeName)
                        : results.OrderByDescending(x => x.BodySeatTypeName);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodySeatTypeName);
            }
        }
    }
}
