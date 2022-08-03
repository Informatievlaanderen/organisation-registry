namespace OrganisationRegistry.Api.Backoffice.Organisation.Regulation;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/regulations")]
public class OrganisationRegulationController : OrganisationRegistryController
{
    /// <summary>Get a list of available regulations for an organisation.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationRegulationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        var isAuthorizedForRegulation = () =>
            new RegulationPolicy()
                .Check(user)
                .IsSuccessful;

        var pagedOrganisations = new OrganisationRegulationListQuery(
                context,
                organisationId,
                isAuthorizedForRegulation)
            .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a regulation for an organisation.</summary>
    /// <response code="200">If the regulation is found.</response>
    /// <response code="404">If the regulation cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationRegulationList
            .SingleOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationRegulationId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
