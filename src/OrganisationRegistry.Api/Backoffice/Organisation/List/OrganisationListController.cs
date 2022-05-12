namespace OrganisationRegistry.Api.Backoffice.Organisation.List
{
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Commands;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("organisations")]
    public class OrganisationListController : OrganisationRegistryController
    {
        public OrganisationListController(ICommandSender commandSender) : base(commandSender)
        {
        }

        /// <summary>Get a list of available organisations.</summary>
        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> Get(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] ISecurityService securityService)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            var securityInformation = await securityService.GetSecurityInformation(User);

            var pagedOrganisations =
                new OrganisationListQuery(context, securityInformation)
                    .Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisations.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisations.Items.ToListAsync());
        }
    }
}
