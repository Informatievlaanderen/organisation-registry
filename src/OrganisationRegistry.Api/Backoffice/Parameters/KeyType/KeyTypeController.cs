namespace OrganisationRegistry.Api.Backoffice.Parameters.KeyType;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Handling.Authorization;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Queries;
using SqlServer.Infrastructure;
using SqlServer.KeyType;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("keytypes")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Parameters")]
public class KeyTypeController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van sleuteltypes op.</summary>
    /// <response code="200">Een lijst van sleuteltypes.</response>
    [HttpGet]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(KeyTypeListExamples))]
    [ProducesResponseType(typeof(List<KeyTypeListItem>), StatusCodes.Status200OK)]
    [OrganisationRegistryAuthorize]
    public async Task<IActionResult> Get(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IMemoryCaches memoryCaches,
        [FromServices] ISecurityService securityService,
        [FromServices] IOrganisationRegistryConfiguration configuration,
        [FromQuery] Guid? forOrganisationId)
    {
        var filtering = Request.ExtractFilteringRequest<KeyTypeListQuery.KeyTypeListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        filtering.Filter ??= new KeyTypeListQuery.KeyTypeListItemFilter();

        var user = await securityService.GetUser(User);
        Func<Guid, bool> isAuthorizedForKeyType = keyTypeId =>
            !forOrganisationId.HasValue ||
            new KeyPolicy(
                    memoryCaches.OvoNumbers[forOrganisationId.Value],
                    configuration,
                    keyTypeId)
                .Check(user)
                .IsSuccessful;

        var pagedKeyTypes = new KeyTypeListQuery(context, isAuthorizedForKeyType).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedKeyTypes.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedKeyTypes.Items.ToListAsync());
    }

    /// <summary>Vraag een sleuteltype op.</summary>
    /// <response code="200">Als het sleuteltype gevonden is.</response>
    /// <response code="404">Als het sleuteltype niet gevonden kan worden.</response>
    [HttpGet("{id}")]
    [OrganisationRegistryAuthorize]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid id)
    {
        var keyType = await context.KeyTypeList.FirstOrDefaultAsync(x => x.Id == id);

        if (keyType == null)
            return NotFound();

        return Ok(keyType);
    }
}
