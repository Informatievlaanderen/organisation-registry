namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification
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
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Queries;
    using Requests;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisationclassifications")]
    public class OrganisationClassificationController : OrganisationRegistryController
    {
        private readonly ApiConfigurationSection _config;

        public OrganisationClassificationController(
            ICommandSender commandSender,
            IOptions<ApiConfigurationSection> config)
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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
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
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromBody] CreateOrganisationClassificationRequest message)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await CommandSender.Send(CreateOrganisationClassificationRequestMapping.Map(message));

            return CreatedWithLocation(nameof(Get), new { id = message.Id });
        }

        /// <summary>Update an organisation classification.</summary>
        /// <response code="200">If the organisation classification is updated, together with the location.</response>
        /// <response code="400">If the organisation classification information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder)]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateOrganisationClassificationRequest message)
        {
            var internalMessage = new UpdateOrganisationClassificationInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationClassificationRequestMapping.Map(internalMessage));

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.OrganisationClassificationId });
        }
    }
}
