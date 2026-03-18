namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType;

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
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("contacttypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class ContactTypeCommandController : OrganisationRegistryCommandController
{
    public ContactTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een contacttype.</summary>
    /// <response code="201">Als het contacttype succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor het contacttype mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateContactTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateContactTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(ContactTypeController),nameof(ContactTypeController.Get), new { id = message.Id });
    }

    /// <summary>Pas een contacttype aan.</summary>
    /// <response code="200">Als het contacttype succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor het contacttype mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateContactTypeRequest message)
    {
        var internalMessage = new UpdateContactTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateContactTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(ContactTypeController),nameof(ContactTypeController.Get), new { id = internalMessage.ContactTypeId });
    }
}
