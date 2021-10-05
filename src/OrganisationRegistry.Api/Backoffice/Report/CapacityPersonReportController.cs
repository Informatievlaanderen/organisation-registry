namespace OrganisationRegistry.Api.Backoffice.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using ElasticSearch.Client;
    using Infrastructure;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Responses;
    using Search;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class CapacityPersonReportController: OrganisationRegistryController
    {
        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        private readonly ILogger<SearchController> _log;
        private readonly ApiConfiguration _config;

        public CapacityPersonReportController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config,
            ILogger<SearchController> log) : base(commandSender)
        {
            _log = log;
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
