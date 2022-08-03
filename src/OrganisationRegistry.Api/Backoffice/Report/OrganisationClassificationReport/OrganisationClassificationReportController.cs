namespace OrganisationRegistry.Api.Backoffice.Report.OrganisationClassificationReport;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification.Queries;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using ElasticSearch.Client;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
public class OrganisationClassificationReportController : OrganisationRegistryController
{
    private const string ScrollTimeout = "30s";
    private const int ScrollSize = 500;

    private readonly ApiConfigurationSection _config;

    public OrganisationClassificationReportController(
        IOptions<ApiConfigurationSection> config)
    {
        _config = config.Value;
    }

    /// <summary>
    /// Get all organisations and their labels for a classification of classificationtype "Beleidsdomein".
    /// </summary>
    /// <param name="elastic"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    [HttpGet("classificationorganisations/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClassificationOrganisations(
        [FromServices] Elastic elastic,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            ClassificationOrganisation.Sort(
                    ClassificationOrganisation.Map(
                        await ClassificationOrganisation.Search(
                            elastic.ReadClient,
                            id,
                            ScrollSize,
                            ScrollTimeout),
                        id,
                        _config),
                    sorting)
                .ToList();

        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        var possiblePagination = Request.ExtractPaginationRequest();

        if (possiblePagination is NoPaginationRequest)
            return Ok(orderedResults);

        var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

        Response.AddPaginationResponse(
            new PaginationInfo(
                pagination.RequestedPage,
                pagination.ItemsPerPage,
                orderedResults.Count,
                (int)Math.Ceiling((double)orderedResults.Count / pagination.ItemsPerPage)));

        return Ok(
            orderedResults
                .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                .Take(pagination.ItemsPerPage)
                .ToList());
    }

    /// <summary>
    /// Get all classifications for classificationtype "Beleidsdomein".
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [HttpGet("policydomainclassifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrganisationClassificationsForPolicyDomain(
        [FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        if (filtering.Filter is not { })
            filtering.Filter = new OrganisationClassificationListItemFilter();

        //  Set classificationtype GUID for "Beleidsdomein"
        filtering.Filter.OrganisationClassificationTypeId = _config.PolicyDomainClassificationTypeId;
        filtering.Filter.OrganisationClassificationTypeName = string.Empty;

        var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
    }

    /// <summary>
    /// Get all classifications for classificationtype "Bevoegde minister".
    /// </summary>
    /// <param name="context"></param>
    /// <returns></returns>
    [HttpGet("responsibleministerclassifications")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetOrganisationClassificationsForResponsibleMinister(
        [FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        if (filtering.Filter is not { })
            filtering.Filter = new OrganisationClassificationListItemFilter();

        //  Set classificationtype GUID for "Beleidsdomein"
        filtering.Filter.OrganisationClassificationTypeId = _config.ResponsibleMinisterClassificationTypeId;
        filtering.Filter.OrganisationClassificationTypeName = string.Empty;

        var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
    }

    /// <summary>
    /// Get gender ratio for a classification organisation (grouped by body)
    /// </summary>
    /// <param name="context"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="classificationOrganisationId">A classification organisation GUID identifier</param>
    /// <returns></returns>
    [HttpGet("classificationorganisationsparticipation/{classificationOrganisationId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetClassificationOrganisationParticipation(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromRoute] Guid classificationOrganisationId)
    {
        var sorting = Request.ExtractSortingRequest();

        var participations =
            ClassificationOrganisationParticipation.Sort(
                    ClassificationOrganisationParticipation.Map(
                        ClassificationOrganisationParticipation.Search(
                            context,
                            classificationOrganisationId,
                            _config,
                            dateTimeProvider.Today)),
                    sorting)
                .ToList();

        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        var possiblePagination = Request.ExtractPaginationRequest();

        if (possiblePagination is NoPaginationRequest)
            return await OkAsync(participations);

        var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

        Response.AddPaginationResponse(
            new PaginationInfo(
                pagination.RequestedPage,
                pagination.ItemsPerPage,
                participations.Count,
                (int)Math.Ceiling((double)participations.Count / pagination.ItemsPerPage)));

        return await OkAsync(
            participations
                .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                .Take(pagination.ItemsPerPage)
                .ToList());
    }
}
