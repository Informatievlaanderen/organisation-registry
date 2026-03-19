namespace OrganisationRegistry.Api.Backoffice.Person.Function;

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
[OrganisationRegistryRoute("people/{personId}/functions")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Personen")]
public class PersonFunctionController : OrganisationRegistryController
{
    /// <summary>Vraag een lijst van functies voor een persoon op.</summary>
    /// <response code="200">Een lijst van functies voor een persoon.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PersonFunctionListItem>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(PersonFunctionListExamples))]
    public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid personId)
    {
        var filtering = Request.ExtractFilteringRequest<PersonFunctionListItem>();
        var sorting = Request.ExtractSortingRequest();
        var pagination = Request.ExtractPaginationRequest();

        var pagedPeople = new PersonFunctionListQuery(context, personId).Fetch(filtering, sorting, pagination);

        Response.AddPaginationResponse(pagedPeople.PaginationInfo);
        Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

        return Ok(await pagedPeople.Items.ToListAsync());
    }
}
