namespace OrganisationRegistry.Api.Backoffice.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using ElasticSearch.Client;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Parameters.OrganisationClassification.Queries;
    using Responses;
    using Search;
    using SqlServer.Infrastructure;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class OrganisationClassificationReportController : OrganisationRegistryController
    {
        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        private readonly ILogger<SearchController> _log;
        private readonly ApiConfiguration _config;

        public OrganisationClassificationReportController(
            ICommandSender commandSender,
            IOptions<ApiConfiguration> config,
            ILogger<SearchController> log) : base(commandSender)
        {
            _log = log;
            _config = config.Value;
        }

        /// <summary>
        /// Get all organisations and their labels for a classification of classificationtype "Beleidsdomein".
        /// </summary>
        /// <param name="elastic"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("classificationorganisations/{id}")]
        [ProducesResponseType(typeof(IEnumerable<ClassificationOrganisation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetClassificationOrganisations(
            [FromServices] Elastic elastic,
            [FromRoute] Guid id)
        {
            var sorting = Request.ExtractSortingRequest();

            var orderedResults =
                ClassificationOrganisation.Sort(
                        ClassificationOrganisation.Map(
                            await ClassificationOrganisation.Search(
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

        /// <summary>
        /// Get all classifications for classificationtype "Beleidsdomein".
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [HttpGet("policydomainclassifications")]
        [ProducesResponseType(typeof(PagedQueryable<OrganisationClassificationListQueryResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrganisationClassificationsForPolicyDomain(
            [FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            //  Set classificationtype GUID for "Beleidsdomein"
            filtering.Filter.OrganisationClassificationTypeId = _config.PolicyDomainClassificationTypeId;
            filtering.Filter.OrganisationClassificationTypeName = string.Empty;

            var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
        }

        /// <summary>
        /// Get all classifications for classificationtype "Bevoegde minister".
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        [HttpGet("responsibleministerclassifications")]
        [ProducesResponseType(typeof(PagedQueryable<OrganisationClassificationListQueryResult>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetOrganisationClassificationsForResponsibleMinister(
            [FromServices] OrganisationRegistryContext context)
        {
            var filtering = Request.ExtractFilteringRequest<OrganisationClassificationListItemFilter>();
            var sorting = Request.ExtractSortingRequest();
            var pagination = Request.ExtractPaginationRequest();

            //  Set classificationtype GUID for "Beleidsdomein"
            filtering.Filter.OrganisationClassificationTypeId = _config.ResponsibleMinisterClassificationTypeId;
            filtering.Filter.OrganisationClassificationTypeName = string.Empty;

            var pagedOrganisationClassifications = new OrganisationClassificationListQuery(context).Fetch(filtering, sorting, pagination);

            Response.AddPaginationResponse(pagedOrganisationClassifications.PaginationInfo);
            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            return Ok(await pagedOrganisationClassifications.Items.ToListAsync());
        }

        /// <summary>
        /// Get gender ratio for a classification organisation (grouped by body)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="dateTimeProvider"></param>
        /// <param name="classificationOrganisationId">A classification organisation GUID identifier</param>
        /// <returns></returns>
        [HttpGet("classificationorganisationsparticipation/{classificationOrganisationId}")]
        [ProducesResponseType(typeof(IEnumerable<ClassificationOrganisationParticipation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetClassificationOrganisationParticipation(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IDateTimeProvider dateTimeProvider,
            [FromRoute] Guid classificationOrganisationId)
        {
            var sorting = Request.ExtractSortingRequest();

            var participations =
                ClassificationOrganisationParticipation.Sort(
                        ClassificationOrganisationParticipation.Map(
                            ClassificationOrganisationParticipation.Search(
                                context,
                                classificationOrganisationId,
                                _config,
                                dateTimeProvider.Today)),
                        sorting)
                    .ToList();

            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            var possiblePagination = Request.ExtractPaginationRequest();

            if (possiblePagination is NoPaginationRequest)
                return Ok(participations);

            var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

            Response.AddPaginationResponse(
                new PaginationInfo(
                    pagination.RequestedPage,
                    pagination.ItemsPerPage,
                    participations.Count,
                    (int)Math.Ceiling((double)participations.Count / pagination.ItemsPerPage)));

            return Ok(
                participations
                    .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                    .Take(pagination.ItemsPerPage)
                    .ToList());
        }
    }
}
