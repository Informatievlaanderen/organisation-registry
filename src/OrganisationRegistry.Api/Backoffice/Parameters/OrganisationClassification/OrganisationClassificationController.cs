namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassification;

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
[OrganisationRegistryRoute("organisationclassifications")]
public class OrganisationClassificationController : OrganisationRegistryController
{
    public OrganisationClassificationController(
        ICommandSender commandSender)
        : base(commandSender)
    {
    }

    /// <summary>Get a list of available organisation classifications.</summary>
    [HttpGet]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
    }

    /// <summary>Get an organisation classification.</summary>
    /// <response code="200">If the organisation classification is found.</response>
    /// <response code="404">If the organisation classification cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var organisationClassification = await context.OrganisationClassificationList.FirstOrDefaultAsync(x => x.Id == id);

        if (organisationClassification == null)
            return NotFound();

        return Ok(organisationClassification);
    }
}
