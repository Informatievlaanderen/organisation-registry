namespace OrganisationRegistry.Api.Backoffice.Management.DelegationAssignments;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Api.Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Api.Infrastructure.Security;
using OrganisationRegistry.Api.Infrastructure.Swagger.Examples;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.SqlServer.DelegationAssignments;
using OrganisationRegistry.SqlServer.Infrastructure;
using Queries;
using Responses;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("manage/delegations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class DelegationAssignmentController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van delegatieopdrachten op.</summary>
    /// <response code="200">Een lijst van delegatieopdrachten.</response>
    [HttpGet("{delegationId}/assignments")]
    [OrganisationRegistryAuthorize(RequiredPermissions = [Permission.DelegationsRead])]
    [ProducesResponseType(typeof(List<DelegationAssignmentListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(DelegationAssignmentListExamples))]
    [ActionName("List")]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid delegationId)
    {
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

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
    [OrganisationRegistryAuthorize(RequiredPermissions = [Permission.DelegationsRead])]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid delegationId,
        Guid id)
    {
        var delegationAssignment = await context.DelegationAssignmentList.FirstOrDefaultAsync(x => x.Id == id);

        if (delegationAssignment == null)
            return NotFound();

        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == delegationId);

        if (delegation == null)
            return NotFound();

        return Ok(new DelegationAssignmentResponse(delegationAssignment));
    }
}
