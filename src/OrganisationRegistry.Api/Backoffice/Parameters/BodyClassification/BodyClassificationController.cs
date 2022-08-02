namespace OrganisationRegistry.Api.Backoffice.Parameters.BodyClassification;

using System;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Commands;
using Queries;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("bodyclassifications")]
public class BodyClassificationController : OrganisationRegistryController
{
    public BodyClassificationController(ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available body classifications.</summary>
    [HttpGet]
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

    /// <summary>Get an body classification.</summary>
    /// <response code="200">If the body classification is found.</response>
    /// <response code="404">If the body classification cannot be found.</response>
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
