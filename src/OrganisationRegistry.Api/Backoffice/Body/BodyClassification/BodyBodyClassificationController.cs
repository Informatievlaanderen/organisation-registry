namespace OrganisationRegistry.Api.Backoffice.Body.BodyClassification;

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
[OrganisationRegistryRoute("bodies/{bodyId}/classifications")]
public class BodyBodyClassificationController : OrganisationRegistryController
{
    /// <summary>Get a list of available classifications for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyBodyClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodys = new BodyBodyClassificationListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodys.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodys.Items.ToListAsync());
    }

    /// <summary>Get a classification for a body.</summary>
    /// <response code="200">If the classification is found.</response>
    /// <response code="404">If the classification cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var body = await context.BodyBodyClassificationList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyBodyClassificationId == id);

        if (body == null)
            return NotFound();

        return Ok(body);
    }
}
