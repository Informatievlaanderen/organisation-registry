namespace OrganisationRegistry.Api.Backoffice.Report.OrganisationClassificationReport;

using System;
using System.Collections.Generic;
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
using SqlServer.OrganisationClassification;
using Infrastructure.Swagger.Examples;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Rapporten")]
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

    /// <summary>Rapport: Organisaties per beleidsdomein.</summary>
    /// <remarks>Geef alle organisaties en hun labels voor een classificatie van het type "Beleidsdomein".</remarks>
    /// <param name="elastic"></param>
    /// <param name="id"></param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Organisaties per beleidsdomein.</response>
    [HttpGet("classificationorganisations/{id}")]
    [ProducesResponseType(typeof(List<ClassificationOrganisation>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ClassificationOrganisationListExamples))]
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

    /// <summary>Rapport: Alle classificaties voor het classificatietype "Beleidsdomein".</summary>
    /// <remarks>Geef alle classificaties voor het classificatietype "Beleidsdomein".</remarks>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Alle classificaties voor het classificatietype "Beleidsdomein".</response>
    [HttpGet("policydomainclassifications")]
    [ProducesResponseType(typeof(List<OrganisationClassificationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationClassificationListExamples))]
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

    /// <summary>Rapport: Alle classificaties voor het classificatietype "Bevoegde minister".</summary>
    /// <remarks>Geef alle classificaties voor het classificatietype "Bevoegde minister".</remarks>
    /// <param name="context"></param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Alle classificaties voor het classificatietype "Bevoegde minister".</response>
    [HttpGet("responsibleministerclassifications")]
    [ProducesResponseType(typeof(List<OrganisationClassificationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationClassificationListExamples))]
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

    /// <summary>Rapport: Meer Evenwichtige Participatie per classificatieorganisatie.</summary>
    /// <remarks>Geef de meer evenwichtige participatie voor een classificatieorganisatie (gegroepeerd per orgaan).</remarks>
    /// <param name="context"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="classificationOrganisationId">A classification organisation GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Meer Evenwichtige Participatie per classificatieorganisatie.</response>
    [HttpGet("classificationorganisationsparticipation/{classificationOrganisationId}")]
    [ProducesResponseType(typeof(List<ClassificationOrganisationParticipation>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ClassificationOrganisationParticipationListExamples))]
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
