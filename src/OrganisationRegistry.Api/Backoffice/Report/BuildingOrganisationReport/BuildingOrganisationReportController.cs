namespace OrganisationRegistry.Api.Backoffice.Report.BuildingOrganisationReport;

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
public class BuildingOrganisationReportController : OrganisationRegistryController
{
    private const string ScrollTimeout = "30s";
    private const int ScrollSize = 500;

    private readonly ApiConfigurationSection _config;
    private readonly IDateTimeProvider _dateTimeProvider;

    public BuildingOrganisationReportController(
        IOptions<ApiConfigurationSection> config,
        IDateTimeProvider dateTimeProvider)
    {
        _dateTimeProvider = dateTimeProvider;
        _config = config.Value;
    }

    /// <summary>Rapport: Organisaties per gebouw.</summary>
    /// <remarks>Geef alle organisaties voor een gebouw.</remarks>
    /// <param name="elastic"></param>
    /// <param name="id">A formal framework GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Organisaties per gebouw.</response>
    [HttpGet("buildingorganisations/{id}")]
    [ProducesResponseType(typeof(List<BuildingOrganisation>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BuildingOrganisationListExamples))]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBuildingOrganisations(
        [FromServices] Elastic elastic,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            BuildingOrganisation.Sort(
                    BuildingOrganisation.Map(
                        await BuildingOrganisation.Search(
                            elastic.ReadClient,
                            id,
                            ScrollSize,
                            ScrollTimeout),
                        id,
                        _config,
                        _dateTimeProvider),
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
}
