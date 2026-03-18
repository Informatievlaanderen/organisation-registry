namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassificationType;

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
[OrganisationRegistryRoute("organisationclassificationtypes")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class OrganisationClassificationTypeCommandController : OrganisationRegistryCommandController
{
    public OrganisationClassificationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een organisatieclassificatietype.</summary>
    /// <response code="201">Als het organisatieclassificatietype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het organisatieclassificatietype mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateOrganisationClassificationTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateOrganisationClassificationTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(OrganisationClassificationTypeController), nameof(OrganisationClassificationTypeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een organisatieclassificatietype aan.</summary>
    /// <response code="200">Als het organisatieclassificatietype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het organisatieclassificatietype mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateOrganisationClassificationTypeRequest message)
    {
        var internalMessage = new UpdateOrganisationClassificationTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationClassificationTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(OrganisationClassificationTypeController), nameof(OrganisationClassificationTypeController.Get), new { id = internalMessage.OrganisationClassificationTypeId });
    }
}
