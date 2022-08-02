namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose;

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
using SqlServer.Purpose;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("purposes")]
public class PurposeController : OrganisationRegistryController
{
    public PurposeController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available purposes.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<PurposeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPurposes = new PurposeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPurposes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPurposes.Items.ToListAsync());
    }

    /// <summary>Get a purpose.</summary>
    /// <response code="200">If the purpose is found.</response>
    /// <response code="404">If the purpose cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.PurposeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
