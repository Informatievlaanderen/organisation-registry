namespace OrganisationRegistry.Api.Organisation
{
    using Microsoft.AspNetCore.Mvc;
    using System.Threading.Tasks;
    using Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;
    using System;
    using SqlServer.Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Infrastructure.Search.Sorting;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Filtering;
    using Queries;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations/{organisationId}/bodies")]
    public class OrganisationBodyController : OrganisationRegistryController
    {
        public OrganisationBodyController(ICommandSender commandSender)
            : base(commandSender)
        {
        }

        /// <summary>Get a list of available bodies for an organisation.</summary>
        [HttpGet]
        public async Task<IActionResult> Get([FromServices] OrganisationRegistryContext context, [FromRoute] Guid organisationId)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationBodyListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var pagedOrganisations = new OrganisationBodyListQuery(context, organisationId).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }
    }
}
