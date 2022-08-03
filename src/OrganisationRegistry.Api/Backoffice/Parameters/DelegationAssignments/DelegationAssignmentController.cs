namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using Queries;
using Responses;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.DecentraalBeheerder)]
[OrganisationRegistryRoute("manage/delegations")]
public class DelegationAssignmentController : OrganisationRegistryController
{
    /// <summary>Get a list of available delegation assignments.</summary>
    [HttpGet("{delegationId}/assignments")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromServices] ISecurityService securityService, [FromRoute] Guid delegationId)
    {
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        if (!await securityService.CanEditDelegation(User, delegation.OrganisationId, delegation.BodyId))
            return Unauthorized(); // ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze delegatie.");

        var filtering = Request.ExtractFilteringRequest<DelegationAssignmentListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedDelegationAssignments =
            new DelegationAssignmentListQuery(context, delegationId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedDelegationAssignments.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedDelegationAssignments.Items.ToListAsync());
    }

    /// <summary>Get a delegation assignment.</summary>
    /// <response code="200">If the delegation assignment is found.</response>
    /// <response code="404">If the delegation assignment cannot be found.</response>
    [HttpGet("{delegationId}/assignments/{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid delegationId,
        Guid id)
    {
        var delegationAssignment = await context.DelegationAssignmentList.FirstOrDefaultAsync(x => x.Id == id);

        if (delegationAssignment == null)
            return NotFound();

        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        if (!await securityService.CanEditDelegation(User, delegation.OrganisationId, delegation.BodyId))
            return Unauthorized(); // ModelState.AddModelError("NotAllowed", "U hebt niet voldoende rechten voor deze delegatie.");

        return Ok(new DelegationAssignmentResponse(delegationAssignment));
    }
}
