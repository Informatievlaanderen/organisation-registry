namespace OrganisationRegistry.Api.Backoffice.Person.Capacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Api.Infrastructure.Swagger.Examples;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Person;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("people/{personId}/capacities")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonCapacityController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van hoedanigheden voor een persoon op.</summary>
    /// <response code="200">Een lijst van hoedanigheden voor een persoon.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonCapacityListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PersonCapacityListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid personId)
    {
        var filtering = Request.ExtractFilteringRequest<PersonCapacityListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPeople = new PersonCapacityListQuery(context, personId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPeople.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPeople.Items.ToListAsync());
    }
}
