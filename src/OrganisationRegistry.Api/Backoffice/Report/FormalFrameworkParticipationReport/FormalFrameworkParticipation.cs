namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkParticipationReport
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using OrganisationRegistry.Person;
    using OrganisationRegistry.SqlServer.Infrastructure;
    using SqlServer.Reporting;

    public class FormalFrameworkParticipation
    {
        [ExcludeFromCsv] public Guid OrganisationId { get; set; }
        [DisplayName("Entiteit")] public string? OrganisationName { get; set; }

        [ExcludeFromCsv] public Guid BodyId { get; set; }
        [DisplayName("Orgaan")] public string? BodyName { get; set; }

        [ExcludeFromCsv] public bool? IsEffective { get; set; }
        [DisplayName("Is Effectief")] public string? IsEffectiveTranslation { get; set; }

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
                .AsQueryable()
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
                    (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

            var activeBodiesById = activeBodiesQuery
                .ToDictionary(
                    x => x.BodyId,
                    x => x);

            var activeBodyIds = activeBodiesById.Keys;

            var activeSeatsPerType = activeBodiesQuery
                .SelectMany(body => body.PostsPerType)
                .Where(post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today));

            var bodySeatGenderRatioPostsPerTypeItems = activeSeatsPerType.ToList();

            var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems
                .Select(item => item.BodySeatId)
                .ToList();

            var activeSeatsPerBodyAndIsEffective = bodySeatGenderRatioPostsPerTypeItems
                .ToList()
                .GroupBy(mandate => new Tuple<Guid, bool>(mandate.BodyId, mandate.BodySeatTypeIsEffective))
                .ToDictionary(
                    x => x.Key,
                    x => x);

            var activeMandates = context.BodySeatGenderRatioBodyMandateList
                .AsAsyncQueryable()
                .Where(mandate => activeBodyIds.Contains(mandate.BodyId))
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                .ToList();

            var activeMandateIds = activeMandates
                .Select(item => item.BodyMandateId)
                .ToList();

            var activeAssignmentsPerIsEffective =
                context.BodySeatGenderRatioBodyMandateList
                    .Include(mandate => mandate.Assignments)
                    .AsAsyncQueryable()
                    .Where(mandate => activeBodyIds.Contains(mandate.BodyId))
                    .Where(mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                    .Where(mandate => activeMandateIds.Contains(mandate.BodyMandateId))
                    .ToList()
                    .GroupBy(mandate => new Tuple<Guid, bool>(mandate.BodyId, mandate.BodySeatTypeIsEffective))
                    .ToDictionary(
                        x => x.Key,
                        x => x.SelectMany(y => y.Assignments));

            var groupedResults = activeSeatsPerBodyAndIsEffective
                .Select(seatPer =>
                {
                    var totalCount = seatPer.Value.Count();
                    var activeAssignments =
                        activeAssignmentsPerIsEffective.ContainsKey(seatPer.Key)
                            ? activeAssignmentsPerIsEffective[seatPer.Key].ToList()
                            : new List<BodySeatGenderRatioAssignmentItem>();

                    var assignedCount = activeAssignments.Count;
                    var body = activeBodiesById[seatPer.Key.Item1];
                    return new FormalFrameworkParticipation
                    {
                        BodyId = seatPer.Key.Item1,
                        BodyName = body.BodyName,
                        OrganisationId = body.OrganisationId ?? Guid.Empty,
                        OrganisationName = body.OrganisationName,

                        IsEffective = seatPer.Key.Item2,
                        IsEffectiveTranslation = seatPer.Key.Item2 ? "Effectief" : "Niet effectief",

                        MaleCount = activeAssignments.Count(x => x.Sex == Sex.Male),
                        FemaleCount = activeAssignments.Count(x => x.Sex == Sex.Female),
                        UnknownCount = activeAssignments.Count(x => !x.Sex.HasValue),

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
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
                case "bodyname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation)
                        : results.OrderByDescending(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);
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
                case "iseffectivetranslation":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.IsEffectiveTranslation)
                        : results.OrderByDescending(x => x.IsEffectiveTranslation);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);
            }
        }
    }
}
