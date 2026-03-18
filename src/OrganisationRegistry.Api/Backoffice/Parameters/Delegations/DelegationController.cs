namespace OrganisationRegistry.Api.Backoffice.Parameters.Delegations;

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
[OrganisationRegistryAuthorize(Role.AlgemeenBeheerder , Role.DecentraalBeheerder)]
[OrganisationRegistryRoute("manage/delegations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class DelegationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van delegaties op.</summary>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromServices] ISecurityService securityService)
    {
        var filtering = Request.ExtractFilteringRequest<DelegationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var securityInformation = await securityService.GetSecurityInformation(User);

        var pagedDelegations =
            new DelegationListQuery(context, securityInformation).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedDelegations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedDelegations.Items.ToListAsync());
    }

    /// <summary>Vraag een delegatie op.</summary>
    /// <response code="200">Als de delegatie gevonden is.</response>
    /// <response code="404">Als de delegatie niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] ISecurityService securityService, [FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var delegation = await context.DelegationList.FirstOrDefaultAsync(x => x.Id == id);

        if (delegation == null)
            return NotFound();

        if (!await securityService.CanEditDelegation(User, delegation.OrganisationId, delegation.BodyId))
            return Unauthorized();

        return Ok(new DelegationResponse(delegation));
    }
}
