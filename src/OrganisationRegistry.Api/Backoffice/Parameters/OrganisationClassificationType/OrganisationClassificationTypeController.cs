namespace OrganisationRegistry.Api.Backoffice.Parameters.OrganisationClassificationType;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Security;
using OrganisationRegistry.Infrastructure.Authorization;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Configuration;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.OrganisationClassificationType;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisationclassificationtypes")]
[OrganisationRegistryAuthorize(RequiredPermissions = [Permission.ParametersOrganisationClassificationTypesRead, Permission.CanManageOrganisationClassifications])]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class OrganisationClassificationTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van organisatieclassificatietypes op.</summary>
    /// <response code="200">Een lijst van organisatieclassificatietypes.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationClassificationTypeListExamples))]
    [ProducesResponseType(typeof(List<OrganisationClassificationTypeListItem>), StatusCodes.Status200OK)]
    [ActionName("List")]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IOrganisationRegistryConfiguration organisationRegistryConfiguration,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromQuery] Guid? forOrganisationId)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationClassificationTypeListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForOrganisationClassificationType = organisationClassificationTypeId =>
            !forOrganisationId.HasValue ||
            new OrganisationClassificationTypePolicy(
                    memoryCaches.OvoNumbers[forOrganisationId.Value],
                    organisationClassificationTypeId)
                .Check(user)
                .IsSuccessful;

        var pagedOrganisationClassificationTypes =
            new OrganisationClassificationTypeListQuery(
                    context,
                    organisationRegistryConfiguration,
                    isAuthorizedForOrganisationClassificationType)
                .Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedOrganisationClassificationTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedOrganisationClassificationTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een organisatieclassificatietype op.</summary>
    /// <response code="200">Als het organisatieclassificatietype gevonden is.</response>
    /// <response code="404">Als het organisatieclassificatietype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var key = await context.OrganisationClassificationTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (key == null)
            return NotFound();

        return Ok(key);
    }
}
