namespace OrganisationRegistry.Api.Edit.Organisation.Contacts;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.FeatureManagement.Mvc;
using OrganisationRegistry.Infrastructure.Commands;

[FeatureGate(FeatureFlags.EditApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("edit/organisations/{organisationId:guid}/contacts")]
[ApiController]
[ApiExplorerSettings(GroupName = "OrganisationContacts")]
[Consumes("application/json")]
[Produces("application/json")]
[Authorize(AuthenticationSchemes = AuthenticationSchemes.EditApi, Policy = PolicyNames.OrganisationContacts)]
public class OrganisationContactsController : EditApiController
{
    public OrganisationContactsController(ICommandSender commandSender) : base(commandSender)
    {
    }

    [HttpPost]
    public async Task<IActionResult> Post([FromRoute] Guid organisationId, [FromBody] AddOrganisationContactRequest message)
    {
        if (!TryValidateModel(message))
            return BadRequest(ModelState);

        var command = AddOrganisationContactRequestMapping.Map(organisationId, message);
        await CommandSender.Send(command);

        return CreatedWithLocation(
            nameof(Backoffice.Organisation.Contact.OrganisationContactController),
            nameof(Backoffice.Organisation.Contact.OrganisationContactController.Get),
            new { id = command.OrganisationContactId });
    }

    [HttpPut("{organisationContactId}")]
    public async Task<IActionResult> Put([FromRoute] Guid organisationId, [FromRoute] Guid organisationContactId, [FromBody] UpdateOrganisationContactRequest message)
    {
        if (!TryValidateModel(message))
            return BadRequest(ModelState);

        var command = UpdateOrganisationContactRequestMapping.Map(organisationId, organisationContactId, message);
        await CommandSender.Send(command);

        return Ok();
    }
}
