namespace OrganisationRegistry.Api.Backoffice.Report.CapacityPersonReport
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Infrastructure;
    using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using ElasticSearch.Client;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class CapacityPersonReportController: OrganisationRegistryController
    {
        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        private readonly ApiConfigurationSection _config;

        public CapacityPersonReportController(
            ICommandSender commandSender,
            IOptions<ApiConfigurationSection> config) : base(commandSender)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Get all persons for a capacity.
        /// </summary>
        /// <param name="elastic"></param>
        /// <param name="id">A capacity GUID identifier</param>
        /// <returns></returns>
        [HttpGet("capacitypersons/{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetCapacityPersons(
            [FromServices] Elastic elastic,
            [FromRoute] Guid id)
        {
            var sorting = Request.ExtractSortingRequest();

            var orderedResults =
                CapacityPerson.Sort(
                        CapacityPerson.Map(
                            await CapacityPerson.Search(
                                elastic.ReadClient,
                                id,
                                ScrollSize,
                                ScrollTimeout),
                            id,
                            _config),
                        sorting)
                    .ToList();

            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            var possiblePagination = Request.ExtractPaginationRequest();

            if (possiblePagination is NoPaginationRequest)
                return Ok(orderedResults);

            var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

            Response.AddPaginationResponse(
                new PaginationInfo(
                    pagination.RequestedPage,
                    pagination.ItemsPerPage,
                    orderedResults.Count,
                    (int)Math.Ceiling((double)orderedResults.Count / pagination.ItemsPerPage)));

            return Ok(
                orderedResults
                    .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                    .Take(pagination.ItemsPerPage)
                    .ToList());
        }
    }
}
