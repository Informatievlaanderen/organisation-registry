namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkOrganisationReport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using ElasticSearch.Client;
using OrganisationRegistry.Infrastructure.Configuration;
using Infrastructure.Swagger.Examples;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Rapporten")]
public class FormalFrameworkOrganisationReportController : OrganisationRegistryController
{
    private readonly ApiConfigurationSection _config;

    private const string ScrollTimeout = "30s";
    private const int ScrollSize = 500;

    public FormalFrameworkOrganisationReportController(
        IOptions<ApiConfigurationSection> config)
    {
        _config = config.Value;
    }

    /// <summary>Rapport: Organisaties per toepassingsgebied.</summary>
    /// <remarks>Geef alle organisaties voor een toepassingsgebied.</remarks>
    /// <param name="elastic"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="id">A formal framework GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Organisaties per toepassingsgebied.</response>
    [HttpGet("formalframeworkorganisations/{id}")]
    [ProducesResponseType(typeof(List<FormalFrameworkOrganisationBase>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FormalFrameworkOrganisationBaseListExamples))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFormalFrameworkOrganisations(
        [FromServices] Elastic elastic,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            FormalFrameworkOrganisation.Sort(
                    FormalFrameworkOrganisation.Map(
                        await FormalFrameworkOrganisation.Search(
                            elastic.ReadClient,
                            id,
                            ScrollSize,
                            ScrollTimeout),
                        id,
                        _config,
                        dateTimeProvider.Today),
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
                (int) Math.Ceiling((double) orderedResults.Count / pagination.ItemsPerPage)));

        return Ok(
            orderedResults
                .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                .Take(pagination.ItemsPerPage)
                .ToList());
    }

    /// <summary>Rapport: Organisaties per toepassingsgebied (uitgebreid).</summary>
    /// <remarks>Geef alle organisaties voor een toepassingsgebied.</remarks>
    /// <param name="elastic"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="id">A formal framework GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Organisaties per toepassingsgebied (uitgebreid).</response>
    [HttpGet("formalframeworkorganisations/{id}/extended")]
    [ProducesResponseType(typeof(List<FormalFrameworkOrganisationExtended>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FormalFrameworkOrganisationExtendedListExamples))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFormalFrameworkOrganisationsExtended(
        [FromServices] Elastic elastic,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            FormalFrameworkOrganisation.Sort(
                    FormalFrameworkOrganisation.MapExtended(
                        await FormalFrameworkOrganisation.Search(
                            elastic.ReadClient,
                            id,
                            ScrollSize,
                            ScrollTimeout),
                        id,
                        _config,
                        dateTimeProvider.Today),
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

    /// <summary>Rapport: Organisaties per toepassingsgebied (vademecum).</summary>
    /// <remarks>Geef alle organisaties voor een toepassingsgebied.</remarks>
    /// <param name="elastic"></param>
    /// <param name="id">A formal framework GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Organisaties per toepassingsgebied (vademecum).</response>
    [HttpGet("formalframeworkorganisations/vademecum/{id}")]
    [ProducesResponseType(typeof(List<FormalFrameworkOrganisationVademecum>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FormalFrameworkOrganisationVademecumListExamples))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFormalFrameworkOrganisationsVademecum(
        [FromServices] Elastic elastic,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            FormalFrameworkOrganisationVademecum.Sort(
                    FormalFrameworkOrganisationVademecum.Map(
                        await FormalFrameworkOrganisationVademecum.Search(
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
}
