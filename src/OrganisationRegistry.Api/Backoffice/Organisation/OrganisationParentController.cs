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
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/parents")]
    public class OrganisationParentController : OrganisationRegistryController
    {
        public OrganisationParentController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available parents for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationParentListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationParentListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a parent for an organisation.</summary>
        /// <response code="200">If the parent is found.</response>
        /// <response code="404">If the parent cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationParentList.FirstOrDefaultAsync(x => x.OrganisationOrganisationParentId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Create a parent for an organisation.</summary>
        /// <response code="201">If the parent is created, together with the location.</response>
        /// <response code="400">If the parent information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationParentRequest message)
        {
            var internalMessage = new AddOrganisationParentInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationParentRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.OrganisationOrganisationParentId });
        }

        /// <summary>Update a parent for an organisation.</summary>
        /// <response code="201">If the parent is updated, together with the location.</response>
        /// <response code="400">If the parent information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationParentRequest message)
        {
            var internalMessage = new UpdateOrganisationParentInternalRequest(organisationId, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationParentRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
