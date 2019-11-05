namespace OrganisationRegistry.Api.Location
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.EntityFrameworkCore;
    using Security;
    using SqlServer.Infrastructure;
    using Responses;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("locations")]
    public class LocationController : OrganisationRegistryController
    {
        public LocationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available location types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<LocationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedLocations = new LocationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedLocations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedLocations.Items.ToListAsync());
        }

        /// <summary>Get a location type.</summary>
        /// <response code="200">If the location type is found.</response>
        /// <response code="404">If the location type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LocationResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var location = await context.LocationList.FirstOrDefaultAsync(x => x.Id == id);

            if (location == null)
                return NotFound();

            return Ok(new LocationResponse(location));
        }

        /// <summary>Create a location.</summary>
        /// <response code="201">If the location is created, together with the location.</response>
        /// <response code="400">If the location information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateLocationRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateLocationRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a location.</summary>
        /// <response code="200">If the location is updated, together with the location.</response>
        /// <response code="400">If the location information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateLocationRequest message)
        {
            var internalMessage = new UpdateLocationInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateLocationRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.LocationId }));
        }
    }
}
