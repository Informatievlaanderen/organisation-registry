namespace OrganisationRegistry.Api.PowerBI;

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using System.Runtime.CompilerServices;
using System.Threading;
using ElasticSearch;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using ElasticSearch.Configuration;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.FeatureManagement.Mvc;
using Newtonsoft.Json;

[FeatureGate(FeatureFlags.PowerBIApi)]
[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("powerbi/search")]
[ApiController]
[ApiExplorerSettings(GroupName = "PowerBI")]
public class PBISearchController : OrganisationRegistryController
{
    private readonly ILogger<PBISearchController> _logger;

    public PBISearchController(ILogger<PBISearchController> logger)
    {
        _logger = logger;
    }

    /// <summary>Organisaties exporteren (als bestand).</summary>
    /// <remarks>Dit endpoint laat toe een export bestand te maken van alle organisaties op een ElasticSearch index.
    /// <br />
    /// Het resultaat is een json-bestand.
    /// <br /><br />
    /// </remarks>
    /// <param name="elastic"></param>
    /// <param name="elasticSearchConfiguration"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("organisations")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult StreamOrganisations(
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration,
        CancellationToken cancellationToken)
    {
        var esFacade = new ElasticSearchFacade(HttpContext, _logger, elasticSearchConfiguration.Value);
        return ToFile(SearchOrganisations(esFacade, elastic, cancellationToken), "organisations_zoekresultaten.json", _logger);
    }

    /// <summary>Personen exporteren (als bestand).</summary>
    /// <remarks>Dit endpoint laat toe een export bestand te maken van alle personen op een ElasticSearch index.
    /// <br />
    /// Het resultaat is een json-bestand.
    /// <br /><br />
    /// </remarks>
    /// <param name="elastic"></param>
    /// <param name="elasticSearchConfiguration"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("people")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult StreamPeople(
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration,
        CancellationToken cancellationToken)
    {
        var esFacade = new ElasticSearchFacade(HttpContext, _logger, elasticSearchConfiguration.Value);
        return ToFile(SearchPeople(esFacade, elastic, cancellationToken), "people_zoekresultaten.json", _logger);
    }

    /// <summary>Organen exporteren (als bestand).</summary>
    /// <remarks>Dit endpoint laat toe een export bestand te maken van alle organen op een ElasticSearch index.
    /// <br />
    /// Het resultaat is een json-bestand.
    /// <br /><br />
    /// </remarks>
    /// <param name="elastic"></param>
    /// <param name="elasticSearchConfiguration"></param>
    /// <param name="cancellationToken"></param>
    [HttpGet("bodies")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult StreamBodies(
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration,
        CancellationToken cancellationToken)
    {
        var esFacade = new ElasticSearchFacade(HttpContext, _logger, elasticSearchConfiguration.Value);
        return ToFile(SearchBodies(esFacade, elastic, cancellationToken), "bodies_zoekresultaten.json", _logger);
    }

    private static FileCallbackResult ToFile(IAsyncEnumerable<IDocument> searchResult, string fileName, ILogger logger)
        => new(
            new MediaTypeHeaderValue("application/json"),
            async (outputStream, _) =>
            {
                logger.LogDebug("Start streaming");
                var streamWriter = new StreamWriter(outputStream)
                {
                    AutoFlush = true,
                };

                await streamWriter.WriteLineAsync("["); // write start of json to stream


                await foreach (var (document, index) in searchResult.Select((value, index) => (value, index)))
                {
                    if (index > 0)
                        await streamWriter.WriteLineAsync(",");

                    var serializedDocument = JsonConvert.SerializeObject(document);
                    await streamWriter.WriteAsync(serializedDocument);
                    await streamWriter.FlushAsync();
                }

                await streamWriter.WriteLineAsync("]"); // write end of json to stream

                logger.LogDebug("Finished streaming");
            })
        {
            FileDownloadName = fileName,
        };

    private static async IAsyncEnumerable<IDocument> SearchBodies(
        ElasticSearchFacade esFacade,
        Elastic elastic,
       [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var maybeScrollResult = await esFacade.SearchBodiesWithDefaultScrolling(elastic, null!, null!, "id", cancellationToken);

        if (maybeScrollResult is not { } scrollResult)
            yield break;

        var lastOvoNumber = Guid.Empty;
        while (scrollResult.Documents.Any())
        {
            foreach (var document in scrollResult.Documents)
            {
                yield return document;
                lastOvoNumber = document.Id;
            }

            scrollResult = await esFacade.ScrollSearch<BodyDocument>(elastic, scrollResult.ScrollId, cancellationToken);

            if (scrollResult.IsValid) continue;

            maybeScrollResult = await esFacade.SearchBodiesWithDefaultScrolling(elastic, $"id:{{{lastOvoNumber} TO *]", null!, "id", cancellationToken);
            if (maybeScrollResult is { } newScrollResult)
                scrollResult = newScrollResult;
        }
    }

    private static async IAsyncEnumerable<IDocument> SearchPeople(
        ElasticSearchFacade esFacade,
        Elastic elastic,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var maybeScrollResult = await esFacade.SearchPeopleWithDefaultScrolling(elastic, null!, null!, "id", cancellationToken);

        if (maybeScrollResult is not { } scrollResult)
            yield break;

        while (scrollResult.Documents.Any())
        {
            var lastid = Guid.Empty;
            foreach (var document in scrollResult.Documents)
            {
                yield return document;
                lastid = document.Id;
            }

            scrollResult = await esFacade.ScrollSearch<PersonDocument>(elastic, scrollResult.ScrollId, cancellationToken);

            if (scrollResult.IsValid) continue;

            maybeScrollResult = await esFacade.SearchPeopleWithDefaultScrolling(elastic, $"id:{{{lastid.ToString()} TO *]", null!, "id", cancellationToken);
            if (maybeScrollResult is { } newScrollResult)
                scrollResult = newScrollResult;
        }
    }

    private static async IAsyncEnumerable<IDocument> SearchOrganisations(
        ElasticSearchFacade esFacade,
        Elastic elastic,
        [EnumeratorCancellation] CancellationToken cancellationToken)
    {
        var maybeScrollResult = await esFacade.SearchOrganisationsWithDefaultScrolling(elastic, null!, null!, "ovoNumber", cancellationToken);

        if (maybeScrollResult is not { } scrollResult)
            yield break;

        var lastOvoNumber = "";
        while (scrollResult.Documents.Any())
        {
            foreach (var document in scrollResult.Documents)
            {
                yield return document;
                lastOvoNumber = document.OvoNumber;
            }

            scrollResult = await esFacade.ScrollSearch<OrganisationDocument>(elastic, scrollResult.ScrollId, cancellationToken);

            if (scrollResult.IsValid) continue;

            maybeScrollResult = await esFacade.SearchOrganisationsWithDefaultScrolling(elastic, $"ovoNumber:{{{lastOvoNumber} TO *]", null!, "ovoNumber", cancellationToken);
            if (maybeScrollResult is { } newScrollResult)
                scrollResult = newScrollResult;
        }
    }
}
