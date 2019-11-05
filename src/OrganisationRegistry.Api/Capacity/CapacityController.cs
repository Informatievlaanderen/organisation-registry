namespace OrganisationRegistry.Api.Capacity
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
    using SqlServer.Capacity;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("capacities")]
    public class CapacityController : OrganisationRegistryController
    {
        public CapacityController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available capacities.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<CapacityListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedCapacities = new CapacityListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedCapacities.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedCapacities.Items.ToListAsync());
        }

        /// <summary>Get a capacity.</summary>
        /// <response code="200">If the capacity is found.</response>
        /// <response code="404">If the capacity cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(CapacityListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.CapacityList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create a capacity.</summary>
        /// <response code="201">If the capacity is created, together with the location.</response>
        /// <response code="400">If the capacity information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateCapacityRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateCapacityRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a capacity.</summary>
        /// <response code="200">If the capacity is updated, together with the location.</response>
        /// <response code="400">If the capacity information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateCapacityRequest message)
        {
            var internalMessage = new UpdateCapacityInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateCapacityRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.CapacityId }));
        }
    }
}
