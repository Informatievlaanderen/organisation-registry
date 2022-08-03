namespace OrganisationRegistry.Api.Backoffice.Parameters.FormalFrameworkCategory;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Queries;
using SqlServer.FormalFrameworkCategory;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("formalframeworkcategories")]
public class FormalFrameworkCategoryController : OrganisationRegistryController
{
    /// <summary>Get a list of available formal framework categories.</summary>
    [HttpGet]
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

    /// <summary>Get a formal framework category.</summary>
    /// <response code="200">If the formal framework category is found.</response>
    /// <response code="404">If the formal framework category cannot be found.</response>
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
