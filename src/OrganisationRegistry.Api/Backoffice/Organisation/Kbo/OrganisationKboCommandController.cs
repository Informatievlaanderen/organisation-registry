namespace OrganisationRegistry.Api.Backoffice.Organisation.Kbo;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationKboCommandController : OrganisationRegistryCommandController
{
    public OrganisationKboCommandController(ICommandSender commandSender)
        : base(commandSender) { }

    /// <summary>Koppel een organisatie aan een KBO-nummer.</summary>
    /// <response code="200">Als de organisatie gekoppeld is.</response>
    [HttpPut("{id}/kbo/number/{kboNumber}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CoupleToKboNumber(
        [FromRoute] Guid id,
        [FromRoute] string kboNumber)
    {
        await CommandSender.Send(
            new CoupleOrganisationToKbo(
                new OrganisationId(id),
                new KboNumber(kboNumber)));

        return Ok();
    }

    /// <summary>Annuleer de actieve koppeling van een organisatie met een KBO-nummer.</summary>
    /// <response code="200">Als de koppeling geannuleerd is.</response>
    [HttpPut("{id}/kbo/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelCouplingWithKbo([FromRoute] Guid id)
    {
        await CommandSender.Send(
            new CancelCouplingWithKbo(
                new OrganisationId(id)));

        return Ok();
    }

    /// <summary>Pas de organisatie aan op basis van gegevens uit de KBO.</summary>
    /// <response code="200">Als de organisatie succesvol aangepast is.</response>
    [HttpPut("{id}/kbo/sync")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> UpdateFromKbo([FromRoute] Guid id)
    {
        await CommandSender.Send(
            new SyncOrganisationWithKbo(
                new OrganisationId(id),
                DateTimeOffset.Now,
                null));

        return Ok();
    }

    /// <summary>Beëindig de organisatie op basis van gegevens uit de KBO.</summary>
    /// <response code="200">Als de organisatie beëindigd is.</response>
    [HttpPut("{id}/kbo/terminate")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> TerminateKboCoupling([FromRoute] Guid id)
    {
        await CommandSender.Send(
            new SyncOrganisationTerminationWithKbo(
                new OrganisationId(id)));

        return Ok();
    }
}
