namespace OrganisationRegistry.Api.Backoffice.Body.FormalFramework;

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
[OrganisationRegistryRoute("bodies/{bodyId}/formalframeworks")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organen")]
public class BodyFormalFrameworkController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van toepassingsgebieden voor een orgaan op.</summary>
    /// <response code="200">Een lijst van toepassingsgebieden voor een orgaan.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BodyFormalFrameworkListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BodyFormalFrameworkListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyFormalFrameworkListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyFormalFrameworks = new BodyFormalFrameworkListQuery(context, bodyId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyFormalFrameworks.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyFormalFrameworks.Items.ToListAsync());
    }

    /// <summary>Vraag een toepassingsgebied voor een orgaan op.</summary>
    /// <response code="200">Als het toepassingsgebied gevonden is.</response>
    /// <response code="404">Als het toepassingsgebied niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid bodyId, [FromRoute] Guid id)
    {
        var bodyFormalFramework = await context.BodyFormalFrameworkList.FirstOrDefaultAsync(x => x.BodyId == bodyId && x.BodyFormalFrameworkId == id);

        if (bodyFormalFramework == null)
            return NotFound();

        return Ok(bodyFormalFramework);
    }
}
