namespace OrganisationRegistry.Api.Backoffice.Organisation.Parent;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/parents")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationParentCommandController : OrganisationRegistryCommandController
{
    public OrganisationParentCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Voeg een bovenliggende organisatie toe.</summary>
    /// <response code="201">Als de bovenliggende organisatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de bovenliggende organisatie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationParentRequest message)
    {
        var internalMessage = new AddOrganisationParentInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationParentRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationParentController), nameof(OrganisationParentController.Get), new { id = message.OrganisationOrganisationParentId });
    }

    /// <summary>Pas de bovenliggende organisatie aan.</summary>
    /// <response code="200">Als de bovenliggende organisatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de bovenliggende organisatie mislukt is.</response>
    /// <response code="200">Als de bovenliggende organisatie succesvol aangepast is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationParentRequest message)
    {
        var internalMessage = new UpdateOrganisationParentInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationParentRequestMapping.Map(internalMessage));

        return Ok();
    }
}
