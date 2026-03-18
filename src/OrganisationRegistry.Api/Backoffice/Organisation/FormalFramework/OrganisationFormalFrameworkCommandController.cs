namespace OrganisationRegistry.Api.Backoffice.Organisation.FormalFramework;

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
[OrganisationRegistryRoute("organisations/{organisationId}/formalframeworks")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationFormalFrameworkCommandController : OrganisationRegistryCommandController
{
    public OrganisationFormalFrameworkCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een toepassingsgebied toe aan een organisatie.</summary>
    /// <response code="201">Als het toepassingsgebied succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationFormalFrameworkRequest message)
    {
        var internalMessage = new AddOrganisationFormalFrameworkInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof( OrganisationFormalFrameworkController), nameof( OrganisationFormalFrameworkController.Get), new { id = message.OrganisationFormalFrameworkId });
    }

    /// <summary>Pas een toepassingsgebied aan voor een organisatie.</summary>
    /// <response code="200">Als het toepassingsgebied succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    /// <response code="200">Als het toepassingsgebied succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFormalFrameworkRequest message)
    {
        var internalMessage = new UpdateOrganisationFormalFrameworkInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationFormalFrameworkRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>Verwijder een toepassingsgebied van een organisatie.</summary>
    /// <response code="204">Als het toepassingsgebied succesvol verwijderd is.</response>
    /// <response code="400">Als de validatie voor het toepassingsgebied mislukt is.</response>
    [HttpDelete("{id}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        await CommandSender.Send(new RemoveOrganisationFormalFramework(id, new OrganisationId(organisationId)));

        return NoContent();
    }
}
