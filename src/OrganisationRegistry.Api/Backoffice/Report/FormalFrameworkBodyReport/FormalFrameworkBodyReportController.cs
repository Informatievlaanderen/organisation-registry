namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkBodyReport;

using System;
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

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
public class FormalFrameworkBodyReportController : OrganisationRegistryController
{
    private readonly ApiConfigurationSection _config;

    private const string ScrollTimeout = "30s";
    private const int ScrollSize = 500;

    public FormalFrameworkBodyReportController(
        IOptions<ApiConfigurationSection> config)
    {
        _config = config.Value;
    }

    /// <summary>
    /// Get all bodies for a formal framework.
    /// </summary>
    /// <param name="elastic"></param>
    /// <param name="id">A formal framework GUID identifier</param>
    /// <returns></returns>
    [HttpGet("formalframeworkbodies/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetFormalFrameworkBodies(
        [FromServices] Elastic elastic,
        [FromRoute] Guid id)
    {
        var sorting = Request.ExtractSortingRequest();

        var orderedResults =
            FormalFrameworkBody.Sort(
                    FormalFrameworkBody.Map(
                        await FormalFrameworkBody.Search(
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
