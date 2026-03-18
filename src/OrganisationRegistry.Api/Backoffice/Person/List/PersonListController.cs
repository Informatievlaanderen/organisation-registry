namespace OrganisationRegistry.Api.Backoffice.Person.List;

using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Infrastructure;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("people")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonListController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van personen op.</summary>
    /// <response code="200">Een lijst van personen.</response>
    [HttpGet]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context)
    {
        var filtering = Request.ExtractFilteringRequest<PersonListItemFilter>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPersons = new PersonListQuery(context).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPersons.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPersons.Items.ToListAsync());
    }
}
