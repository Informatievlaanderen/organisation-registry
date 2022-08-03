namespace OrganisationRegistry.Api.Backoffice.Person.Mandate;

using System;
using System.Threading.Tasks;
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
public class PersonMandateController : OrganisationRegistryController
{
    /// <summary>Get a list of available mandates for a person.</summary>
    [HttpGet]
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
