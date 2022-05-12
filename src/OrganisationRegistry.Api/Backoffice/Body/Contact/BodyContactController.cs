namespace OrganisationRegistry.Api.Backoffice.Body.Contact
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure;
    using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
    using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using OrganisationRegistry.Api.Infrastructure.Security;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/contacts")]
    public class BodyContactController : OrganisationRegistryController
    {
        public BodyContactController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available contacts for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyContactListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodies = new BodyContactListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodies.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodies.Items.ToListAsync());
        }

        /// <summary>Get a contact for a body.</summary>
        /// <response code="200">If the contact is found.</response>
        /// <response code="404">If the contact cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var organisation = await context.BodyContactList.FirstOrDefaultAsync(x => x.BodyContactId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Create a contact for an organisation.</summary>
        /// <response code="201">If the contact is created, together with the location.</response>
        /// <response code="400">If the contact information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyContactRequest message)
        {
            var internalMessage = new AddBodyContactInternalRequest(bodyId, message);

            if (!await securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodyContactRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.BodyContactId });
        }

        /// <summary>Update a contact for an organisation.</summary>
        /// <response code="201">If the contact is updated, together with the location.</response>
        /// <response code="400">If the contact information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyContactRequest message)
        {
            var internalMessage = new UpdateBodyContactInternalRequest(bodyId, message);

            if (!await securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyContactRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
