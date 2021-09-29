namespace OrganisationRegistry.Api.Backoffice.Body
{
    using System;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using Requests;
    using SqlServer.Body;
    using SqlServer.Infrastructure;

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
        [ProducesResponseType(typeof(BodyLifecyclePhaseListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
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
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyLifecyclePhaseRequest message)
        {
            var internalMessage = new AddBodyLifecyclePhaseInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodyLifecyclePhaseRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.BodyLifecyclePhaseId }), null);
        }

        /// <summary>Update a lifecycle phase for a body.</summary>
        /// <response code="201">If the lifecycle phase is updated, together with the location.</response>
        /// <response code="400">If the lifecycle phase information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyLifecyclePhaseRequest message)
        {
            var internalMessage = new UpdateBodyLifecyclePhaseInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyLifecyclePhaseRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
