namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkParticipationReport;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Person;
using OrganisationRegistry.SqlServer.Infrastructure;
using SqlServer.Reporting;
using Participation;

public class ParticipationSummary
{
    [ExcludeFromCsv] public Guid OrganisationId { get; set; }
    [DisplayName("Entiteit")] public string? OrganisationName { get; set; }

    [ExcludeFromCsv] public Guid BodyId { get; set; }
    [DisplayName("Orgaan")] public string? BodyName { get; set; }

    [DisplayName("Percentage Man effectief")] public decimal EffectiveMalePercentage { get; set; }
    [DisplayName("Percentage Vrouw effectief")] public decimal EffectiveFemalePercentage { get; set; }
    [DisplayName("Percentage Onbekend effectief")] public decimal EffectiveUnknownPercentage { get; set; }
    [DisplayName("Percentage Man plaatsvervangend")] public decimal SubsidiaryMalePercentage { get; set; }
    [DisplayName("Percentage Vrouw plaatsvervangend")] public decimal SubsidiaryFemalePercentage { get; set; }
    [DisplayName("Percentage Onbekend plaatsvervangend")] public decimal SubsidiaryUnknownPercentage { get; set; }
    [DisplayName("Aantal Man effectief")] public int EffectiveMaleCount { get; set; }
    [DisplayName("Aantal Vrouw effectief")] public int EffectiveFemaleCount { get; set; }
    [DisplayName("Aantal Onbekend effectief")] public int EffectiveUnknownCount { get; set; }
    [DisplayName("Totaal Aantal effectief")] public int EffectiveTotalCount { get; set; }
    [DisplayName("Aantal Man plaatsvervangend")] public int SubsidiaryMaleCount { get; set; }
    [DisplayName("Aantal Vrouw plaatsvervangend")] public int SubsidiaryFemaleCount { get; set; }
    [DisplayName("Aantal Onbekend plaatsvervangend")] public int SubsidiaryUnknownCount { get; set; }
    [DisplayName("Totaal Aantal plaatsvervangend")] public int SubsidiaryTotalCount { get; set; }
    [DisplayName("Totaal Aantal")] public int TotalCount { get; set; }

    [ExcludeFromCsv] public int AssignedCount { get; set; }
    [ExcludeFromCsv] public int UnassignedCount { get; set; }

    [DisplayName("Totaal ok")] public bool IsTotalCompliant { get; set; }

    [DisplayName("Vrouw ok")] public bool IsSubsidiaryCompliant { get; set; }

    [DisplayName("Man ok")] public bool IsEffectiveCompliant { get; set; }

    [DisplayName("Beleidsdomein Id")] public Guid? PolicyDomainClassificationId { get; set; }
    [DisplayName("Beleidsdomein Naam")] public string? PolicyDomainClassificationName { get; set; }

    [DisplayName("Verantwoordelijke Minister Id")] public Guid? ResponsibleMinisterClassificationId { get; set; }
    [DisplayName("Verantwoordelijke Minister Naam")] public string? ResponsibleMinisterClassificationName { get; set; }

    /// <summary>
    /// </summary>
    /// <param name="context"></param>
    /// <param name="apiConfiguration"></param>
    /// <param name="today"></param>
    /// <returns></returns>
    public static async Task<IEnumerable<ParticipationSummary>> Search(
        OrganisationRegistryContext context,
        ApiConfigurationSection apiConfiguration,
        DateTime today)
    {
        var bodyIdsForMep = context.BodyFormalFrameworkList
            .AsAsyncQueryable()
            .Where(body => body.FormalFrameworkId == apiConfiguration.Mep_FormalFrameworkId &&
                           (!body.ValidFrom.HasValue || body.ValidFrom <= today) &&
                           (!body.ValidTo.HasValue || body.ValidTo >= today))
            .Select(body => body.BodyId);

        var bodies = await context.BodySeatGenderRatioBodyList
            .Include(item => item.PostsPerType)
            .Include(item => item.LifecyclePhaseValidities)
            .Where(body => body.LifecyclePhaseValidities.Any(y =>
                y.RepresentsActivePhase &&
                (!y.ValidFrom.HasValue || y.ValidFrom <= today) &&
                (!y.ValidTo.HasValue || y.ValidTo >= today)))
            .Where(body => bodyIdsForMep.Contains(body.BodyId))
            .ToListAsync();

        var activeSeatsPerType = bodies
            .SelectMany(x => x.PostsPerType)
            .Where(post =>
                post.EntitledToVote &&
                (!post.BodySeatValidFrom.HasValue || post.BodySeatValidFrom <= today) &&
                (!post.BodySeatValidTo.HasValue || post.BodySeatValidTo >= today));

        var bodySeatGenderRatioPostsPerTypeItems = activeSeatsPerType.ToList();

        var activeSeatIds = bodySeatGenderRatioPostsPerTypeItems
            .Select(item => item.BodySeatId)
            .ToList();

        var activeSeatsPerBodyAndIsEffective = bodySeatGenderRatioPostsPerTypeItems
            .ToList()
            .GroupBy(mandate => new
            {
                mandate.BodyId,
                mandate.BodySeatTypeIsEffective,
            })
            .ToDictionary(
                x => x.Key,
                x => x);

        var activeMandates = await context.BodySeatGenderRatioBodyMandateList
            .AsAsyncQueryable()
            .Where(mandate =>
                (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= today) &&
                (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= today))
            .Where(mandate => activeSeatIds.Contains(mandate.BodySeatId))
            .ToListAsync();

        var activeMandateIds = activeMandates
            .Select(item => item.BodyMandateId)
            .ToList();

        var activeAssignmentsPerIsEffective =
            (await context.BodySeatGenderRatioBodyMandateList
                .Include(mandate => mandate.Assignments)
                .AsAsyncQueryable()
                .Where(mandate =>
                    (!mandate.BodyMandateValidFrom.HasValue || mandate.BodyMandateValidFrom <= DateTime.Today) &&
                    (!mandate.BodyMandateValidTo.HasValue || mandate.BodyMandateValidTo >= DateTime.Today))
                .Where(mandate => activeMandateIds.Contains(mandate.BodyMandateId))
                .ToListAsync())
            .GroupBy(mandate => new
            {
                mandate.BodyId,
                mandate.BodySeatTypeIsEffective,
            })
            .ToDictionary(
                x => x.Key,
                x => x.SelectMany(y => y.Assignments));

        var bodyIdsWithAssignments = activeAssignmentsPerIsEffective.Select(x => x.Key.BodyId);
        var bodiesWithAssignments = bodies.Where(x => bodyIdsWithAssignments.Contains(x.BodyId));
        var policyDomainsPerBody = (await context.BodyBodyClassificationList
                .AsAsyncQueryable()
                .Where(bodyClassification => bodyIdsWithAssignments.Contains(bodyClassification.BodyId))
                .Where(bodyClassification => bodyClassification.BodyClassificationTypeId ==
                                             apiConfiguration.BodyPolicyDomainClassificationTypeId)
                .Where(bodyClassification =>
                    (!bodyClassification.ValidFrom.HasValue || bodyClassification.ValidFrom <= DateTime.Today) &&
                    (!bodyClassification.ValidTo.HasValue || bodyClassification.ValidTo >= DateTime.Today))
                .ToListAsync())
            .GroupBy(bodyClassification => bodyClassification.BodyId)
            .ToDictionary(x => x.Key, x => x);

        var responsibleMinisterPerBody = (await context.BodyBodyClassificationList
                .AsAsyncQueryable()
                .Where(bodyClassification => bodyIdsWithAssignments.Contains(bodyClassification.BodyId))
                .Where(bodyClassification => bodyClassification.BodyClassificationTypeId ==
                                             apiConfiguration.BodyResponsibleMinisterClassificationTypeId)
                .Where(bodyClassification =>
                    (!bodyClassification.ValidFrom.HasValue || bodyClassification.ValidFrom <= DateTime.Today) &&
                    (!bodyClassification.ValidTo.HasValue || bodyClassification.ValidTo >= DateTime.Today))
                .ToListAsync())
            .GroupBy(bodyClassification => bodyClassification.BodyId)
            .ToDictionary(x => x.Key, x => x);

        return bodiesWithAssignments
            .Select(body =>
            {
                var effectiveKey = new { body.BodyId, BodySeatTypeIsEffective = true};
                var subsidiaryKey = new { body.BodyId, BodySeatTypeIsEffective = false};

                var effectiveSeats =
                    activeSeatsPerBodyAndIsEffective.ContainsKey(effectiveKey)
                        ? activeSeatsPerBodyAndIsEffective[effectiveKey].ToList()
                        : new List<BodySeatGenderRatioPostsPerTypeItem>();

                var subsidiarySeats =
                    activeSeatsPerBodyAndIsEffective.ContainsKey(subsidiaryKey)
                        ? activeSeatsPerBodyAndIsEffective[subsidiaryKey].ToList()
                        : new List<BodySeatGenderRatioPostsPerTypeItem>();

                var effectiveAssignments =
                    activeAssignmentsPerIsEffective.ContainsKey(effectiveKey)
                        ? activeAssignmentsPerIsEffective[effectiveKey].ToList()
                        : new List<BodySeatGenderRatioAssignmentItem>();

                var subsidiaryAssignments =
                    activeAssignmentsPerIsEffective.ContainsKey(subsidiaryKey)
                        ? activeAssignmentsPerIsEffective[subsidiaryKey].ToList()
                        : new List<BodySeatGenderRatioAssignmentItem>();

                var effectiveTotalCount = effectiveAssignments.Count;
                var subsidiaryTotalCount = subsidiaryAssignments.Count;
                var assignedCount = effectiveTotalCount + subsidiaryTotalCount;

                var totalCount = effectiveSeats.Count + subsidiarySeats.Count;

                var policyDomain = policyDomainsPerBody.ContainsKey(body.BodyId)
                    ? policyDomainsPerBody[body.BodyId].Single()
                    : null;

                var responsibleMinister = responsibleMinisterPerBody.ContainsKey(body.BodyId)
                    ? responsibleMinisterPerBody[body.BodyId].Single()
                    : null;

                return new ParticipationSummary
                {
                    BodyId = body.BodyId,
                    BodyName = body.BodyName ?? string.Empty,

                    OrganisationId = body.OrganisationId ?? Guid.Empty,
                    OrganisationName = body.OrganisationName ?? string.Empty,

                    EffectiveMaleCount = effectiveAssignments.Count(x => x.Sex == Sex.Male),
                    EffectiveFemaleCount = effectiveAssignments.Count(x => x.Sex == Sex.Female),
                    EffectiveUnknownCount = effectiveAssignments.Count(x => !x.Sex.HasValue),

                    SubsidiaryMaleCount = subsidiaryAssignments.Count(x => x.Sex == Sex.Male),
                    SubsidiaryFemaleCount = subsidiaryAssignments.Count(x => x.Sex == Sex.Female),
                    SubsidiaryUnknownCount = subsidiaryAssignments.Count(x => !x.Sex.HasValue),

                    EffectiveTotalCount = effectiveTotalCount,
                    SubsidiaryTotalCount = subsidiaryTotalCount,

                    AssignedCount = assignedCount,
                    UnassignedCount = totalCount - assignedCount,

                    PolicyDomainClassificationId = policyDomain?.BodyClassificationId,
                    PolicyDomainClassificationName = policyDomain?.BodyClassificationName ?? string.Empty,

                    ResponsibleMinisterClassificationId = responsibleMinister?.BodyClassificationId,
                    ResponsibleMinisterClassificationName = responsibleMinister?.BodyClassificationName ?? string.Empty,

                    TotalCount = totalCount
                };
            })
            .Where(summary => summary.UnassignedCount == 0);
    }

    /// <summary>
    /// </summary>
    /// <param name="results"></param>
    /// <returns></returns>
    public static IEnumerable<ParticipationSummary> Map(
        IEnumerable<ParticipationSummary> results)
    {
        var participations = new List<ParticipationSummary>();
        var lower = Math.Floor(1m / 3 * 100) / 100;
        var upper = Math.Ceiling(2m / 3 * 100) / 100;

        foreach (var result in results)
        {
            if (result.AssignedCount > 0)
            {
                result.SubsidiaryMalePercentage = ParticipationCalculator.CalculatePercentage(result.SubsidiaryMaleCount, result.SubsidiaryTotalCount);
                result.SubsidiaryFemalePercentage = ParticipationCalculator.CalculatePercentage(result.SubsidiaryFemaleCount, result.SubsidiaryTotalCount);
                result.SubsidiaryUnknownPercentage = ParticipationCalculator.CalculatePercentage(result.SubsidiaryUnknownCount, result.SubsidiaryTotalCount);

                result.EffectiveMalePercentage = ParticipationCalculator.CalculatePercentage(result.EffectiveMaleCount, result.EffectiveTotalCount);
                result.EffectiveFemalePercentage = ParticipationCalculator.CalculatePercentage(result.EffectiveFemaleCount, result.EffectiveTotalCount);
                result.EffectiveUnknownPercentage = ParticipationCalculator.CalculatePercentage(result.EffectiveUnknownCount, result.EffectiveTotalCount);

                result.IsEffectiveCompliant =
                    result.EffectiveTotalCount == 0 ||
                    result.EffectiveMalePercentage >= lower && result.EffectiveMalePercentage <= upper &&
                    result.EffectiveFemalePercentage >= lower && result.EffectiveFemalePercentage <= upper;

                result.IsSubsidiaryCompliant =
                    result.SubsidiaryTotalCount == 0 ||
                    result.SubsidiaryMalePercentage >= lower && result.SubsidiaryMalePercentage <= upper &&
                    result.SubsidiaryFemalePercentage >= lower && result.SubsidiaryFemalePercentage <= upper;

                result.IsTotalCompliant = result.IsEffectiveCompliant && result.IsSubsidiaryCompliant;
            }

            participations.Add(result);
        }

        return participations;
    }

    /// <summary>
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
            case "bodyname":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.BodyName)
                    : results.OrderByDescending(x => x.BodyName);
            case "istotalcompliant":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.IsTotalCompliant)
                    : results.OrderByDescending(x => x.IsTotalCompliant);
            case "responsibleministerclassificationname":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.ResponsibleMinisterClassificationName)
                    : results.OrderByDescending(x => x.ResponsibleMinisterClassificationName);
            case "policydomainclassificationname":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.PolicyDomainClassificationName)
                    : results.OrderByDescending(x => x.PolicyDomainClassificationName);
            default:
                return results.OrderBy(x => x.BodyName).ThenBy(x => x.BodyName);
        }
    }
}