namespace OrganisationRegistry.Api.Backoffice.Body
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;
    using Responses;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies")]
    public class BodyInfoController : OrganisationRegistryController
    {
        public BodyInfoController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a body's information.</summary>
        /// <response code="200">If the body is found.</response>
        /// <response code="404">If the body cannot be found.</response>
        [HttpGet("{id}/info")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var body = await context.BodyDetail.FirstOrDefaultAsync(x => x.Id == id);

            if (body == null)
                return NotFound();

            return Ok(new BodyInfoResponse(body));
        }

        /// <summary>Update a body's info.</summary>
        /// <response code="200">If the body info is updated, together with the location.</response>
        /// <response code="400">If the body information does not pass validation.</response>
        [HttpPut("{id}/info")]
        [OrganisationRegistryAuthorize]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromServices] ISecurityService securityService, [FromRoute] Guid id, [FromBody] UpdateBodyInfoRequest message)
        {
            var internalMessage = new UpdateBodyInfoInternalRequest(id, message);

            if (!await securityService.CanEditBody(User, internalMessage.BodyId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor dit orgaan.");

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyInfoRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
