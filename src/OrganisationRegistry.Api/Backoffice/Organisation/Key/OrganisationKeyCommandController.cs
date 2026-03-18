namespace OrganisationRegistry.Api.Backoffice.Organisation.Key;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/keys")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationKeyCommandController : OrganisationRegistryCommandController
{
    public OrganisationKeyCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een sleutel toe aan een organisatie.</summary>
    /// <response code="201">Als de sleutel succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de sleutel mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationKeyRequest message)
    {
        var internalMessage = new AddOrganisationKeyInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationKeyRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationKeyController), nameof(OrganisationKeyController.Get), new { id = message.OrganisationKeyId });
    }

    /// <summary>Pas een sleutel aan voor een organisatie.</summary>
    /// <response code="200">Als de sleutel succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de sleutel mislukt is.</response>
    /// <response code="200">Als de sleutel succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationKeyRequest message)
    {
        var internalMessage = new UpdateOrganisationKeyInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationKeyRequestMapping.Map(internalMessage));

        return Ok();
    }
}
