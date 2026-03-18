namespace OrganisationRegistry.Api.Backoffice.Organisation.OrganisationClassification;

using System;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations/{organisationId}/classifications")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationOrganisationClassificationController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van classificaties voor een organisatie op.</summary>
    [HttpGet]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromRoute] Guid organisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationOrganisationClassificationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForOrganisationClassificationType = id =>
            new OrganisationClassificationTypePolicy(
                    memoryCaches.OvoNumbers[organisationId],
                    configuration,
                    id)
                .Check(user)
                .IsSuccessful;

        var pagedOrganisations =
            new OrganisationOrganisationClassificationListQuery(
                    context,
                    organisationId,
                    isAuthorizedForOrganisationClassificationType)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisations.Items.ToListAsync());
    }

    /// <summary>Vraag een classificatie voor een organisatie op.</summary>
    /// <response code="200">Als de classificatie gevonden is.</response>
    /// <response code="404">Als de classificatie niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId, [FromRoute] Guid id)
    {
        var organisation = await context.OrganisationOrganisationClassificationList.FirstOrDefaultAsync(x => x.OrganisationId == organisationId && x.OrganisationOrganisationClassificationId == id);

        if (organisation == null)
            return NotFound();

        return Ok(organisation);
    }
}
