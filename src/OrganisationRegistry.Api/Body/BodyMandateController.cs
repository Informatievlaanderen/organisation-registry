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
    using Infrastructure.Search.Filtering;
    using Queries;
    using System.Net;
    using Infrastructure.Security;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Responses;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies/{bodyId}/mandates")]
    public class BodyMandateController : OrganisationRegistryController
    {
        public BodyMandateController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available mandates for a body.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyMandateListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedBodyMandates = new BodyMandateListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedBodyMandates.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedBodyMandates.Items.ToListAsync());
        }

        /// <summary>Get a mandate for a body.</summary>
        /// <response code="200">If the mandate is found.</response>
        /// <response code="404">If the mandate cannot be found.</response>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(BodyMandateResponse), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
        {
            var bodyMandate = await context.BodyMandateList.FirstOrDefaultAsync(x => x.BodyMandateId == id);

            if (bodyMandate == null)
                return NotFound();

            return Ok(new BodyMandateResponse(bodyMandate));
        }

        /// <summary>Create a mandate for a body.</summary>
        /// <response code="201">If the mandate is created, together with the location.</response>
        /// <response code="400">If the mandate information does not pass validation.</response>
        [HttpPost]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(CreatedResult), (int)HttpStatusCode.Created)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] AddBodyMandateRequest message)
        {
            var internalMessage = new AddBodyMandateInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            if (!internalMessage.Body.BodyMandateType.HasValue)
                return BadRequest("Body Mandate Type is required."); // NOTE: this should never happen.

            switch (internalMessage.Body.BodyMandateType.Value)
            {
                case BodyMandateType.Person:
                    await CommandSender.Send(AddBodyMandateRequestMapping.MapForPerson(internalMessage));
                    break;
                case BodyMandateType.FunctionType:
                    await CommandSender.Send(AddBodyMandateRequestMapping.MapForFunctionType(internalMessage));
                    break;
                case BodyMandateType.Organisation:
                    await CommandSender.Send(AddBodyMandateRequestMapping.MapForOrganisation(internalMessage));
                    break;
            }

            return Created(Url.Action(nameof(Get), new { id = message.BodyMandateId }), null);
        }

        /// <summary>Update a mandate for a body.</summary>
        /// <response code="201">If the mandate is updated, together with the location.</response>
        /// <response code="400">If the mandate information does not pass validation.</response>
        [HttpPut("{id}")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(typeof(OkResult), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid bodyId, [FromBody] UpdateBodyMandateRequest message)
        {
            var internalMessage = new UpdateBodyMandateInternalRequest(bodyId, message);

            if (!securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            if (!internalMessage.Body.BodyMandateType.HasValue)
                return BadRequest("Body Mandate Type is required."); // NOTE: this should never happen.

            switch (internalMessage.Body.BodyMandateType.Value)
            {
                case BodyMandateType.Person:
                    await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForPerson(internalMessage));
                    break;
                case BodyMandateType.FunctionType:
                    await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForFunctionType(internalMessage));
                    break;
                case BodyMandateType.Organisation:
                    await CommandSender.Send(UpdateBodyMandateRequestMapping.MapForOrganisation(internalMessage));
                    break;
            }

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
