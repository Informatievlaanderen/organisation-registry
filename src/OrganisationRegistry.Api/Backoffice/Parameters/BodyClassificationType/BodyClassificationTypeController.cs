namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassificationType;

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
using SqlServer.BodyClassificationType;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodyclassificationtypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BodyClassificationTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van orgaanclassificatietypes op.</summary>
    /// <response code="200">Een lijst van orgaanclassificatietypes.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BodyClassificationTypeListExamples))]
    [ProducesResponseType(typeof(List<BodyClassificationTypeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BodyClassificationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyClassificationTypes = new BodyClassificationTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyClassificationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyClassificationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een orgaanclassificatietype op.</summary>
    /// <response code="200">Als het orgaanclassificatietype gevonden is.</response>
    /// <response code="404">Als het orgaanclassificatietype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.BodyClassificationTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
