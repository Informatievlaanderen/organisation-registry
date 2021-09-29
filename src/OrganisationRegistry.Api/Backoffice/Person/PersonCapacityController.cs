namespace OrganisationRegistry.Api.Backoffice.Person
{
    using System;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Commands;
    using Queries;
    using SqlServer.Infrastructure;
    using SqlServer.Person;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("people/{personId}/capacities")]
    public class PersonCapacityController : OrganisationRegistryController
    {
        public PersonCapacityController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available capacities for a person.</summary>
        [HttpGet]
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
}
