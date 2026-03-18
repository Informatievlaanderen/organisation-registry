namespace OrganisationRegistry.Api.Backoffice.Organisation.Building;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/buildings")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationBuildingCommandController : OrganisationRegistryCommandController
{
    public OrganisationBuildingCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een gebouw toe aan een organisatie.</summary>
    /// <response code="201">Als het gebouw succesvol toegevoegd is.</response>
    /// <response code="400">Als de validatie voor het gebouw mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationBuildingRequest message)
    {
        var internalMessage = new AddOrganisationBuildingInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationBuildingRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationBuildingController), nameof(OrganisationBuildingController.Get), new { id = message.OrganisationBuildingId });
    }

    /// <summary>Pas een gebouw aan voor een organisatie.</summary>
    /// <response code="200">Als het gebouw succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het gebouw mislukt is.</response>
    /// <response code="200">Als het gebouw succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationBuildingRequest message)
    {
        var internalMessage = new UpdateOrganisationBuildingInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationBuildingRequestMapping.Map(internalMessage));

        return Ok();
    }
}
