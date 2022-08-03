namespace OrganisationRegistry.Api.Backoffice.Body.Mandate;

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
[OrganisationRegistryRoute("bodies/{bodyId}/mandates")]
public class BodyMandateController : OrganisationRegistryController
{
    /// <summary>Get a list of available mandates for a body.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyMandateListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyMandates = new BodyMandateListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyMandates.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyMandates.Items.ToListAsync());
    }

    /// <summary>Get a mandate for a body.</summary>
    /// <response code="200">If the mandate is found.</response>
    /// <response code="404">If the mandate cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyMandate = await context.BodyMandateList.FirstOrDefaultAsync(x => x.BodyMandateId == id);

        if (bodyMandate == null)
            return NotFound();

        return Ok(new BodyMandateResponse(bodyMandate));
    }
}
