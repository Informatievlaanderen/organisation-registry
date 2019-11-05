namespace OrganisationRegistry.Api.SeatType
{
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;
    using SqlServer.SeatType;
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("seattypes")]
    public class SeatTypeController : OrganisationRegistryController
    {
        public SeatTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available seat types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<SeatTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedSeatTypes = new SeatTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedSeatTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedSeatTypes.Items.ToListAsync());
        }

        /// <summary>Get a seat type.</summary>
        /// <response code="200">If the seat type is found.</response>
        /// <response code="404">If the seat type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(SeatTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.SeatTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create a seat type.</summary>
        /// <response code="201">If the seat type is created, together with the location.</response>
        /// <response code="400">If the seat type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateSeatTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateSeatTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a seat type.</summary>
        /// <response code="200">If the seat type is updated, together with the location.</response>
        /// <response code="400">If the seat type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateSeatTypeRequest message)
        {
            var internalMessage = new UpdateSeatTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateSeatTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.SeatTypeId }));
        }
    }
}
