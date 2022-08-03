namespace OrganisationRegistry.Api.Backoffice.Organisation.Function;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/functions")]
[OrganisationRegistryAuthorize]
public class OrganisationFunctionCommandController : OrganisationRegistryCommandController
{
    public OrganisationFunctionCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a function for an organisation.</summary>
    /// <response code="201">If the function is created, together with the location.</response>
    /// <response code="400">If the function information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationFunctionRequest message)
    {
        var internalMessage = new AddOrganisationFunctionInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationFunctionRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationFunctionController), nameof(OrganisationFunctionController.Get), new { id = message.OrganisationFunctionId });
    }

    /// <summary>Update a function for an organisation.</summary>
    /// <response code="201">If the function is updated, together with the location.</response>
    /// <response code="400">If the function information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromBody] UpdateOrganisationFunctionRequest message)
    {
        var internalMessage = new UpdateOrganisationFunctionInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationFunctionRequestMapping.Map(internalMessage));

        return Ok();
    }

    /// <summary>Remove a function from an organisation</summary>
    /// <response code="204">If the function is removed.</response>
    /// <response code="400">If the function is not found for the organisation.</response>
    [HttpDelete("{id:guid}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete([FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        await CommandSender.Send(new RemoveOrganisationFunction(new OrganisationId(organisationId), id));
        return NoContent();
    }
}
