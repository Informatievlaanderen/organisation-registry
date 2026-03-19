namespace OrganisationRegistry.Api.Backoffice.Parameters.DelegationAssignments;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using Queries;
using Responses;
using SqlServer.DelegationAssignments;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.DecentraalBeheerder)]
[OrganisationRegistryRoute("manage/delegations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class DelegationAssignmentController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van delegatieopdrachten op.</summary>
    /// <response code="200">Een lijst van delegatieopdrachten.</response>
    [HttpGet("{delegationId}/assignments")]
    [ProducesResponseType(typeof(List<DelegationAssignmentListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DelegationAssignmentListExamples))]
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

    /// <summary>Vraag een delegatieopdracht op.</summary>
    /// <response code="200">Als de toewijzing gevonden is.</response>
    /// <response code="404">Als de toewijzing niet gevonden kan worden.</response>
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
