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
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationOpeningHourController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van openingsuren voor een organisatie op.</summary>
    /// <response code="200">Een gepagineerde lijst van openingsuren voor de organisatie.</response>
    /// <response code="404">Als de organisatie niet gevonden kan worden.</response>
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

    /// <summary>Vraag een openingsuur voor een organisatie op.</summary>
    /// <response code="200">Als het openingsuur gevonden is.</response>
    /// <response code="404">Als het openingsuur of de organisatie niet gevonden kan worden.</response>
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
