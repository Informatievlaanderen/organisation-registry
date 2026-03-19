namespace OrganisationRegistry.Api.Backoffice.Parameters.RegulationSubTheme;

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
using SqlServer.RegulationSubTheme;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("regulationsubthemes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class RegulationSubThemeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van regelgevingsubthema's op.</summary>
    /// <response code="200">Een lijst van regelgevingsubthema's.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(RegulationSubThemeListExamples))]
    [ProducesResponseType(typeof(List<RegulationSubThemeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<RegulationSubThemeListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedRegulationSubThemes = new RegulationSubThemeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedRegulationSubThemes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedRegulationSubThemes.Items.ToListAsync());
    }

    /// <summary>Vraag een regelgevingsubthema op.</summary>
    /// <response code="200">Als het regelgevingsubthema gevonden is.</response>
    /// <response code="404">Als het regelgevingsubthema niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var regulationSubTheme = await context.RegulationSubThemeList.FirstOrDefaultAsync(x => x.Id == id);

        if (regulationSubTheme == null)
            return NotFound();

        return Ok(regulationSubTheme);
    }
}
