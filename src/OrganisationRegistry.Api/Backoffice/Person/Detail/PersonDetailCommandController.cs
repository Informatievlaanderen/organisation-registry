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
public class PersonDetailCommandController : OrganisationRegistryCommandController
{
    public PersonDetailCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a person.</summary>
    /// <response code="201">If the person is created, together with the location.</response>
    /// <response code="400">If the person information does not pass validation.</response>
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

    /// <summary>Update a person.</summary>
    /// <response code="200">If the person is updated, together with the location.</response>
    /// <response code="400">If the person information does not pass validation.</response>
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
