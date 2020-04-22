namespace OrganisationRegistry.Api.Report
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure;
    using Infrastructure.Search.Pagination;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Mvc;
    using Responses;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.Commands;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("reports")]
    public class FormalFrameworkParticipationReportController : OrganisationRegistryController
    {
        public FormalFrameworkParticipationReportController(
            ICommandSender commandSender) : base(commandSender)
        {
        }

        /// <summary>
        /// Get gender ratio for a formalframework (grouped by body, organisation and bodyseat)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="formalFrameworkId">A formal framework GUID identifier</param>
        /// <returns></returns>
        [HttpGet("formalframeworkparticipation/{formalFrameworkId}")]
        [ProducesResponseType(typeof(IEnumerable<FormalFrameworkParticipation>), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(NotFoundResult), (int)HttpStatusCode.NotFound)]
        [ProducesResponseType(typeof(BadRequestResult), (int)HttpStatusCode.BadRequest)]
        public async Task<IActionResult> GetFormalFrameworkParticipation(
            [FromServices] OrganisationRegistryContext context,
            [FromRoute] Guid formalFrameworkId)
        {
            var sorting = Request.ExtractSortingRequest();

            var participations =
                FormalFrameworkParticipation.Sort(
                        FormalFrameworkParticipation.Map(
                            FormalFrameworkParticipation.Search(
                                context,
                                formalFrameworkId)),
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
