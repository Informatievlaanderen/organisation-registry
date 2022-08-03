namespace OrganisationRegistry.Api.Backoffice.Body.Contact;

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
[OrganisationRegistryRoute("bodies/{bodyId}/contacts")]
public class BodyContactController : OrganisationRegistryController
{
    /// <summary>Get a list of available contacts for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyContactListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodies = new BodyContactListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodies.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodies.Items.ToListAsync());
    }

    /// <summary>Get a contact for a body.</summary>
    /// <response code="200">If the contact is found.</response>
    /// <response code="404">If the contact cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var organisation = await context.BodyContactList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyContactId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
