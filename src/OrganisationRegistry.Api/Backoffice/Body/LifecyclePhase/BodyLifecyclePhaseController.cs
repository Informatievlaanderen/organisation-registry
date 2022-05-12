namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase
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
    using OrganisationRegistry.SqlServer.Body;
    using OrganisationRegistry.SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/lifecyclephases")]
    public class BodyLifecyclePhaseController : OrganisationRegistryController
    {
        public BodyLifecyclePhaseController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available lifecycle phases for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyLifecyclePhaseListItem>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodyLifecyclePhases = new BodyLifecyclePhaseListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodyLifecyclePhases.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodyLifecyclePhases.Items.ToListAsync());
        }

        /// <summary>Get a lifecycle phase for a body.</summary>
        /// <response code="200">If the lifecycle phase is found.</response>
        /// <response code="404">If the lifecycle phase cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var bodyLifecyclePhase = await context.BodyLifecyclePhaseList.FirstOrDefaultAsync(x => x.BodyLifecyclePhaseId == id);

            if (bodyLifecyclePhase == null)
                return NotFound();

            return Ok(bodyLifecyclePhase);
        }

        /// <summary>Create a lifecycle phase for a body.</summary>
        /// <response code="201">If the lifecycle phase is created, together with the location.</response>
        /// <response code="400">If the lifecycle phase information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyLifecyclePhaseRequest message)
        {
            var internalMessage = new AddBodyLifecyclePhaseInternalRequest(bodyId, message);

            if (!await securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodyLifecyclePhaseRequestMapping.Map(internalMessage));

            return CreatedWithLocation(nameof(Get), new { id = message.BodyLifecyclePhaseId });
        }

        /// <summary>Update a lifecycle phase for a body.</summary>
        /// <response code="201">If the lifecycle phase is updated, together with the location.</response>
        /// <response code="400">If the lifecycle phase information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyLifecyclePhaseRequest message)
        {
            var internalMessage = new UpdateBodyLifecyclePhaseInternalRequest(bodyId, message);

            if (!await securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyLifecyclePhaseRequestMapping.Map(internalMessage));

            return OkWithLocationHeader(nameof(Get), new { id = internalMessage.BodyId });
        }
    }
}
