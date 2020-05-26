namespace OrganisationRegistry.Api.Report.Responses
{
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using System.Threading.Tasks;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Reporting;
    using OrganisationRegistry.Person;

    public class ParticipationSummary
    {
        [ExcludeFromCsv] public Guid OrganisationId { get; set; }
        [DisplayName("Entiteit")] public string OrganisationName { get; set; }

        [ExcludeFromCsv] public Guid BodyId { get; set; }
        [DisplayName("Orgaan")] public string BodyName { get; set; }

        [ExcludeFromCsv] public bool? IsEffective { get; set; }
        [DisplayName("Is Effectief")] public string IsEffectiveTranslation { get; set; }

        [DisplayName("Percentage Man")] public decimal MalePercentage { get; set; }
        [DisplayName("Percentage Vrouw")] public decimal FemalePercentage { get; set; }
        [DisplayName("Percentage Onbekend")] public decimal UnknownPercentage { get; set; }

        [DisplayName("Aantal Man")] public int MaleCount { get; set; }
        [DisplayName("Aantal Vrouw")] public int FemaleCount { get; set; }
        [DisplayName("Aantal Onbekend")] public int UnknownCount { get; set; }

        [DisplayName("Totaal Aantal")] public int TotalCount { get; set; }

        [ExcludeFromCsv] public int AssignedCount { get; set; }
        [ExcludeFromCsv] public int UnassignedCount { get; set; }

        [DisplayName("Totaal ok")] public bool IsTotalCompliant { get; set; }

        [DisplayName("Vrouw ok")] public bool IsFemaleCountCompliant { get; set; }

        [DisplayName("Man ok")] public bool IsMaleCountCompliant { get; set; }

        /// <summary>
        ///
        /// </summary>
        /// <param name="context"></param>
        /// <param name="today"></param>
        /// <returns></returns>
        public static async Task<IEnumerable<ParticipationSummary>> Search(
            OrganisationRegistryContext context,
            DateTime today)
        {
            //get organisation ids for organisation classification
            var organisationIds = context.BodySeatGenderRatioOrganisationClassificationList
                .AsAsyncQueryable()
                .Where(
                    body =>
                        (!body.ClassificationValidFrom.HasValue || body.ClassificationValidFrom.Value <= today) &&
                        (!body.ClassificationValidTo.HasValue || body.ClassificationValidTo.Value >= today))
                .Select(x => x.OrganisationId)
                .ToList();

            if (!organisationIds.Any())
                return new List<ParticipationSummary>();

            //get body ids
            var bodies = context.BodySeatGenderRatioOrganisationPerBodyList
                .AsQueryable()
                .Where(body => organisationIds.Contains(body.OrganisationId))
                .ToList();
            if (!bodies.Any())
                return new List<ParticipationSummary>();

            var results = new List<BodyParticipation>();

            var nonFilteredBodyIds = bodies.Select(x => x.BodyId);
            var filteredBodyIds = context
                .BodyFormalFrameworkList
                .AsQueryable()
                .Where(x => nonFilteredBodyIds.Contains(x.BodyId) &&
                            (!x.ValidFrom.HasValue || x.ValidFrom.Value <= today) &&
                            (!x.ValidTo.HasValue || x.ValidTo.Value >= today))
                .Select(x => x.BodyId);

            foreach (var bodyId in filteredBodyIds)
            {
                var bodySeatGenderRatioBodyItems = context.BodySeatGenderRatioBodyList
                    .Include(item => item.LifecyclePhaseValidities)
                    .Include(item => item.PostsPerType)
                    .Where(body => body.BodyId == bodyId)
                    .ToList();

                var seatGenderRatioBodyItems = bodySeatGenderRatioBodyItems
                    .Where(body => body.LifecyclePhaseValidities.Any(y =>
                        y.RepresentsActivePhase &&
                        (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                        (!y.ValidTo.HasValue || y.ValidFrom >= DateTime.Today)));

                var activePostsPerTypeQuery =
                    seatGenderRatioBodyItems
                        .SelectMany(item => item.PostsPerType)
                        .Where(post =>
                            (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                            (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today));

                var bodySeatGenderRatioPostsPerTypeItems = activePostsPerTypeQuery.ToList();

                var activePostsPerType =
                    bodySeatGenderRatioPostsPerTypeItems
                        .GroupBy(x => new
                        {
                            x.BodyId,
                            x.Body.BodyName,
                            x.BodySeatTypeIsEffective,
                        })
                        .ToList();

                var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems.Select(item => item.BodySeatId);

                var activeMandates =
                    await context.BodySeatGenderRatioBodyMandateList
                        .Include(mandate => mandate.Assignments)
                        .Where(mandate => mandate.BodyId == bodyId)
                        .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
                        .Where(mandate =>
                            (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                            (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                        .ToListAsync();  // need to include tolist here for now, otherwise causes
                                    // System.InvalidOperationException: 'Client projection contains reference to constant expression of type: Microsoft.EntityFrameworkCore.Metadata.IPropertyBase. This could potentially cause memory leak.'
                                    // See also:
                                    // https://github.com/StefH/System.Linq.Dynamic.Core/issues/317,
                                    // https://github.com/dotnet/efcore/issues/17623,
                                    // https://github.com/dotnet/efcore/issues/18051

                var activeAssignmentsPerBodySeatTypeId =
                    activeMandates
                        .GroupBy(mandate => mandate.BodySeatTypeIsEffective)
                        .ToDictionary(
                            x => x.Key,
                            x => x
                                .SelectMany(mandate => mandate.Assignments)
                                .Where(assignment =>
                                    (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                                    (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today))
                                .ToList());

                results.AddRange(activePostsPerType
                    .Select(g =>
                    {
                        var isEffective = g.Key.BodySeatTypeIsEffective;
                        var assignmentsPerType =
                            activeAssignmentsPerBodySeatTypeId.ContainsKey(isEffective) ?
                                activeAssignmentsPerBodySeatTypeId[isEffective] :
                                new List<BodySeatGenderRatioAssignmentItem>();

                        var totalCount = g.Count();
                        var assignedCount = assignmentsPerType.Count;

                        return new BodyParticipation
                        {
                            BodyId = g.Key.BodyId,
                            BodyName = g.Key.BodyName,
                            IsEffective = isEffective,
                            IsEffectiveTranslation = isEffective ? "Effectief" : "Niet effectief",

                            MaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Male),
                            FemaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Female),
                            UnknownCount = assignmentsPerType.Count(x => !x.Sex.HasValue),

                            AssignedCount = assignedCount,
                            UnassignedCount = totalCount - assignedCount,

                            TotalCount = totalCount
                        };
                    }));
            }

            var groupedResults = results.GroupBy(
                key => new
                {
                    key.BodyId,
                    key.BodyName
                },
                element => new
                {
                    element.MaleCount,
                    element.FemaleCount,
                    element.UnknownCount,
                    element.AssignedCount,
                    element.UnassignedCount,
                    element.TotalCount
                });

            return groupedResults.Select(x =>
            {
                var organisation = bodies.Single(body => body.BodyId == x.Key.BodyId);

                return new ParticipationSummary
                {
                    OrganisationId = organisation.OrganisationId,
                    OrganisationName = organisation.OrganisationName,

                    BodyId = x.Key.BodyId,
                    BodyName = x.Key.BodyName,

                    MaleCount = x.Sum(g => g.MaleCount),
                    FemaleCount = x.Sum(g => g.FemaleCount),
                    UnknownCount = x.Sum(g => g.UnknownCount),

                    AssignedCount = x.Sum(g => g.AssignedCount),
                    UnassignedCount = x.Sum(g => g.UnassignedCount),

                    TotalCount = x.Sum(g => g.TotalCount)
                };
            });
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<ParticipationSummary> Map(
            IEnumerable<ParticipationSummary> results)
        {
            var participations = new List<ParticipationSummary>();
            var lower = Math.Floor((1m / 3) * 100) / 100;
            var upper = Math.Ceiling((2m / 3) * 100) / 100;

            foreach (var result in results)
            {
                if (result.AssignedCount > 0)
                {
                    result.MalePercentage = Math.Round((decimal)result.MaleCount / result.AssignedCount, 2);
                    result.FemalePercentage = Math.Round((decimal)result.FemaleCount / result.AssignedCount, 2);
                    result.UnknownPercentage = Math.Round((decimal)result.UnknownCount / result.AssignedCount, 2);
                    result.IsMaleCountCompliant = result.MalePercentage >= lower && result.MalePercentage <= upper;
                    result.IsFemaleCountCompliant = result.FemalePercentage >= lower && result.FemalePercentage <= upper;
                    result.IsTotalCompliant = result.IsMaleCountCompliant && result.IsFemaleCountCompliant;
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
        public static IOrderedEnumerable<ParticipationSummary> Sort(
            IEnumerable<ParticipationSummary> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodyName);

            switch (sortingHeader.SortBy.ToLowerInvariant())
            {
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
                case "bodyname":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodyName)
                        : results.OrderByDescending(x => x.BodyName);
                case "istotalcompliant":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.IsTotalCompliant)
                        : results.OrderByDescending(x => x.IsTotalCompliant);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodyName);
            }
        }
    }
}
