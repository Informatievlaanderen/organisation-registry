namespace OrganisationRegistry.Api.Backoffice.Body.Detail;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
public class BodyDetailController : OrganisationRegistryController
{
    public BodyDetailController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a body.</summary>
    /// <response code="200">If the body is found.</response>
    /// <response code="404">If the body cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var body =
            await context.BodyDetail
                .AsQueryable()
                .FirstOrDefaultAsync(x => x.Id == id);

        if (body == null)
            return NotFound();

        var hasAllSeatsAssigned = BodyParticipationStatus.HasAllSeatsAssigned(context, id);

        return Ok(new BodyResponse(body, hasAllSeatsAssigned, BodyParticipationStatus.IsMepCompliant(context, id)));
    }


    /// <summary>Register a body.</summary>
    /// <response code="201">If the body is registered, together with the location.</response>
    /// <response code="400">If the body information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] ISecurityService securityService, [FromServices] OrganisationRegistryContext context, [FromBody] RegisterBodyRequest message)
    {
        if (message.OrganisationId.HasValue && !await securityService.CanAddBody(User, message.OrganisationId))
            ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authInfo = await HttpContext.GetAuthenticateInfoAsync();
        if (authInfo?.Principal == null || !authInfo.Principal.IsInRole(Roles.Developer))
            message.BodyNumber = string.Empty;

        await CommandSender.Send(
            RegisterBodyRequestMapping.Map(
                message,
                context.LifecyclePhaseTypeList.SingleOrDefault(x => x.RepresentsActivePhase && x.IsDefaultPhase),
                context.LifecyclePhaseTypeList.SingleOrDefault(x => !x.RepresentsActivePhase && x.IsDefaultPhase)));

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }
}