namespace OrganisationRegistry.Api.Backoffice.Parameters.Purpose;

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
[OrganisationRegistryRoute("purposes")]
[OrganisationRegistryAuthorize]
public class PurposeCommandController : OrganisationRegistryController
{
    public PurposeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a purpose.</summary>
    /// <response code="201">If the purpose is created, together with the location.</response>
    /// <response code="400">If the purpose information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreatePurposeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreatePurposeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(PurposeController), nameof(PurposeController.Get), new { id = message.Id });
    }

    /// <summary>Update a purpose.</summary>
    /// <response code="200">If the purpose is updated, together with the location.</response>
    /// <response code="400">If the purpose information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdatePurposeRequest message)
    {
        var internalMessage = new UpdatePurposeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdatePurposeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(PurposeController), nameof(PurposeController.Get), new { id = internalMessage.PurposeId });
    }
}
