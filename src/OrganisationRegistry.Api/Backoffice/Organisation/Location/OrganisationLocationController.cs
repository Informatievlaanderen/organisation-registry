namespace OrganisationRegistry.Api.Backoffice.Organisation.Location
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
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/locations")]
    public class OrganisationLocationController : OrganisationRegistryController
    {
        public OrganisationLocationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available locations for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] ISecurityService securityService,
            [FromServices] IMemoryCaches memoryCaches,
            [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationLocationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationLocationListQuery(
                context,
                memoryCaches,
                organisationId,
                await securityService.GetUser(User)
            ).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a location for an organisation.</summary>
        /// <response code="200">If the location is found.</response>
        /// <response code="404">If the location cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationLocationList.FirstOrDefaultAsync(x => x.OrganisationLocationId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Add a location to an organisation.</summary>
        /// <response code="201">If the location is added, together with the location.</response>
        /// <response code="400">If the location information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationLocationRequest message)
        {
            var internalMessage = new AddOrganisationLocationInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationLocationRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.OrganisationLocationId });
        }

        /// <summary>Update a location for an organisation.</summary>
        /// <response code="201">If the location is updated, together with the location.</response>
        /// <response code="400">If the location information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationLocationRequest message)
        {
            var internalMessage = new UpdateOrganisationLocationInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationLocationRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
