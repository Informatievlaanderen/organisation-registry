namespace OrganisationRegistry.Api.Report.Responses
{
    using Infrastructure;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Linq;
    using Infrastructure.Search.Filtering;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Reporting;
    using OrganisationRegistry.Person;

    public class BodyParticipation
    {
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

        ///  <summary>
        ///
        ///  </summary>
        ///  <param name="context"></param>
        ///  <param name="bodyId"></param>
        /// <param name="filteringHeader"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipation> Search(
            OrganisationRegistryContext context,
            Guid bodyId,
            FilteringHeader<BodyParticipationFilter> filteringHeader)
        {
            // No checkboxes are enabled
            if (!filteringHeader.Filter.EntitledToVote && !filteringHeader.Filter.NotEntitledToVote)
                return new List<BodyParticipation>();

            var bodySeatGenderRatioBodyItems = context.BodySeatGenderRatioBodyList
                .Include(item => item.LifecyclePhaseValidities)
                .Include(item => item.PostsPerType)
                .Where(body => body.BodyId == bodyId)
                .ToList();

            var seatGenderRatioBodyItems = bodySeatGenderRatioBodyItems
                .Where(body => body.LifecyclePhaseValidities.Any(y =>
                    y.RepresentsActivePhase &&
                    (!y.ValidFrom.HasValue || y.ValidFrom <= DateTime.Today) &&
                    (!y.ValidTo.HasValue || y.ValidTo >= DateTime.Today)));

            var postsPerTypeQuery =
                seatGenderRatioBodyItems
                    .SelectMany(item => item.PostsPerType)
                    .Where(post =>
                        (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= DateTime.Today) &&
                        (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= DateTime.Today))/*
                    .Where(post => post.EntitledToVote)*/;

            // One of the checkboxes is checked
            if (filteringHeader.Filter.EntitledToVote ^ filteringHeader.Filter.NotEntitledToVote)
                if (filteringHeader.Filter.EntitledToVote)
                    postsPerTypeQuery = postsPerTypeQuery.Where(x => x.EntitledToVote);
                else if (filteringHeader.Filter.NotEntitledToVote)
                    postsPerTypeQuery = postsPerTypeQuery.Where(x => !x.EntitledToVote);

            var bodySeatGenderRatioPostsPerTypeItems = postsPerTypeQuery.ToList();

            var activePostsPerType =
                bodySeatGenderRatioPostsPerTypeItems
                    .GroupBy(x => new
                    {
                        x.BodyId,
                        x.Body.BodyName,
                        x.BodySeatTypeId,
                        x.BodySeatTypeName
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
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                    .ToList();

            var activeAssignments =
                activeMandates
                    .GroupBy(mandate => new
                    {
                        mandate.BodySeatTypeId
                    })
                    .ToDictionary(
                        x => x.Key.BodySeatTypeId,
                        x => x
                            .SelectMany(mandate => mandate.Assignments)
                            .Where(assignment =>
                                (!assignment.AssignmentValidFrom.HasValue || assignment.AssignmentValidFrom <= DateTime.Today) &&
                                (!assignment.AssignmentValidTo.HasValue || assignment.AssignmentValidTo >= DateTime.Today))
                            .ToList());

            var groupedResults = activePostsPerType
                .Select(g =>
                {
                    var bodySeatTypeId = g.Key.BodySeatTypeId;
                    var assignmentsPerType =
                        activeAssignments.ContainsKey(bodySeatTypeId) ?
                        activeAssignments[bodySeatTypeId] :
                        new List<BodySeatGenderRatioAssignmentItem>();

                    var totalCount = g.Count();
                    var assignedCount = assignmentsPerType.Count;
                    return new BodyParticipation
                    {
                        BodyId = g.Key.BodyId,
                        BodyName = g.Key.BodyName,
                        BodySeatTypeId = bodySeatTypeId,
                        BodySeatTypeName = g.Key.BodySeatTypeName,

                        MaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Male),
                        FemaleCount = assignmentsPerType.Count(x => x.Sex == Sex.Female),
                        UnknownCount = assignmentsPerType.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount
                    };
                })
                .ToList();

            return groupedResults;
        }

        /// <summary>
        /// </summary>
        /// <param name="results"></param>
        /// <returns></returns>
        public static IEnumerable<BodyParticipation> Map(
            IEnumerable<BodyParticipation> results)
        {
            var participations = new List<BodyParticipation>();

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

        ///  <summary>
        ///  </summary>
        ///  <param name="results"></param>
        ///  <param name="sortingHeader"></param>
        /// <returns></returns>
        public static IOrderedEnumerable<BodyParticipation> Sort(
            IEnumerable<BodyParticipation> results,
            SortingHeader sortingHeader)
        {
            if (!sortingHeader.ShouldSort)
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodySeatTypeName);

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
                case "bodyseattypename":
                    return sortingHeader.SortOrder == SortOrder.Ascending
                        ? results.OrderBy(x => x.BodySeatTypeName)
                        : results.OrderByDescending(x => x.BodySeatTypeName);
                default:
                    return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodySeatTypeName);
            }
        }
    }

    public class BodyParticipationFilter
    {
        public bool EntitledToVote { get; set; }

        public bool NotEntitledToVote { get; set; }
    }
}
