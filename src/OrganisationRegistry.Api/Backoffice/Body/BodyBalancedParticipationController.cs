namespace OrganisationRegistry.Api.Backoffice.Body
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Commands;
    using Requests;
    using Responses;
    using Security;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("bodies")]
    [OrganisationRegistryAuthorize(Roles = Roles.OrgaanBeheerder + "," + Roles.OrganisationRegistryBeheerder)]
    public class BodyBalancedParticipationController : OrganisationRegistryController
    {
        public BodyBalancedParticipationController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a body's balanced participation info.</summary>
        /// <response code="200">If the body is found.</response>
        /// <response code="404">If the body cannot be found.</response>
        [HttpGet("{id}/balancedparticipation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
        {
            var body = await context.BodyDetail.FirstOrDefaultAsync(x => x.Id == id);

            if (body == null)
                return NotFound();

            return Ok(new BodyBalancedParticipationResponse(body));
        }

        /// <summary>Update a body's balanced participation info.</summary>
        /// <response code="200">If the body balanced participation info is updated, together with the location.</response>
        /// <response code="400">If the body balanced participation information does not pass validation.</response>
        [HttpPut("{id}/balancedparticipation")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateBodyBalancedParticipationRequest message)
        {
            var internalMessage = new UpdateBodyBalancedParticipationInternalRequest(id, message);

            if (!TryValidateModel(internalMessage))
                return BadRequest(ModelState);

            await CommandSender.Send(UpdateBodyBalancedParticipationRequestMapping.Map(internalMessage));

            return OkWithLocation(Url.Action(nameof(Get), new { id = internalMessage.BodyId }));
        }
    }
}
