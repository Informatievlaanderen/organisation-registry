namespace OrganisationRegistry.Api.Backoffice.Organisation.Kbo;

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
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
public class OrganisationKboController : OrganisationRegistryController
{
    /// <summary>Gets the termination status of an organisation coupled with the KBO.</summary>
    [HttpGet("{id}/kbo/{kboNumber}/termination")]
    [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer, Role.VlimpersBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTerminationStatus(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] string kboNumber,
        [FromRoute] Guid id)
    {
        var organisationTermination = await context.OrganisationTerminationList.SingleOrDefaultAsync(x => x.Id == id && x.KboNumber == kboNumber);

        if (organisationTermination == null)
            return Ok(OrganisationTerminationResponse.NotFound(id));

        return Ok(OrganisationTerminationResponse.FromListItem(organisationTermination));
    }

    /// <summary>Get a list of organisations to be terminated according to the KBO.</summary>
    [HttpGet("kbo/terminated")]
    [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationTerminationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFunctions = new OrganisationTerminationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFunctions.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFunctions.Items.ToListAsync());
    }
}
