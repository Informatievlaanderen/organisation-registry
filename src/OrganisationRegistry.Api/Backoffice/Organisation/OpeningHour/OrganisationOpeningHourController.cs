namespace OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/openingHours")]
public class OrganisationOpeningHourController : OrganisationRegistryController
{
    /// <summary>List opening hours for an organisation.</summary>
    /// <response code="200">Returns a paginated list of opening hours for the organisation.</response>
    /// <response code="404">If the organisation cannot be found.</response>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationOpeningHourListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisations = new OrganisationOpeningHourListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a specific opening hour for an organisation.</summary>
    /// <response code="200">If the opening hour is found.</response>
    /// <response code="404">If the opening hour or organisation cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid organisationId,
        [FromRoute] Guid id)
    {
        var openingHour = await context.OrganisationOpeningHourList.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationOpeningHourId == id);
        if (openingHour == null)
            return NotFound();

        return Ok(openingHour);
    }
}
