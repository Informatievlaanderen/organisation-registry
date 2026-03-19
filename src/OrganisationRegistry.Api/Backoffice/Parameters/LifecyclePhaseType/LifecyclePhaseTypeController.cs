namespace OrganisationRegistry.Api.Backoffice.Parameters.LifecyclePhaseType;

using System;
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
using Queries;
using SqlServer.Infrastructure;
using SqlServer.LifecyclePhaseType;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("lifecyclephasetypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class LifecyclePhaseTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van levensloopfasetypes op.</summary>
    /// <response code="200">Een lijst van levensloopfasetypes.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(LifecyclePhaseTypeListExamples))]
    [ProducesResponseType(typeof(List<LifecyclePhaseTypeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<LifecyclePhaseTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedLifecyclePhaseTypes = new LifecyclePhaseTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLifecyclePhaseTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLifecyclePhaseTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een levensloopfasetype op.</summary>
    /// <response code="200">Als het levensloopfasetype gevonden is.</response>
    /// <response code="404">Als het levensloopfasetype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.LifecyclePhaseTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
