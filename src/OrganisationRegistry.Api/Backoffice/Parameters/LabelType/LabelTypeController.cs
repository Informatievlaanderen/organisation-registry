namespace OrganisationRegistry.Api.Backoffice.Parameters.LabelType;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.LabelType;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("labeltypes")]
public class LabelTypeController : OrganisationRegistryController
{
    /// <summary>Get a list of available label types.</summary>
    [HttpGet]
    [OrganisationRegistryAuthorize]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromQuery] Guid? forOrganisationId)
    {
        var filtering = Request.ExtractFilteringRequest<LabelTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForLabelType = labelTypeId =>
            !forOrganisationId.HasValue ||
            LabelPolicy.ForCreate(
                    memoryCaches.OvoNumbers[forOrganisationId.Value],
                    memoryCaches.UnderVlimpersManagement.Contains(forOrganisationId.Value),
                    configuration,
                    labelTypeId)
                .Check(user)
                .IsSuccessful;

        var pagedLabelTypes = new LabelTypeListQuery(context, configuration, isAuthorizedForLabelType).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedLabelTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedLabelTypes.Items.ToListAsync());
    }

    /// <summary>Get a label type.</summary>
    /// <response code="200">If the label type is found.</response>
    /// <response code="404">If the label type cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context,
        [FromRoute] Guid id)
    {
        var key = await context.LabelTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
