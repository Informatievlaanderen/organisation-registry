namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType;

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
[OrganisationRegistryRoute("organisationrelationtypes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class OrganisationRelationTypeCommandController : OrganisationRegistryCommandController
{
    public OrganisationRelationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een organisatierelatietype.</summary>
    /// <response code="201">Als het organisatierelatietype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het organisatierelatietype mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateOrganisationRelationTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateOrganisationRelationTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(OrganisationRelationTypeController), nameof(OrganisationRelationTypeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een organisatierelatietype aan.</summary>
    /// <response code="200">Als het organisatierelatietype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het organisatierelatietype mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateOrganisationRelationTypeRequest message)
    {
        var internalMessage = new UpdateOrganisationRelationTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationRelationTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(OrganisationRelationTypeController), nameof(OrganisationRelationTypeController.Get), new { id = internalMessage.OrganisationRelationTypeId });
    }
}
