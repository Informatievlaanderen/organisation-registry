namespace OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/openingHours")]
[OrganisationRegistryAuthorize]
public class OrganisationOpeningHourCommandController : OrganisationRegistryCommandController
{
    public OrganisationOpeningHourCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a new opening hour for an organisation.</summary>
    /// <response code="201">If the opening hour is created, together with the location.</response>
    /// <response code="400">If the opening hour does not pass validation.</response>
    /// <response code="404">If the organisation cannot be found.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromRoute] Guid organisationId,
        [FromBody] AddOrganisationOpeningHourRequest message)
    {
        var internalMessage = new AddOrganisationOpeningHourInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddOrganisationOpeningHourRequestMapping.Map(internalMessage));

        return CreatedWithLocation(nameof(OrganisationOpeningHourController), nameof(OrganisationOpeningHourController.Get), new { id = message.OrganisationOpeningHourId });
    }

    /// <summary>Update an existing opening hour for an organisation.</summary>
    /// <response code="200">If the opening hour is updated.</response>
    /// <response code="400">If the opening hour does not pass validation.</response>
    /// <response code="404">If the opening hour or organisation cannot be found.</response>
    [HttpPut("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid organisationId,
        [FromBody] UpdateOrganisationOpeningHourRequest message)
    {
        var internalMessage = new UpdateOrganisationOpeningHourInternalRequest(organisationId, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationOpeningHourRequestMapping.Map(internalMessage));

        return Ok();
    }
}
