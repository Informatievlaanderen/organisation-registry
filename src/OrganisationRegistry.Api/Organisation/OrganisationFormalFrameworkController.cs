namespace OrganisationRegistry.Api.Organisation
{
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using System.Threading.Tasks;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using System;
    using SqlServer.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Search.Pagination;
    using SqlServer.Organisation;
    using Infrastructure.Search.Filtering;
    using Queries;
    using System.Net;
    using Infrastructure.Security;
    using Security;

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
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationFormalFrameworkListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationFormalFrameworkListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a formal framework for an organisation.</summary>
        /// <response code="200">If the formal framework is found.</response>
        /// <response code="404">If the formal framework cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationFormalFrameworkListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
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
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] AddOrganisationFormalFrameworkRequest message)
        {
            var internalMessage = new AddOrganisationFormalFrameworkInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.OrganisationFormalFrameworkId }), null);
        }

        /// <summary>Update a formal framework for an organisation.</summary>
        /// <response code="201">If the formal framework is updated, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFormalFrameworkRequest message)
        {
            var internalMessage = new UpdateOrganisationFormalFrameworkInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
