namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationRelationType;

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
using SqlServer.OrganisationRelationType;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisationrelationtypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class OrganisationRelationTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van organisatierelatietypes op.</summary>
    /// <response code="200">Een lijst van organisatierelatietypes.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationRelationTypeListExamples))]
    [ProducesResponseType(typeof(List<OrganisationRelationTypeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationRelationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationRelationTypes = new OrganisationRelationTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationRelationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationRelationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een organisatierelatietype op.</summary>
    /// <response code="200">Als het organisatierelatietype gevonden is.</response>
    /// <response code="404">Als het organisatierelatietype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.OrganisationRelationTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
