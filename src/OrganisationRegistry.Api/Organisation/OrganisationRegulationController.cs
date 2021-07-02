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
    using OrganisationRegistry.Infrastructure.Authorization;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/regulations")]
    public class OrganisationRegulationController : OrganisationRegistryController
    {
        public OrganisationRegulationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available regulations for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationRegulationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationRegulationListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }

        /// <summary>Get a regulation for an organisation.</summary>
        /// <response code="200">If the regulation is found.</response>
        /// <response code="404">If the regulation cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(OrganisationRegulationListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
        {
            var organisation = await context.OrganisationRegulationList.FirstOrDefaultAsync(x => x.OrganisationRegulationId == id);

            if (organisation == null)
                return NotFound();

            return Ok(organisation);
        }

        /// <summary>Create a regulation for an organisation.</summary>
        /// <response code="201">If the regulation is created, together with the location.</response>
        /// <response code="400">If the regulation information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] AddOrganisationRegulationRequest message)
        {
            var internalMessage = new AddOrganisationRegulationInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddOrganisationRegulationRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.OrganisationRegulationId }), null);
        }

        /// <summary>Update a regulation for an organisation.</summary>
        /// <response code="201">If the regulation is updated, together with the location.</response>
        /// <response code="400">If the regulation information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid organisationId, [FromBody] UpdateOrganisationRegulationRequest message)
        {
            var internalMessage = new UpdateOrganisationRegulationInternalRequest(organisationId, message);

            if (!securityService.CanEditOrganisation(User, internalMessage.OrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateOrganisationRegulationRequestMapping.Map(internalMessage));

            return Ok();
        }
    }
}
