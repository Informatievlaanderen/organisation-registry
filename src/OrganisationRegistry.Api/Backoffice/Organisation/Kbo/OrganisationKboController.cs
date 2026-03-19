namespace OrganisationRegistry.Api.Backoffice.Organisation.Kbo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Infrastructure;
using Infrastructure.Search.Filtering;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Infrastructure.Security;
using Infrastructure.Swagger.Examples;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using SqlServer.Infrastructure;
using SqlServer.Organisation;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("organisations")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Organisaties")]
public class OrganisationKboController : OrganisationRegistryController
{
    /// <summary>Vraag de beëindigingsstatus van een organisatie gekoppeld aan de KBO op.</summary>
    /// <response code="200">De beëindigingsstatus van de organisatie gekoppeld aan de KBO.</response>
    [HttpGet("{id}/kbo/{kboNumber}/termination")]
    [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer, Role.VlimpersBeheerder)]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetTerminationStatus(
        [FromServices] OrganisationRegistryContext context,
        [FromRoute] string kboNumber,
        [FromRoute] Guid id)
    {
        var organisationTermination = await context.OrganisationTerminationList.SingleOrDefaultAsync(x => x.Id == id && x.KboNumber == kboNumber);

        if (organisationTermination == null)
            return Ok(OrganisationTerminationResponse.NotFound(id));

        return Ok(OrganisationTerminationResponse.FromListItem(organisationTermination));
    }

    /// <summary>Vraag een lijst van te beëindigen organisaties volgens de KBO op.</summary>
    /// <response code="200">Een lijst van te beëindigen organisaties volgens de KBO.</response>
    [HttpGet("kbo/terminated")]
    [OrganisationRegistryAuthorize(Role.AlgemeenBeheerder, Role.CjmBeheerder, Role.Developer)]
    [ProducesResponseType(typeof(List<OrganisationTerminationListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(OrganisationTerminationListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<OrganisationTerminationListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedFunctions = new OrganisationTerminationListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedFunctions.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedFunctions.Items.ToListAsync());
    }
}
