namespace OrganisationRegistry.Api.Backoffice.Organisation.Regulation;

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
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/regulations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationRegulationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van regelgevingen voor een organisatie op.</summary>
    /// <response code="200">Een lijst van regelgevingen voor een organisatie.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<OrganisationRegulationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationRegulationListExamples))]
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

    /// <summary>Vraag een regelgeving voor een organisatie op.</summary>
    /// <response code="200">Als de regelgeving gevonden is.</response>
    /// <response code="404">Als de regelgeving niet gevonden kan worden.</response>
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
