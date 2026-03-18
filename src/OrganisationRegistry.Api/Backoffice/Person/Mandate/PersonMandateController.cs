namespace OrganisationRegistry.Api.Backoffice.Person.Mandate;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Person;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("people/{personId}/mandates")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonMandateController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van mandaten voor een persoon op.</summary>
    /// <response code="200">Een lijst van mandaten voor een persoon.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid personId)
    {
        var filtering = Request.ExtractFilteringRequest<PersonMandateListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPeople = new PersonMandateListQuery(context, personId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPeople.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPeople.Items.ToListAsync());
    }
}
