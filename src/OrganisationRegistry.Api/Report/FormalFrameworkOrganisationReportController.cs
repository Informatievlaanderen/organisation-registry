namespace OrganisationRegistry.Api.Report
{
    using Configuration;
    using ElasticSearch.Client;
    using Infrastructure;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Options;
    using Responses;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class FormalFrameworkOrganisationReportController : OrganisationRegistryController
    {
        private readonly ApiConfiguration _config;

        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        public FormalFrameworkOrganisationReportController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config) : base(commandSender)
        {
            _config = config.Value;
        }

        /// <summary>
        /// Get all organisations for a formal framework.
        /// </summary>
        /// <param name="elastic"></param>
        /// <param name="id">A formal framework GUID identifier</param>
        /// <returns></returns>
        [HttpGet("formalframeworkorganisations/{id}")]
        [ProducesResponseType(typeof(IEnumerable<FormalFrameworkOrganisationBase>), (int) HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int) HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int) HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFormalFrameworkOrganisations(
            [FromServices] Elastic elastic,
            [FromRoute] Guid id)
        {
            var sorting = Request.ExtractSortingRequest();

            var orderedResults =
                FormalFrameworkOrganisation.Sort(
                        FormalFrameworkOrganisation.Map(
                            await FormalFrameworkOrganisation.Search(
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
                    (int) Math.Ceiling((double) orderedResults.Count / pagination.ItemsPerPage)));

            return Ok(
                orderedResults
                    .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                    .Take(pagination.ItemsPerPage)
                    .ToList());
        }

        /// <summary>
        /// Get all organisations for a formal framework.
        /// </summary>
        /// <param name="elastic"></param>
        /// <param name="dateTimeProvider"></param>
        /// <param name="id">A formal framework GUID identifier</param>
        /// <returns></returns>
        [HttpGet("formalframeworkorganisations/{id}/extended")]
        [ProducesResponseType(typeof(IEnumerable<FormalFrameworkOrganisationExtended>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFormalFrameworkOrganisationsExtended(
            [FromServices] Elastic elastic,
            [FromServices] IDateTimeProvider dateTimeProvider,
            [FromRoute] Guid id)
        {
            var sorting = Request.ExtractSortingRequest();

            var orderedResults =
                FormalFrameworkOrganisation.Sort(
                        FormalFrameworkOrganisation.MapExtended(
                            await FormalFrameworkOrganisation.Search(
                                elastic.ReadClient,
                                id,
                                ScrollSize,
                                ScrollTimeout),
                            id,
                            _config,
                            dateTimeProvider.Today),
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

        /// <summary>
        /// Get all organisations for a formal framework.
        /// </summary>
        /// <param name="elastic"></param>
        /// <param name="id">A formal framework GUID identifier</param>
        /// <returns></returns>
        [HttpGet("formalframeworkorganisations/vademecum/{id}")]
        [ProducesResponseType(typeof(IEnumerable<FormalFrameworkOrganisation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFormalFrameworkOrganisationsVademecum(
            [FromServices] Elastic elastic,
            [FromRoute] Guid id)
        {
            var sorting = Request.ExtractSortingRequest();

            var orderedResults =
                FormalFrameworkOrganisationVademecum.Sort(
                        FormalFrameworkOrganisationVademecum.Map(
                            await FormalFrameworkOrganisationVademecum.Search(
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
