namespace OrganisationRegistry.Api.Backoffice.Report.BodyParticipationReport;

using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.SqlServer.Infrastructure;
using Participation;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Rapporten")]
public class BodyParticipationReportController : OrganisationRegistryController
{
    /// <summary>Rapport: Meer Evenwichtige Participatie per orgaan per zetel.</summary>
    /// <remarks>Geef de geslachtsverhouding voor een orgaan (gegroepeerd per orgaan en zetel).</remarks>
    /// <param name="context"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="bodyId">A body GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Meer Evenwichtige Participatie per orgaan per zetel.</response>
    [HttpGet("bodyparticipation/{bodyId}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBodyParticipation(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IDateTimeProvider dateTimeProvider,
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
                            filtering,
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

        return await OkAsync(
            participations
                .Skip((pagination.RequestedPage - 1) * pagination.ItemsPerPage)
                .Take(pagination.ItemsPerPage)
                .ToList());
    }

    /// <summary>Rapport: Meer Evenwichtige Participatie per orgaan - totaal.</summary>
    /// <remarks>Geef de totale geslachtsverhouding voor een orgaan (gegroepeerd per orgaan).</remarks>
    /// <param name="context"></param>
    /// <param name="dateTimeProvider"></param>
    /// <param name="bodyId">A body GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Meer Evenwichtige Participatie per orgaan - totaal.</response>
    [HttpGet("bodyparticipation/{bodyId}/totals")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetBodyParticipationTotals(
        [FromServices] OrganisationRegistryContext context,
        [FromServices] IDateTimeProvider dateTimeProvider,
        [FromRoute] Guid bodyId)
    {
        var filtering = Request.ExtractFilteringRequest<BodyParticipationFilter>();

        var participationTotals =
            BodyParticipationTotals.Map(
                BodyParticipation.Search(
                    context,
                    bodyId,
                    filtering,
                    dateTimeProvider.Today));

        return await OkAsync(participationTotals);
    }
}
