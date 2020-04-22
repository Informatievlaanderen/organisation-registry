namespace OrganisationRegistry.Api.Body
{
    using Microsoft.AspNetCore.Mvc;
    using Requests;
    using System.Threading.Tasks;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using System;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Search.Pagination;
    using SqlServer.Body;
    using Infrastructure.Search.Filtering;
    using Queries;
    using System.Net;
    using Infrastructure.Security;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/formalframeworks")]
    public class BodyFormalFrameworkController : OrganisationRegistryController
    {
        public BodyFormalFrameworkController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available formal frameworks for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyFormalFrameworkListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodyFormalFrameworks = new BodyFormalFrameworkListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodyFormalFrameworks.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodyFormalFrameworks.Items.ToListAsync());
        }

        /// <summary>Get a formal framework for a body.</summary>
        /// <response code="200">If the formal framework is found.</response>
        /// <response code="404">If the formal framework cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BodyFormalFrameworkListItem), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var bodyFormalFramework = await context.BodyFormalFrameworkList.FirstOrDefaultAsync(x => x.BodyFormalFrameworkId == id);

            if (bodyFormalFramework == null)
                return NotFound();

            return Ok(bodyFormalFramework);
        }

        /// <summary>Create a formal framework for a body.</summary>
        /// <response code="201">If the formal framework is created, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyFormalFrameworkRequest message)
        {
            var internalMessage = new AddBodyFormalFrameworkInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(AddBodyFormalFrameworkRequestMapping.Map(internalMessage));

            return Created(Url.Action(nameof(Get), new { id = message.BodyFormalFrameworkId }), null);
        }

        /// <summary>Update a formal framework for a body.</summary>
        /// <response code="201">If the formal framework is updated, together with the location.</response>
        /// <response code="400">If the formal framework information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyFormalFrameworkRequest message)
        {
            var internalMessage = new UpdateBodyFormalFrameworkInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyFormalFrameworkRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
