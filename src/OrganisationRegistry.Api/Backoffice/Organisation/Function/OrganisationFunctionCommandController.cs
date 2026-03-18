namespace OrganisationRegistry.Api.Backoffice.Organisation.Function;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/functions")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationFunctionCommandController : OrganisationRegistryCommandController
{
    public OrganisationFunctionCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een functie toe aan een organisatie.</summary>
    /// <response code="201">Als de functie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de functie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationFunctionRequest message)
    {
        var internalMessage = new AddOrganisationFunctionInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationFunctionRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationFunctionController), nameof(OrganisationFunctionController.Get), new { id = message.OrganisationFunctionId });
    }

    /// <summary>Pas een functie aan voor een organisatie.</summary>
    /// <response code="201">Als de functie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de functie mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFunctionRequest message)
    {
        var internalMessage = new UpdateOrganisationFunctionInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationFunctionRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>Verwijder een functie van een organisatie.</summary>
    /// <response code="204">Als de functie succesvol verwijderd is.</response>
    /// <response code="400">Als de functie niet gevonden kan worden voor de organisatie.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        await CommandSender.Send(new RemoveOrganisationFunction(new OrganisationId(organisationId), id));
        return NoContent();
    }
}
