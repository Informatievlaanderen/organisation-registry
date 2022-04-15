namespace OrganisationRegistry.Api.Backoffice.Parameters.Building
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using Security;
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateBuildingRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateBuildingRequestMapping.Map(message));

            return CreatedWithLocation(nameof(Get), new { id = message.Id });
        }

        /// <summary>Update a building.</summary>
        /// <response code="200">If the building is updated, together with the location.</response>
        /// <response code="400">If the building information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBuildingRequest message)
        {
            var internalMessage = new UpdateBuildingInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBuildingRequestMapping.Map(internalMessage));

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.BuildingId });
        }
    }
}
