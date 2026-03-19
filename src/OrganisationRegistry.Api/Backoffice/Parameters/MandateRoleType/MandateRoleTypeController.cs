namespace OrganisationRegistry.Api.Backoffice.Parameters.MandateRoleType;

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
using SqlServer.MandateRoleType;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("mandateroletypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class MandateRoleTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van mandaat rol types op.</summary>
    /// <response code="200">Een lijst van mandaat rol types.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(MandateRoleTypeListExamples))]
    [ProducesResponseType(typeof(List<MandateRoleTypeListItem>), StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<MandateRoleTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedMandateRoleTypes = new MandateRoleTypeListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedMandateRoleTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedMandateRoleTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een mandaat rol type op.</summary>
    /// <response code="200">Als het mandaat rol type gevonden is.</response>
    /// <response code="404">Als het mandaat rol type niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.MandateRoleTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
