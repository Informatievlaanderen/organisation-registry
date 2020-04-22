namespace OrganisationRegistry.Api.LabelType
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
    using SqlServer.LabelType;
    using System.Net;
    using Infrastructure.Security;
    using Security;
    using OrganisationRegistry.Organisation;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("labeltypes")]
    public class LabelTypeController : OrganisationRegistryController
    {
        public LabelTypeController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available label types.</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IOrganisationRegistryConfiguration configuration)
        {
            var filtering = Request.ExtractFilteringRequest<LabelTypeListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedLabelTypes = new LabelTypeListQuery(context, configuration).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedLabelTypes.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedLabelTypes.Items.ToListAsync());
        }

        /// <summary>Get a label type.</summary>
        /// <response code="200">If the label type is found.</response>
        /// <response code="404">If the label type cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(LabelTypeListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var key = await context.LabelTypeList.FirstOrDefaultAsync(x => x.Id == id);

            if (key == null)
                return NotFound();

            return Ok(key);
        }

        /// <summary>Create a label type.</summary>
        /// <response code="201">If the label type is created, together with the location.</response>
        /// <response code="400">If the label type information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateLabelTypeRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateLabelTypeRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update a label type.</summary>
        /// <response code="200">If the label type is updated, together with the location.</response>
        /// <response code="400">If the label type information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateLabelTypeRequest message)
        {
            var internalMessage = new UpdateLabelTypeInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateLabelTypeRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.LabelTypeId }));
        }
    }
}
