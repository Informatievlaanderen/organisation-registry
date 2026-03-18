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
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyDetailCommandController : OrganisationRegistryCommandController
{
    public BodyDetailCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een orgaan.</summary>
    /// <response code="201">Als het orgaan succesvol geregistreerd is.</response>
    /// <response code="400">Als de validatie voor het orgaan mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromServices] OrganisationRegistryContext context, [FromBody] RegisterBodyRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authInfo = await HttpContext.GetAuthenticateInfoAsync();
        if (authInfo?.Principal == null || !authInfo.Principal.IsInRole(RoleMapping.Map(Role.Developer)))
            message.BodyNumber = string.Empty;

        await CommandSender.Send(
            RegisterBodyRequestMapping.Map(
                message,
                context.LifecyclePhaseTypeList.Single(x => x.RepresentsActivePhase && x.IsDefaultPhase),
                context.LifecyclePhaseTypeList.Single(x => !x.RepresentsActivePhase && x.IsDefaultPhase)));

        return CreatedWithLocation(nameof(BodyDetailController),nameof(BodyDetailController.Get), new { id = message.Id });
    }
}
