namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using Requests;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize]
[OrganisationRegistryRoute("manage/delegations")]
public class DelegationAssignmentCommandController : OrganisationRegistryController
{
    public DelegationAssignmentCommandController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Create a delegation assignment for an organisation.</summary>
    /// <response code="201">If the delegation assignment is created, together with the location.</response>
    /// <response code="400">If the delegation assignment information does not pass validation.</response>
    [HttpPost("{delegationId}/assignments")]
    [ProducesResponseType(StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Post(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid delegationId,
        [FromBody] AddDelegationAssignmentRequest message)
    {
        var internalMessage = new AddDelegationAssignmentInternalRequest(delegationId, message);

        // TODO: Discuss, should we depend on a projection to check which OrganisationId a delegation belongs to?
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(AddDelegationAssignmentRequestMapping.Map(internalMessage, delegation.OrganisationId));

        return CreatedWithLocation(nameof(DelegationAssignmentController),nameof(DelegationAssignmentController.Get), new { delegationId, id = message.DelegationAssignmentId });
    }

    /// <summary>Update a delegation assignment.</summary>
    /// <response code="200">If the delegation assignment is updated, together with the location.</response>
    /// <response code="400">If the delegation assignment information does not pass validation.</response>
    [HttpPut("{delegationId}/assignments/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Put(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid delegationId,
        [FromRoute] Guid id,
        [FromBody] UpdateDelegationAssignmentRequest message)
    {
        var internalMessage = new UpdateDelegationAssignmentInternalRequest(delegationId, message);

        // TODO: Discuss, should we depend on a projection to check which OrganisationId a delegation belongs to?
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(UpdateDelegationAssignmentRequestMapping.Map(internalMessage,delegation.OrganisationId));

        return OkWithLocationHeader(nameof(DelegationAssignmentController),nameof(DelegationAssignmentController.Get), new { delegationId, id });
    }

    /// <summary>Remove a delegation assignment.</summary>
    /// <response code="200">If the delegation assignment is removed, together with the location.</response>
    /// <response code="400">If the delegation assignment information does not pass validation.</response>
    [HttpDelete("{delegationId}/assignments/{delegationAssignmentId}/{bodyId}/{bodySeatId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid delegationId,
        [FromRoute] Guid delegationAssignmentId,
        [FromRoute] Guid bodyId,
        [FromRoute] Guid bodySeatId)
    {
        var message = new RemoveDelegationAssignmentRequest
        {
            BodyId = bodyId,
            BodySeatId = bodySeatId,
            DelegationAssignmentId = delegationAssignmentId,
        };

        var internalMessage = new RemoveDelegationAssignmentInternalRequest(delegationId, message);

        // TODO: Discuss, should we depend on a projection to check which OrganisationId a delegation belongs to?
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        if (!TryValidateModel(internalMessage))
            return BadRequest(ModelState);

        await CommandSender.Send(RemoveDelegationAssignmentRequestMapping.Map(internalMessage));

        return OkWithLocationHeader(nameof(DelegationAssignmentController),nameof(DelegationAssignmentController.Get), new { delegationId, id = delegationAssignmentId });
    }
}
