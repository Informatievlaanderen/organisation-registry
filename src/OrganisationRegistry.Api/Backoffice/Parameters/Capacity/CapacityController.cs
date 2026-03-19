namespace OrganisationRegistry.Api.Backoffice.Parameters.Capacity;

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
using SqlServer.Capacity;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("capacities")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class CapacityController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van hoedanigheden op.</summary>
    /// <response code="200">Een lijst van hoedanigheden.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(CapacityListExamples))]
    [ProducesResponseType(typeof(List<CapacityListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<CapacityListQuery.CapacityListFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedCapacities = new CapacityListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedCapacities.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedCapacities.Items.ToListAsync());
    }

    /// <summary>Vraag een hoedanigheid op.</summary>
    /// <response code="200">Als de hoedanigheid gevonden is.</response>
    /// <response code="404">Als de hoedanigheid niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.CapacityList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
