namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationTheme;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.RegulationTheme;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("regulationthemes")]
public class RegulationThemeController : OrganisationRegistryController
{
    public RegulationThemeController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available regulation themes.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<RegulationThemeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedRegulationThemes = new RegulationThemeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedRegulationThemes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedRegulationThemes.Items.ToListAsync());
    }

    /// <summary>Get a regulation theme.</summary>
    /// <response code="200">If the regulation theme is found.</response>
    /// <response code="404">If the regulation theme cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.RegulationThemeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
