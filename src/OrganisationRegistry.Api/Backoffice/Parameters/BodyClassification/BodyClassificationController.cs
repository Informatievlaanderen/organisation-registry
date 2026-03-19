namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification;

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
using SqlServer.BodyClassification;
using SqlServer.Infrastructure;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodyclassifications")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class BodyClassificationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van orgaanclassificaties op.</summary>
    /// <response code="200">Een lijst van orgaanclassificaties.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<BodyClassificationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(BodyClassificationListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<BodyClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedBodyClassifications = new BodyClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedBodyClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedBodyClassifications.Items.ToListAsync());
    }

    /// <summary>Vraag een orgaanclassificatie op.</summary>
    /// <response code="200">Als de orgaanclassificatie gevonden is.</response>
    /// <response code="404">Als de orgaanclassificatie niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var bodyClassification = await context.BodyClassificationList.FirstOrDefaultAsync(x => x.Id == id);

        if (bodyClassification == null)
            return NotFound();

        return Ok(bodyClassification);
    }
}
