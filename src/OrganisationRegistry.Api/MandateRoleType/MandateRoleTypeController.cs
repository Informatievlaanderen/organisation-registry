namespace OrganisationRegistry.Api.MandateRoleType
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
    using SqlServer.MandateRoleType;
    using System.Net;
    using Infrastructure.Security;
    using Security;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("mandateroletypes")]
    public class MandateRoleTypeController : OrganisationRegistryController
    {
        public MandateRoleTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available mandate role types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<MandateRoleTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedMandateRoleTypes = new MandateRoleTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedMandateRoleTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedMandateRoleTypes.Items.ToListAsync());
        }

        /// <summary>Get a mandate role type.</summary>
        /// <response code="200">If the mandate role type is found.</response>
        /// <response code="404">If the mandate role type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(MandateRoleTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.MandateRoleTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create a mandate role type.</summary>
        /// <response code="201">If the mandate role type is created, together with the location.</response>
        /// <response code="400">If the mandate role type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateMandateRoleTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateMandateRoleTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a mandate role type.</summary>
        /// <response code="200">If the mandate role type is updated, together with the location.</response>
        /// <response code="400">If the mandate role type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateMandateRoleTypeRequest message)
        {
            var internalMessage = new UpdateMandateRoleTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateMandateRoleTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.MandateRoleTypeId }));
        }
    }
}
