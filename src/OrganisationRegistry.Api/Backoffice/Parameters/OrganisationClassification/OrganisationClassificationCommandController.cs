namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisationclassifications")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class OrganisationClassificationCommandController : OrganisationRegistryCommandController
{
    public OrganisationClassificationCommandController(
        ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een organisatieclassificatie.</summary>
    /// <response code="201">Als de organisatieclassificatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de organisatieclassificatie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateOrganisationClassificationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateOrganisationClassificationRequestMapping.Map(message));

        return CreatedWithLocation(nameof(OrganisationClassificationController), nameof(OrganisationClassificationController.Get), new { id = message.Id });
    }

    /// <summary>Pas een organisatieclassificatie aan.</summary>
    /// <response code="200">Als de organisatieclassificatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de organisatieclassificatie mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateOrganisationClassificationRequest message)
    {
        var internalMessage = new UpdateOrganisationClassificationInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationClassificationRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(OrganisationClassificationController), nameof(OrganisationClassificationController.Get), new { id = internalMessage.OrganisationClassificationId });
    }
}
