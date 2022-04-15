namespace OrganisationRegistry.Api.Backoffice.Organisation
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
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/formalframeworks")]
    public class OrganisationFormalFrameworkController : OrganisationRegistryController
    {
        public OrganisationFormalFrameworkController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available formal frameworks for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IOrganisationRegistryConfiguration configuration,
            [FromServices] IMemoryCaches memoryCaches,
            [FromServices] ISecurityService securityService,
            [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationFormalFrameworkListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationFormalFrameworkListQuery(
                context,
                memoryCaches,
                configuration,
                await securityService.GetUser(User),
                organisationId
                )
                .Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a formal framework for an organisation.</summary>
        /// <response code="200">If the formal framework is found.</response>
        /// <response code="404">If the formal framework cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationFormalFrameworkList.FirstOrDefaultAsync(x => x.OrganisationFormalFrameworkId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Create a formal framework for an organisation.</summary>
        /// <response code="201">If the formal framework is created, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationFormalFrameworkRequest message)
        {
            var internalMessage = new AddOrganisationFormalFrameworkInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.OrganisationFormalFrameworkId });
        }

        /// <summary>Update a formal framework for an organisation.</summary>
        /// <response code="201">If the formal framework is updated, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFormalFrameworkRequest message)
        {
            var internalMessage = new UpdateOrganisationFormalFrameworkInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
