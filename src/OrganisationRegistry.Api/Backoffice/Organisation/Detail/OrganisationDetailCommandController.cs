namespace OrganisationRegistry.Api.Backoffice.Organisation.Detail;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Organisation;
using Security;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationDetailCommandController : OrganisationRegistryCommandController
{
    public OrganisationDetailCommandController(ICommandSender commandSender) : base(commandSender)
    {
    }

    /// <summary>Registreer een organisatie.</summary>
    /// <response code="201">Als de organisatie succesvol aangemaakt is.</response>
    /// <response code="400">Als de validatie voor de organisatie mislukt is.</response>
    [HttpPost]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] ISecurityService securityService,
        [FromBody] CreateOrganisationRequest message)
    {
        if (!ModelState.IsValid)
            return BadRequest(ModelState);

        //TODO can be removed ??
        var authInfo = await HttpContext.GetAuthenticateInfoAsync();
        if (authInfo?.Principal == null || !authInfo.Principal.IsInRole(RoleMapping.Map(Role.Developer)))
            message.OvoNumber = string.Empty;

        if (message.KboNumber is { } kboNumber && kboNumber.IsNotEmptyOrWhiteSpace())
        {
            if (!await securityService.CanAddOrganisation(User, message.ParentOrganisationId))
                ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze organisatie.");

            await CommandSender.Send(CreateOrganisationRequestMapping.MapToCreateKboOrganisation(message, kboNumber));
        }
        else
        {
            await CommandSender.Send(CreateOrganisationRequestMapping.Map(message));
        }

        return CreatedWithLocation(nameof(OrganisationDetailController),nameof(OrganisationDetailController.Get), new { id = message.Id });
    }

    /// <summary>Pas een organisatie aan.</summary>
    /// <response code="200">Als de organisatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de organisatie mislukt is.</response>
    [HttpPut("{id}")]
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

        return OkWithLocationHeader(nameof(OrganisationDetailController),nameof(OrganisationDetailController.Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Pas de organisatiegegevens aan die beperkt zijn tot Vlimpers.</summary>
    /// <response code="200">Als de organisatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de organisatie mislukt is.</response>
    [HttpPut("{id}/limitedtovlimpers")]
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

        return OkWithLocationHeader(nameof(OrganisationDetailController),nameof(OrganisationDetailController.Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Pas de organisatiegegevens aan die niet beperkt zijn tot Vlimpers.</summary>
    /// <response code="200">Als de organisatie succesvol aangepast is.</response>
    /// <response code="400">Als de validatie voor de organisatie mislukt is.</response>
    [HttpPut("{id}/notlimitedtovlimpers")]
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

        return OkWithLocationHeader(nameof(OrganisationDetailController),nameof(OrganisationDetailController.Get), new { id = internalMessage.OrganisationId });
    }

    /// <summary>Beëindig een organisatie.</summary>
    /// <response code="200">Als de organisatie beëindigd is.</response>
    [HttpPut("{id}/terminate")]
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
