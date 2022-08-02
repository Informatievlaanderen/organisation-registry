namespace OrganisationRegistry.Api.Backoffice.Body.Detail;

using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using Security;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies")]
public class BodyDetailCommandController : OrganisationRegistryController
{
    public BodyDetailCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
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
        if (authInfo?.Principal == null || !authInfo.Principal.IsInRole(RoleMapping.Map(Role.Developer)))
            message.BodyNumber = string.Empty;

        await CommandSender.Send(
            RegisterBodyRequestMapping.Map(
                message,
                context.LifecyclePhaseTypeList.SingleOrDefault(x => x.RepresentsActivePhase && x.IsDefaultPhase),
                context.LifecyclePhaseTypeList.SingleOrDefault(x => !x.RepresentsActivePhase && x.IsDefaultPhase)));

        return CreatedWithLocation(nameof(BodyDetailController),nameof(BodyDetailController.Get), new { id = message.Id });
    }
}
