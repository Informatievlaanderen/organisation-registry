namespace OrganisationRegistry.Api.Backoffice.Report.Participation;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Person;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.Reporting;

public class BodyParticipation
{
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

    [DisplayName("Is Mep-conform")] public BodyParticipationCompliance TotalCompliance { get; set; }

    [DisplayName("Is Mep-conform Voor Vrouwen")]
    public BodyParticipationCompliance FemaleCompliance { get; set; }

    [DisplayName("Is Mep-conform Voor Mannen")]
    public BodyParticipationCompliance MaleCompliance { get; set; }

    ///  <summary>
    ///
    ///  </summary>
    ///  <param name="context"></param>
    ///  <param name="bodyId"></param>
    ///  <param name="filteringHeader"></param>
    ///  <param name="today"></param>
    ///  <returns></returns>
    public static IEnumerable<BodyParticipation> Search(
        OrganisationRegistryContext context,
        Guid bodyId,
        FilteringHeader<BodyParticipationFilter> filteringHeader,
        DateTime today)
    {
        if (filteringHeader.Filter is not { } filter)
            throw new NullReferenceException("filteringHeader.Filter should not be null");

        // No checkboxes are enabled
        if (!filter.EntitledToVote && !filter.NotEntitledToVote)
            return new List<BodyParticipation>();

        var body = context.BodySeatGenderRatioBodyList
            .Include(item => item.PostsPerType)
            .Single(item => item.BodyId == bodyId);

        var activeSeatsPerType = body
            .PostsPerType
            .Where(
                post =>
                    (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= today) &&
                    (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= today));

        // One of the checkboxes is checked
        if (filter.EntitledToVote ^ filter.NotEntitledToVote)
            if (filter.EntitledToVote)
                activeSeatsPerType = activeSeatsPerType.Where(x => x.EntitledToVote);
            else if (filter.NotEntitledToVote)
                activeSeatsPerType = activeSeatsPerType.Where(x => !x.EntitledToVote);

        var bodySeatGenderRatioPostsPerTypeItems = activeSeatsPerType.ToList();

        var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems
            .Select(item => item.BodySeatId)
            .ToList();

        var activeSeatsPerIsEffective = bodySeatGenderRatioPostsPerTypeItems
            .ToList()
            .GroupBy(mandate => mandate.BodySeatTypeIsEffective)
            .ToDictionary(
                x => x.Key,
                x => x);

        var activeMandates = context.BodySeatGenderRatioBodyMandateList
            .AsAsyncQueryable()
            .Where(mandate => mandate.BodyId == bodyId)
            .Where(
                mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= today))
            .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
            .ToList();

        var activeMandateIds = activeMandates
            .Select(item => item.BodyMandateId)
            .ToList();

        var bodySeatGenderRatioBodyMandateItems = context.BodySeatGenderRatioBodyMandateList
            .Include(mandate => mandate.Assignments)
            .AsAsyncQueryable();

        var activeAssignmentsPerIsEffective =
            bodySeatGenderRatioBodyMandateItems
                .Where(mandate => mandate.BodyId == bodyId)
                .Where(
                    mandate =>
                        (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= today) &&
                        (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= today))
                .Where(mandate => activeMandateIds.Contains(mandate.BodyMandateId))
                .ToList()
                .GroupBy(mandate => mandate.BodySeatTypeIsEffective)
                .ToDictionary(
                    x => x.Key,
                    x => x
                        .SelectMany(y => y.Assignments)
                        .Where(
                            a =>
                                (!a.AssignmentValidFrom.HasValue || a.AssignmentValidFrom <= today) &&
                                (!a.AssignmentValidTo.HasValue || a.AssignmentValidTo >= today)));

        var groupedResults = activeSeatsPerIsEffective
            .Select(
                seatPerIsEffective =>
                {
                    var totalCount = seatPerIsEffective.Value.Count();
                    var activeAssignments = GetKeyOrEmpty(activeAssignmentsPerIsEffective, seatPerIsEffective.Key).ToList();
                    var assignedCount = activeAssignments.Count;
                    return new BodyParticipation
                    {
                        BodyId = bodyId,
                        BodyName = body.BodyName,
                        IsEffective = seatPerIsEffective.Key,
                        IsEffectiveTranslation = seatPerIsEffective.Key ? "Effectief" : "Plaatsvervangend",

                        MaleCount = activeAssignments.Count(x => x.Sex == Sex.Male),
                        FemaleCount = activeAssignments.Count(x => x.Sex == Sex.Female),
                        UnknownCount = activeAssignments.Count(x => !x.Sex.HasValue),

                        AssignedCount = assignedCount,
                        UnassignedCount = totalCount - assignedCount,

                        TotalCount = totalCount,
                    };
                });

        return groupedResults;
    }

    private static IEnumerable<BodySeatGenderRatioAssignmentItem> GetKeyOrEmpty(
        IReadOnlyDictionary<bool, IEnumerable<BodySeatGenderRatioAssignmentItem>> activeAssignmentsPerIsEffective,
        bool key)
        => activeAssignmentsPerIsEffective.ContainsKey(key)
            ? activeAssignmentsPerIsEffective[key]
            : Array.Empty<BodySeatGenderRatioAssignmentItem>();

    /// <summary>
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public static IEnumerable<BodyParticipation> Map(
        IEnumerable<BodyParticipation> results)
        => results.Select(Map).ToList();

    public static BodyParticipation Map(BodyParticipation bodyParticipation)
    {
        if (bodyParticipation.AssignedCount <= 0) return bodyParticipation;
        bodyParticipation.MalePercentage = ParticipationCalculator.CalculatePercentage(bodyParticipation.MaleCount, bodyParticipation.AssignedCount);
        bodyParticipation.FemalePercentage = ParticipationCalculator.CalculatePercentage(bodyParticipation.FemaleCount, bodyParticipation.AssignedCount);
        bodyParticipation.UnknownPercentage = ParticipationCalculator.CalculatePercentage(bodyParticipation.UnknownCount, bodyParticipation.AssignedCount);

        bodyParticipation.MaleCompliance = ParticipationCalculator.CalculateCompliance(bodyParticipation.TotalCount, bodyParticipation.MalePercentage);
        bodyParticipation.FemaleCompliance = ParticipationCalculator.CalculateCompliance(bodyParticipation.TotalCount, bodyParticipation.FemalePercentage);

        bodyParticipation.TotalCompliance =
            bodyParticipation.TotalCount <= 1
                ? BodyParticipationCompliance.Unknown
                : bodyParticipation.FemaleCompliance == BodyParticipationCompliance.Compliant &&
                  bodyParticipation.MaleCompliance == BodyParticipationCompliance.Compliant
                    ? BodyParticipationCompliance.Compliant
                    : BodyParticipationCompliance.NonCompliant;

        return bodyParticipation;
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
            return results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation);

        return sortingHeader.SortBy.ToLowerInvariant() switch
        {
            "malepercentage" => OrderBy(results, x => x.MalePercentage, sortingHeader.SortOrder),
            "femalepercentage" => OrderBy(results, x => x.FemalePercentage, sortingHeader.SortOrder),
            "unknownpercentage" => OrderBy(results, x => x.UnknownPercentage, sortingHeader.SortOrder),
            "iseffectivetranslation" => OrderBy(results, x => x.IsEffectiveTranslation, sortingHeader.SortOrder),
            _ => results.OrderBy(x => x.BodyName).ThenBy(x => x.IsEffectiveTranslation)
        };
    }

    private static IOrderedEnumerable<BodyParticipation> OrderBy<TKey>(IEnumerable<BodyParticipation> results, Func<BodyParticipation, TKey> keySelector, SortOrder sortOrder)
        => sortOrder == SortOrder.Ascending ? results.OrderBy(keySelector) : results.OrderByDescending(keySelector);
}

public class BodyParticipationFilter
{
    public bool EntitledToVote { get; set; }

    public bool NotEntitledToVote { get; set; }
}
