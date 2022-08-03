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
public class OrganisationKboCommandController : OrganisationRegistryCommandController
{
    public OrganisationKboCommandController(ICommandSender commandSender)
        : base(commandSender) { }

    /// <summary>Couple an organisation to a KBO number.</summary>
    /// <response code="200">If the organisation was coupled.</response>
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

    /// <summary>Cancel an organisation's active coupling with a KBO number.</summary>
    /// <response code="200">If the organisation coupling was cancelled.</response>
    [HttpPut("{id}/kbo/cancel")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> CancelCouplingWithKbo([FromRoute] Guid id)
    {
        await CommandSender.Send(
            new CancelCouplingWithKbo(
                new OrganisationId(id)));

        return Ok();
    }

    /// <summary>Update the organisation according to data in the KBO.</summary>
    /// <response code="200">If the organisation was updated.</response>
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

    /// <summary>Terminate the organisation according to data in the KBO.</summary>
    /// <response code="200">If the organisation was terminated.</response>
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
