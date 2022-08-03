namespace OrganisationRegistry.Api.Backoffice.Organisation.Regulation;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/regulations")]
[OrganisationRegistryAuthorize]
public class OrganisationRegulationCommandController : OrganisationRegistryCommandController
{
    public OrganisationRegulationCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a regulation for an organisation.</summary>
    /// <response code="201">If the regulation is created, together with the location.</response>
    /// <response code="400">If the regulation information does not pass validation.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromRoute] Guid organisationId,
        [FromBody] AddOrganisationRegulationRequest message)
    {
        var internalMessage = new AddOrganisationRegulationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationRegulationRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationRegulationController), nameof(OrganisationRegulationController.Get), new { id = message.OrganisationRegulationId });
    }

    /// <summary>Update a regulation for an organisation.</summary>
    /// <response code="201">If the regulation is updated, together with the location.</response>
    /// <response code="400">If the regulation information does not pass validation.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid organisationId,
        [FromBody] UpdateOrganisationRegulationRequest message)
    {
        var internalMessage = new UpdateOrganisationRegulationInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationRegulationRequestMapping.Map(internalMessage));

        return Ok();
    }
}
