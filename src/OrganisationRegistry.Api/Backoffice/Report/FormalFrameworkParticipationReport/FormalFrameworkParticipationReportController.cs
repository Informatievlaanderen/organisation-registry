namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkParticipationReport
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
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Configuration;
    using OrganisationRegistry.SqlServer.Infrastructure;

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
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
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
                return await OkAsync(participations);

            var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

            Response.AddPaginationResponse(
                new PaginationInfo(
                    pagination.RequestedPage,
                    pagination.ItemsPerPage,
                    participations.Count,
                    (int)Math.Ceiling((double)participations.Count / pagination.ItemsPerPage)));

            return await OkAsync(
                participations
                    .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                    .Take(pagination.ItemsPerPage)
                    .ToList());
        }

        /// <summary>
        /// Get gender ratio summary (grouped by body, organisation and bodyseat)
        /// </summary>
        /// <param name="context"></param>
        /// <param name="apiConfiguration"></param>
        /// <param name="dateTimeProvider"></param>
        /// <returns></returns>
        [HttpGet("participationsummary")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> GetParticipationSummary(
            [FromServices] OrganisationRegistryContext context,
            [FromServices] IOptions<ApiConfigurationSection> apiConfiguration,
            [FromServices] IDateTimeProvider dateTimeProvider)
        {
            var sorting = Request.ExtractSortingRequest();

            var participations =
                ParticipationSummary.Sort(
                        ParticipationSummary.Map(
                            await ParticipationSummary.Search(context, apiConfiguration.Value, dateTimeProvider.Today)),
                        sorting)
                    .ToList();

            Response.AddSortingResponse(sorting.SortBy, sorting.SortOrder);

            var possiblePagination = Request.ExtractPaginationRequest();

            if (possiblePagination is NoPaginationRequest)
                return await OkAsync(participations);

            var pagination = possiblePagination as PaginationRequest ?? new PaginationRequest(1, 10);

            Response.AddPaginationResponse(
                new PaginationInfo(
                    pagination.RequestedPage,
                    pagination.ItemsPerPage,
                    participations.Count,
                    (int)Math.Ceiling((double)participations.Count / pagination.ItemsPerPage)));

            return await OkAsync(
                participations
                    .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                    .Take(pagination.ItemsPerPage)
                    .ToList());
        }
    }
}
