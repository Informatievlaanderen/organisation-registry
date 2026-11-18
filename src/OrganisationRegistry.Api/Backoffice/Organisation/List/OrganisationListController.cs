namespace OrganisationRegistry.Api.Backoffice.Organisation.List;

using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationListController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van organisaties op.</summary>
    /// <response code="200">Een lijst van organisaties.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganisationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationListExamples))]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);

        var pagedOrganisations =
            new OrganisationListQuery(context, user)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Vraag een lijst van organisaties op voor het overzicht op de hoofdpagina.</summary>
    /// <remarks>Gelijk aan <c>GET /v1/organisations</c>, met één bijkomend filterveld:
    /// <c>code</c> zoekt in het OVO-nummer, het KBO-nummer en de Orafin-sleutelwaarde van een
    /// organisatie. Leest uit een eigen read model, zodat de hoofdpagina losstaat van de
    /// generieke organisatielijst.</remarks>
    /// <response code="200">Een lijst van organisaties.</response>
    [HttpGet("overview")]
    [ProducesResponseType(typeof(List<OrganisationSearchListQueryResult>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationListExamples))]
    public async Task<IActionResult> GetOverview(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] ISecurityService securityService,
        [FromServices] IOrganisationRegistryConfiguration configuration)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationSearchListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);

        var pagedOrganisations =
            new OrganisationSearchListQuery(context, user, configuration)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }
}
