namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFrameworkCategory;

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
using SqlServer.FormalFrameworkCategory;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("formalframeworkcategories")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class FormalFrameworkCategoryController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van toepassingsgebiedcategorieën op.</summary>
    /// <response code="200">Een lijst van toepassingsgebiedcategorieën.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FormalFrameworkCategoryListExamples))]
    [ProducesResponseType(typeof(List<FormalFrameworkCategoryListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<FormalFrameworkCategoryListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFormalFrameworkCategories = new FormalFrameworkCategoryListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFormalFrameworkCategories.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFormalFrameworkCategories.Items.ToListAsync());
    }

    /// <summary>Vraag een toepassingsgebiedcategorie op.</summary>
    /// <response code="200">Als de toepassingsgebiedcategorie gevonden is.</response>
    /// <response code="404">Als de toepassingsgebiedcategorie niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var formalFrameworkCategory = await context.FormalFrameworkCategoryList.FirstOrDefaultAsync(x => x.Id == id);

        if (formalFrameworkCategory == null)
            return NotFound();

        return Ok(formalFrameworkCategory);
    }
}
