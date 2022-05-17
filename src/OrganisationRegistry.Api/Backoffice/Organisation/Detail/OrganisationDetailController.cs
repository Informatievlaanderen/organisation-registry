namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using Security;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
public class OrganisationDetailController : OrganisationRegistryController
{
    public OrganisationDetailController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Get an organisation.</summary>
    /// <response code="200">If the organisation is found.</response>
    /// <response code="404">If the organisation cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationDetail.FirstOrDefaultAsync(x => x.Id == id);

        if (organisation == null)
            return NotFound();

        return Ok(new OrganisationResponse(organisation));
    }

    /// <summary>Create an organisation.</summary>
    /// <response code="201">If the organisation is created, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPost]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] ISecurityService securityService,
        [FromBody] CreateOrganisationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        var authInfo = await HttpContext.GetAuthenticateInfoAsync();
        if (authInfo?.Principal == null || !authInfo.Principal.IsInRole(Roles.Developer))
            message.OvoNumber = string.Empty;

        if (!string.IsNullOrWhiteSpace(message.KboNumber))
        {
            if (!await securityService.CanAddOrganisation(User, message.ParentOrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(CreateOrganisationRequestMapping.MapToCreateKboOrganisation(message, User));
        }
        else
        {
            await CommandSender.Send(CreateOrganisationRequestMapping.Map(message));
        }

        return CreatedWithLocation(nameof(Get), new { id = message.Id });
    }

    /// <summary>Update an organisation.</summary>
    /// <response code="200">If the organisation is updated, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPut("{id}")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateOrganisationInfoRequest message)
    {
        var internalMessage = new UpdateOrganisationInfoInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationInfoRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Update the organisation info that is not limited by vlimpers.</summary>
    /// <response code="200">If the organisation is updated, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPut("{id}/limitedtovlimpers")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateOrganisationInfoLimitedToVlimpersRequest message)
    {
        var internalMessage = new UpdateOrganisationInfoLimitedToVlimpersInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationInfoLimitedToVlimpersRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Update the organisation info that is not limited by vlimpers.</summary>
    /// <response code="200">If the organisation is updated, together with the location.</response>
    /// <response code="400">If the organisation information does not pass validation.</response>
    [HttpPut("{id}/notlimitedtovlimpers")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromRoute] Guid id,
        [FromBody] UpdateOrganisationInfoNotLimitedToVlimpersRequest message)
    {
        var internalMessage = new UpdateOrganisationInfoNotLimitedToVlimpersInternalRequest(id, message);

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateOrganisationInfoNotLimitedToVlimpersRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Terminate an organisation.</summary>
    /// <response code="200">If the organisation is terminated.</response>
    [HttpPut("{id}/terminate")]
    [OrganisationRegistryAuthorize(Roles = Roles.AlgemeenBeheerder + "," + Roles.VlimpersBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Terminate([FromRoute] Guid id, [FromBody] OrganisationTerminationRequest message)
    {
        await CommandSender.Send(
            new TerminateOrganisation(
                new OrganisationId(id),
                message.DateOfTermination,
                message.ForceKboTermination));

        return Ok();
    }
}
