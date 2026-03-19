namespace OrganisationRegistry.Api.Backoffice.Organisation.Capacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/capacities")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationCapacityController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van hoedanigheden voor een organisatie op.</summary>
    /// <response code="200">Een lijst van hoedanigheden voor een organisatie.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganisationCapacityListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationCapacityListExamples))]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationCapacityListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForCapacity = id =>
            new CapacityPolicy(
                    memoryCaches.OvoNumbers[organisationId],
                    configuration,
                    id)
                .Check(user)
                .IsSuccessful;

        var pagedOrganisations =
            new OrganisationCapacityListQuery(
                    context,
                    organisationId,
                    isAuthorizedForCapacity)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Vraag een hoedanigheid voor een organisatie op.</summary>
    /// <response code="200">Als de hoedanigheid gevonden is.</response>
    /// <response code="404">Als de hoedanigheid niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationCapacityList
            .FirstOrDefaultAsync(x =>
                x.OrganisationId == organisationId &&
                x.OrganisationCapacityId == id);

        if (organisation == null)
            return NotFound();

        return Ok(new OrganisationCapacityResponse(organisation));
    }
}
