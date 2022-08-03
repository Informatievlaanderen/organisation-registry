namespace OrganisationRegistry.Api.Backoffice.Body.FormalFramework;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/formalframeworks")]
public class BodyFormalFrameworkController : OrganisationRegistryController
{
    /// <summary>Get a list of available formal frameworks for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyFormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyFormalFrameworks = new BodyFormalFrameworkListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Get a formal framework for a body.</summary>
    /// <response code="200">If the formal framework is found.</response>
    /// <response code="404">If the formal framework cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyFormalFramework = await context.BodyFormalFrameworkList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyFormalFrameworkId == id);

        if (bodyFormalFramework == null)
            return NotFound();

        return Ok(bodyFormalFramework);
    }
}
