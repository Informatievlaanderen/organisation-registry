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
public class OrganisationClassificationTypeCommandController : OrganisationRegistryController
{
    public OrganisationClassificationTypeCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create an organisation classification type.</summary>
    /// <response code="201">If the organisation classificiation type is created, together with the location.</response>
    /// <response code="400">If the organisation classificiation type information does not pass validation.</response>
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

    /// <summary>Update an organisation classification type.</summary>
    /// <response code="200">If the organisation classification type is updated, together with the location.</response>
    /// <response code="400">If the organisation classification type information does not pass validation.</response>
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
