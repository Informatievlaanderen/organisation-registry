namespace OrganisationRegistry.Api.OrganisationClassification
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Queries;
    using Requests;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using SqlServer.OrganisationClassification;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using System.Net;
    using Infrastructure.Security;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Security;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisationclassifications")]
    public class OrganisationClassificationController : OrganisationRegistryController
    {
        private readonly ApiConfiguration _config;

        public OrganisationClassificationController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config)
            : base(commandSender)
        {
            _config = config.Value;
        }

        /// <summary>Get a list of available organisation classifications.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
        }

        /// <summary>Get an organisation classification.</summary>
        /// <response code="200">If the organisation classification is found.</response>
        /// <response code="404">If the organisation classification cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationClassificationListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var organisationClassification = await context.OrganisationClassificationList.FirstOrDefaultAsync(x => x.Id == id);

            if (organisationClassification == null)
                return NotFound();

            return Ok(organisationClassification);
        }

        /// <summary>Create an organisation classification.</summary>
        /// <response code="201">If the organisation classificiation is created, together with the location.</response>
        /// <response code="400">If the organisation classificiation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateOrganisationClassificationRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateOrganisationClassificationRequestMapping.Map(message));

            return Created(Url.Action(nameof(Get), new { id = message.Id }), null);
        }

        /// <summary>Update an organisation classification.</summary>
        /// <response code="200">If the organisation classification is updated, together with the location.</response>
        /// <response code="400">If the organisation classification information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.OrganisationRegistryBeheerder)]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateOrganisationClassificationRequest message)
        {
            var internalMessage = new UpdateOrganisationClassificationInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationClassificationRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.OrganisationClassificationId }));
        }
    }
}
