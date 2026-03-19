namespace OrganisationRegistry.Api.Backoffice.Body.LifecyclePhase;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Api.Infrastructure.Swagger.Examples;
using OrganisationRegistry.SqlServer.Body;
using OrganisationRegistry.SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodies/{bodyId}/lifecyclephases")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyLifecyclePhaseController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van levensloopfasen voor een orgaan op.</summary>
    /// <response code="200">Een lijst van levensloopfasen voor een orgaan.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BodyLifecyclePhaseListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BodyLifecyclePhaseListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyLifecyclePhaseListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyLifecyclePhases = new BodyLifecyclePhaseListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyLifecyclePhases.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyLifecyclePhases.Items.ToListAsync());
    }

    /// <summary>Vraag een levensloopfase voor een orgaan op.</summary>
    /// <response code="200">Als de levensloopfase gevonden is.</response>
    /// <response code="404">Als de levensloopfase niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyLifecyclePhase = await context.BodyLifecyclePhaseList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyLifecyclePhaseId == id);

        if (bodyLifecyclePhase == null)
            return NotFound();

        return Ok(bodyLifecyclePhase);
    }
}
