namespace OrganisationRegistry.Api.Building
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using OrganisationRegistry.Infrastructure.Commands;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Security;
    using SqlServer.Building;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("buildings")]
    public class BuildingController : OrganisationRegistryController
    {
        public BuildingController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available buildings.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<BuildingListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBuildings = new BuildingListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBuildings.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBuildings.Items.ToListAsync());
        }

        /// <summary>Get a building.</summary>
        /// <response code="200">If the building is found.</response>
        /// <response code="404">If the building cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BuildingListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var building = await context.BuildingList.FirstOrDefaultAsync(x => x.Id == id);

            if (building == null)
                return NotFound();

            return Ok(building);
        }

        /// <summary>Create a building.</summary>
        /// <response code="201">If the building is created, together with the location.</response>
        /// <response code="400">If the building information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateBuildingRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateBuildingRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a building.</summary>
        /// <response code="200">If the building is updated, together with the location.</response>
        /// <response code="400">If the building information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBuildingRequest message)
        {
            var internalMessage = new UpdateBuildingInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBuildingRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BuildingId }));
        }
    }
}
