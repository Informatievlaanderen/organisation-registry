namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFramework;

using System;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OrganisationRegistry.Infrastructure.Configuration;
using Queries;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("formalframeworks")]
public class FormalFrameworkController : OrganisationRegistryController
{
    private readonly IOptions<ApiConfigurationSection> _config;

    public FormalFrameworkController(IOptions<ApiConfigurationSection> config)
    {
        _config = config;
    }

    /// <summary>Get a list of available formal frameworks.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFormalFrameworks = new FormalFrameworkListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Get a list of available formal frameworks for vademecum report.</summary>
    [HttpGet("vademecum")]
    public async Task<IActionResult> GetVademecumFormalFrameworks([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        if (filtering.Filter is not { })
            filtering.Filter = new FormalFrameworkListItemFilter();

        if (!_config.Value.VademecumReport_FormalFrameworkIds.IsNullOrWhiteSpace())
            foreach (var id in _config.Value.VademecumReport_FormalFrameworkIds.Split(
                         new[] { ',' },
                         StringSplitOptions.RemoveEmptyEntries))
                if (Guid.TryParse(id, out Guid guid))
                    filtering.Filter.Ids.Add(guid);

        var pagedFormalFrameworks = new FormalFrameworkListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Get a formal framework.</summary>
    /// <response code="200">If the formal framework is found.</response>
    /// <response code="404">If the formal framework cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var formalFramework = await context.FormalFrameworkList
            .AsQueryable()
            .FirstOrDefaultAsync(x => x.Id == id);

        if (formalFramework == null)
            return NotFound();

        return Ok(formalFramework);
    }
}
