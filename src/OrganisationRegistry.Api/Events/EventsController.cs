namespace OrganisationRegistry.Api.Events
{
    using System.Collections.Generic;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using SqlServer.Infrastructure;
    using System.Net;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Security;
    using SqlServer.Event;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("events")]
    public class EventsController : OrganisationRegistryController
    {
        public EventsController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of events.</summary>
        [HttpGet]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.Developer)]
        [ProducesResponseType(typeof(List<EventListItem>), (int) HttpStatusCode.OK)]
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

        /// <summary>Get an event.</summary>
        /// <response code="200">If the event is found.</response>
        /// <response code="404">If the event cannot be found.</response>
        [HttpGet("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder + "," + Roles.Developer)]
        [ProducesResponseType(typeof(EventWithData), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] int id)
        {
            var eventData = await context.Events.FirstOrDefaultAsync(x => x.Number == id);

            if (eventData == null)
                return NotFound();

            return Ok(new EventWithData(eventData));
        }
    }
}
