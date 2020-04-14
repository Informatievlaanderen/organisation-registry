namespace OrganisationRegistry.Api.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Responses;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class BodyParticipationReportController : OrganisationRegistryController
    {
        public BodyParticipationReportController(
            ICommandSender commandSender) : base(commandSender)
        {
        }

        /// <summary>
        /// Get gender ratio for a body (grouped by body and bodyseat)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bodyId">A body GUID identifier</param>
        /// <returns></returns>
        [HttpGet("bodyparticipation/{bodyId}")]
        [ProducesResponseType(typeof(IEnumerable<BodyParticipation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBodyParticipation(
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyParticipationFilter>();
            var sorting = Request.ExtractSortingRequest();

            var participations =
                BodyParticipation.Sort(
                        BodyParticipation.Map(
                            BodyParticipation.Search(
                                context,
                                bodyId,
                                filtering)),
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

        /// <summary>
        /// Get gender ratio totals for a body (grouped by body)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="bodyId">A body GUID identifier</param>
        /// <returns></returns>
        [HttpGet("bodyparticipation/{bodyId}/totals")]
        [ProducesResponseType(typeof(BodyParticipationTotals), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetBodyParticipationTotals(
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] Guid bodyId)
        {
            var filtering = Request.ExtractFilteringRequest<BodyParticipationFilter>();

            var participationTotals =
                BodyParticipationTotals.Map(
                        BodyParticipationTotals.Search(
                            context,
                            bodyId,
                            filtering))
                    .ToList();

            return Ok(participationTotals.FirstOrDefault() ?? new BodyParticipationTotals());
        }
    }
}
