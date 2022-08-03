﻿namespace OrganisationRegistry.Api.Backoffice.Organisation.Label;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/labels")]
public class OrganisationLabelController : OrganisationRegistryController
{
    /// <summary>Get a list of available labels for an organisation.</summary>
    [HttpGet]
    [OrganisationRegistryAuthorize]
    [AllowAnonymous]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationLabelListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForLabelType = labelTypeId =>
            LabelPolicy.ForCreate(
                    memoryCaches.OvoNumbers[organisationId],
                    memoryCaches.UnderVlimpersManagement.Contains(organisationId),
                    configuration,
                    labelTypeId)
                .Check(user)
                .IsSuccessful;

        var pagedOrganisations = new OrganisationLabelListQuery(context, organisationId, isAuthorizedForLabelType).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Get a label for an organisation.</summary>
    /// <response code="200">If the label is found.</response>
    /// <response code="404">If the label cannot be found.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationLabelList.FirstOrDefaultAsync(x => x.OrganisationLabelId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
