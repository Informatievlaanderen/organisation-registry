namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType;

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
[OrganisationRegistryRoute("mandateroletypes")]
[OrganisationRegistryAuthorize]
public class MandateRoleTypeCommandController : OrganisationRegistryCommandController
{
    public MandateRoleTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a mandate role type.</summary>
    /// <response code="201">If the mandate role type is created, together with the location.</response>
    /// <response code="400">If the mandate role type information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromBody] CreateMandateRoleTypeRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        await CommandSender.Send(CreateMandateRoleTypeRequestMapping.Map(message));

        return CreatedWithLocation(nameof(MandateRoleTypeController), nameof(MandateRoleTypeController.Get), new { id = message.Id });
    }

    /// <summary>Update a mandate role type.</summary>
    /// <response code="200">If the mandate role type is updated, together with the location.</response>
    /// <response code="400">If the mandate role type information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid id, [FromBody] UpdateMandateRoleTypeRequest message)
    {
        var internalMessage = new UpdateMandateRoleTypeInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateMandateRoleTypeRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(MandateRoleTypeController), nameof(MandateRoleTypeController.Get), new { id = internalMessage.MandateRoleTypeId });
    }
}
