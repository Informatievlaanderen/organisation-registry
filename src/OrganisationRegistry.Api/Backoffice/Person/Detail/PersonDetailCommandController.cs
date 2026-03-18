namespace OrganisationRegistry.Api.Backoffice.Person.Detail;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("people")]
[OrganisationRegistryAuthorize]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonDetailCommandController : OrganisationRegistryCommandController
{
    public PersonDetailCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Registreer een persoon.</summary>
    /// <response code="201">Als de persoon succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de persoon mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreatePersonRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreatePersonRequestMapping.Map(message));

        return CreatedWithLocation(nameof(PersonDetailController), nameof(PersonDetailController.Get), new { id = message.Id });
    }

    /// <summary>Pas een persoon aan.</summary>
    /// <response code="200">Als de persoon succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de persoon mislukt is.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdatePersonRequest message)
    {
        var internalMessage = new UpdatePersonInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdatePersonRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(PersonDetailController), nameof(PersonDetailController.Get), new { id = internalMessage.PersonId });
    }
}
