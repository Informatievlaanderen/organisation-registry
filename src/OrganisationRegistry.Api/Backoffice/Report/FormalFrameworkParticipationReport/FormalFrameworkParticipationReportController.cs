namespace OrganisationRegistry.Api.Backoffice.Report.FormalFrameworkParticipationReport;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Infrastructure;
using OrganisationRegistry.Api.Infrastructure.Search.Pagination;
using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer.Infrastructure;
using Infrastructure.Swagger.Examples;
using Swashbuckle.AspNetCore.Filters;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("reports")]
[ApiController]
[ApiExplorerSettings(GroupName = "Scherm APIs: Rapporten")]
public class FormalFrameworkParticipationReportController : OrganisationRegistryController
{
    /// <summary>Rapport: Meer Evenwichtige Participatie per toepassingsgebied.</summary>
    /// <remarks>Geef de meer evenwichtige participatie voor een toepassingsgebied (gegroepeerd per orgaan, organisatie en zetel).</remarks>
    /// <param name="context"></param>
    /// <param name="formalFrameworkId">A formal framework GUID identifier</param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Meer Evenwichtige Participatie per toepassingsgebied.</response>
    [HttpGet("formalframeworkparticipation/{formalFrameworkId}")]
    [ProducesResponseType(typeof(List<FormalFrameworkParticipation>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(FormalFrameworkParticipationListExamples))]
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

    /// <summary>Rapport: Meer Evenwichtige Participatie - samenvatting.</summary>
    /// <remarks>Geef de samenvatting van de meer evenwichtige participatie (gegroepeerd per orgaan, organisatie en zetel).</remarks>
    /// <param name="context"></param>
    /// <param name="apiConfiguration"></param>
    /// <param name="dateTimeProvider"></param>
    /// <returns></returns>
    /// <response code="200">Het rapport: Meer Evenwichtige Participatie - samenvatting.</response>
    [HttpGet("participationsummary")]
    [ProducesResponseType(typeof(List<ParticipationSummary>), StatusCodes.Status200OK)]
    [SwaggerResponseExample(StatusCodes.Status200OK, typeof(ParticipationSummaryListExamples))]
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
