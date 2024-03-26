namespace OrganisationRegistry.Api.Backoffice.Report.BuildingOrganisationReport;

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using ElasticSearch.Organisations;
using OpenSearch.Client;
using OrganisationRegistry.Infrastructure.Configuration;
using SortOrder = Infrastructure.Search.Sorting.SortOrder;

public class BuildingOrganisation
{
    [ExcludeFromCsv] public Guid? ParentOrganisationId { get; set; }

    [DisplayName("Moeder entiteit")] public string? ParentOrganisationName { get; set; }

    [ExcludeFromCsv] public Guid OrganisationId { get; set; }

    [DisplayName("Entiteit")] public string OrganisationName { get; set; }

    [DisplayName("Korte naam")] public string? OrganisationShortName { get; set; }

    [DisplayName("OVO-nummer")] public string OrganisationOvoNumber { get; set; }

    [DisplayName("Data.Vlaanderen link")] public Uri DataVlaanderenOrganisationUri { get; set; }

    [DisplayName("Juridische vorm")] public string LegalForm { get; set; }

    [DisplayName("Beleidsdomein")] public string PolicyDomain { get; set; }

    [DisplayName("Bevoegde minister")] public string ResponsibleMinister { get; set; }

    /// <summary>
    ///
    /// </summary>
    /// <param name="document"></param>
    /// <param name="params"></param>
    /// <param name="today"></param>
    public BuildingOrganisation(
        OrganisationDocument document,
        ApiConfigurationSection @params,
        DateTime today)
    {
        var parent = document
            .Parents
            .FirstOrDefault(
                x => x.Validity.IsInfinite() ||
                     (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                     (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now));

        ParentOrganisationId = parent?.ParentOrganisationId;
        ParentOrganisationName = parent?.ParentOrganisationName;

        OrganisationId = document.Id;
        OrganisationName = document.Name;
        OrganisationShortName = document.ShortName;
        OrganisationOvoNumber = document.OvoNumber;

        DataVlaanderenOrganisationUri = new Uri(string.Format(@params.DataVlaanderenOrganisationUri, document.OvoNumber));

        LegalForm = document.OrganisationClassifications
            .FirstOrDefault(x => x.OrganisationClassificationTypeId == @params.LegalFormClassificationTypeId)
            ?.OrganisationClassificationName ?? string.Empty;

        PolicyDomain = document.OrganisationClassifications
            .FirstOrDefault(x => x.OrganisationClassificationTypeId == @params.PolicyDomainClassificationTypeId)
            ?.OrganisationClassificationName ?? string.Empty;

        ResponsibleMinister = document.OrganisationClassifications
            .FirstOrDefault(
                x =>
                    x.OrganisationClassificationTypeId == @params.ResponsibleMinisterClassificationTypeId &&
                    (!x.Validity.Start.HasValue || x.Validity.Start.Value <= today) &&
                    (!x.Validity.End.HasValue || x.Validity.End.Value >= today)
            )
            ?.OrganisationClassificationName ?? string.Empty;
    }

    /// <summary>
    /// Scroll through all <see cref="OrganisationDocument"/> with a matching (and active) <see cref="FormalFramework"/>, and return entire dataset
    /// </summary>
    /// <param name="client"></param>
    /// <param name="buildingId"></param>
    /// <param name="scrollSize"></param>
    /// <param name="scrollTimeout"></param>
    /// <returns></returns>
    public static async Task<IList<OrganisationDocument>> Search(
        IOpenSearchClient client,
        Guid buildingId,
        int scrollSize,
        string scrollTimeout)
    {
        var results = new List<OrganisationDocument>();

        var scroll = await client.SearchAsync<OrganisationDocument>(
            s => s
                .From(0)
                .Size(scrollSize)
                .Query(
                    q => q
                        .Match(
                            match => match
                                .Field(f => f.Buildings.Single().BuildingId)
                                .Query(buildingId.ToString())))
                .Scroll(scrollTimeout));

        if (scroll.IsValid)
            results.AddRange(scroll.Documents);

        while (scroll.Documents.Any())
        {
            scroll = await client.ScrollAsync<OrganisationDocument>(scrollTimeout, scroll.ScrollId);

            if (scroll.IsValid)
                results.AddRange(scroll.Documents);
        }

        return results;
    }

    /// <summary>
    /// Map each <see cref="OrganisationDocument"/> to a <see cref="BuildingOrganisation"/>
    /// </summary>
    /// <param name="documents"></param>
    /// <param name="buildingId"></param>
    /// <param name="params"></param>
    /// <param name="dateTimeProvider"></param>
    /// <returns></returns>
    public static IEnumerable<BuildingOrganisation> Map(
        IEnumerable<OrganisationDocument> documents,
        Guid buildingId,
        ApiConfigurationSection @params,
        IDateTimeProvider dateTimeProvider)
    {
        var buildingOrganisations = new List<BuildingOrganisation>();

        foreach (var document in documents)
        {
            var buildings = document
                .Buildings
                .Where(
                    x =>
                        x.BuildingId == buildingId &&
                        (x.Validity.IsInfinite() ||
                         (!x.Validity.Start.HasValue || x.Validity.Start.Value <= dateTimeProvider.Today) &&
                         (!x.Validity.End.HasValue || x.Validity.End.Value >= dateTimeProvider.Today)))
                .ToList();

            if (!buildings.Any())
                continue;

            buildingOrganisations.Add(new BuildingOrganisation(document, @params, dateTimeProvider.Today));
        }

        return buildingOrganisations;
    }

    /// <summary>
    ///
    /// </summary>
    /// <param name="results"></param>
    /// <param name="sortingHeader"></param>
    /// <returns></returns>
    public static IOrderedEnumerable<BuildingOrganisation> Sort(
        IEnumerable<BuildingOrganisation> results,
        SortingHeader sortingHeader)
    {
        if (!sortingHeader.ShouldSort)
            return results.OrderBy(x => x.OrganisationName);

        switch (sortingHeader.SortBy.ToLowerInvariant())
        {
            case "legalform":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.LegalForm)
                    : results.OrderByDescending(x => x.LegalForm);
            case "policydomain":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.PolicyDomain)
                    : results.OrderByDescending(x => x.PolicyDomain);
            case "responsibleminister":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.ResponsibleMinister)
                    : results.OrderByDescending(x => x.ResponsibleMinister);
            case "organisationname":
                return sortingHeader.SortOrder == SortOrder.Ascending
                    ? results.OrderBy(x => x.OrganisationName)
                    : results.OrderByDescending(x => x.OrganisationName);
            default:
                return results.OrderBy(x => x.OrganisationName);
        }
    }
}
