namespace OrganisationRegistry.Api.Backoffice.Admin.Events;

using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using Queries;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("events")]
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.Developer)]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Administratie")]
public class EventsController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van events op.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<EventListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFunctions = new EventListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFunctions.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFunctions.Items.ToListAsync());
    }

    /// <summary>Vraag een event op.</summary>
    /// <response code="200">Het event met de opgegeven ID.</response>
    /// <response code="404">Als het event niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] int id)
    {
        var eventData = await context.Events.FirstOrDefaultAsync(x => x.Number == id);

        if (eventData == null)
            return NotFound();

        return Ok(new EventWithData(eventData));
    }
}
