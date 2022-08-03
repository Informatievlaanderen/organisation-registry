namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Body;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/lifecyclephases")]
public class BodyLifecyclePhaseController : OrganisationRegistryController
{
    /// <summary>Get a list of available lifecycle phases for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyLifecyclePhaseListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyLifecyclePhases = new BodyLifecyclePhaseListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyLifecyclePhases.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyLifecyclePhases.Items.ToListAsync());
    }

    /// <summary>Get a lifecycle phase for a body.</summary>
    /// <response code="200">If the lifecycle phase is found.</response>
    /// <response code="404">If the lifecycle phase cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyLifecyclePhase = await context.BodyLifecyclePhaseList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyLifecyclePhaseId == id);

        if (bodyLifecyclePhase == null)
            return NotFound();

        return Ok(bodyLifecyclePhase);
    }
}
