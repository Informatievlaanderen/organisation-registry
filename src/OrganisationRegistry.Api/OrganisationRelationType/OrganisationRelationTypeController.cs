namespace OrganisationRegistry.Api.OrganisationRelationType
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
    using SqlServer.OrganisationRelationType;
    using System.Net;
    using Infrastructure.Security;
    using Security;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisationrelationtypes")]
    public class OrganisationRelationTypeController : OrganisationRegistryController
    {
        public OrganisationRelationTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available organisation relation types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationRelationTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisationRelationTypes = new OrganisationRelationTypeListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisationRelationTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisationRelationTypes.Items.ToListAsync());
        }

        /// <summary>Get an organisation relation type.</summary>
        /// <response code="200">If the organisation relation type is found.</response>
        /// <response code="404">If the organisation relation type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationRelationTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.OrganisationRelationTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create an organisation relation type.</summary>
        /// <response code="201">If the organisation relation type is created, together with the location.</response>
        /// <response code="400">If the organisation relation type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Post([FromBody] CreateOrganisationRelationTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            CommandSender.Send(CreateOrganisationRelationTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update an organisation relation type.</summary>
        /// <response code="200">If the organisation relation type is updated, together with the location.</response>
        /// <response code="400">If the organisation relation type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public IActionResult Put([FromRoute] Guid id, [FromBody] UpdateOrganisationRelationTypeRequest message)
        {
            var internalMessage = new UpdateOrganisationRelationTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            CommandSender.Send(UpdateOrganisationRelationTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.OrganisationRelationTypeId }));
        }
    }
}
