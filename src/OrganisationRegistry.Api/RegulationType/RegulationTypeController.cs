namespace OrganisationRegistry.Api.RegulationType
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.Infrastructure;
    using SqlServer.RegulationType;
    using System.Net;
    using Infrastructure.Security;
    using Security;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("regulationtypes")]
    public class RegulationTypeController : OrganisationRegistryController
    {
        public RegulationTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available regulation types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<RegulationTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedRegulationTypes = new RegulationTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedRegulationTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedRegulationTypes.Items.ToListAsync());
        }

        /// <summary>Get a regulation type.</summary>
        /// <response code="200">If the regulation type is found.</response>
        /// <response code="404">If the regulation type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(RegulationTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.RegulationTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create a regulation type.</summary>
        /// <response code="201">If the regulation type is created, together with the location.</response>
        /// <response code="400">If the regulation type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateRegulationTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateRegulationTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a regulation type.</summary>
        /// <response code="200">If the regulation type is updated, together with the location.</response>
        /// <response code="400">If the regulation type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateRegulationTypeRequest message)
        {
            var internalMessage = new UpdateRegulationTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateRegulationTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.RegulationTypeId }));
        }
    }
}
