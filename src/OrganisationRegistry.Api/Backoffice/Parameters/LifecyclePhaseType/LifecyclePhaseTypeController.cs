namespace OrganisationRegistry.Api.Backoffice.Parameters.LifecyclePhaseType;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.LifecyclePhaseType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("lifecyclephasetypes")]
public class LifecyclePhaseTypeController : OrganisationRegistryController
{
    /// <summary>Get a list of available lifecyclephase types.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<LifecyclePhaseTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLifecyclePhaseTypes = new LifecyclePhaseTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLifecyclePhaseTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLifecyclePhaseTypes.Items.ToListAsync());
    }

    /// <summary>Get a lifecyclephase type.</summary>
    /// <response code="200">If the lifecyclephase type is found.</response>
    /// <response code="404">If the lifecyclephase type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.LifecyclePhaseTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
