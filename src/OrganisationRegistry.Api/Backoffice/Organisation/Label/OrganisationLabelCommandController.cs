namespace OrganisationRegistry.Api.Backoffice.Organisation.Label;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/labels")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationLabelCommandController : OrganisationRegistryCommandController
{
    public OrganisationLabelCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een label toe aan een organisatie.</summary>
    /// <response code="201">Als de benaming succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de benaming mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationLabelRequest message)
    {
        var internalMessage = new AddOrganisationLabelInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationLabelRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationLabelController), nameof(OrganisationLabelController.Get), new { id = message.OrganisationLabelId });
    }

    /// <summary>Pas een label aan voor een organisatie.</summary>
    /// <response code="201">Als de benaming succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de benaming mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationLabelRequest message)
    {
        var internalMessage = new UpdateOrganisationLabelInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationLabelRequestMapping.Map(internalMessage));

        return Ok();
    }
}
