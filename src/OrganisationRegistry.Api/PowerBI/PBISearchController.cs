namespace OrganisationRegistry.Api.PowerBI;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using ElasticSearch;
using ElasticSearch.Client;
using ElasticSearch.Configuration;
using ElasticSearch.Organisations;
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

    /// <summary>Entiteiten exporteren (als bestand).</summary>
    /// <remarks>Dit endpoint laat toe een export bestand te maken van alle entiteiten op een ElasticSearch index.
    /// <br />
    /// De volgende indices zijn beschikbaar: <br />
    ///     - `organisations` <br />
    ///     - `people` <br />
    ///     - `bodies`
    /// <br /><br />
    /// Het resultaat is een json-bestand.
    /// <br /><br />
    /// </remarks>
    /// <param name="indexName">ElasticSearch index naam.
    /// Keuze tussen `organisations`, `people`, and `bodies`.</param>
    /// <param name="elastic"></param>
    /// <param name="elasticSearchConfiguration"></param>
    [HttpGet("{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetApiSearch(
        [FromRoute] string indexName,
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration)
        => new FileCallbackResult(
            new MediaTypeHeaderValue("application/json"),
            async (outputStream, _) =>
            {
                _logger.LogDebug("Start streaming");
                var streamWriter = new StreamWriter(outputStream)
                {
                    AutoFlush = true,
                };

                await streamWriter.WriteLineAsync("["); // write start of json to stream

                var esFacade = new ElasticSearchFacade(HttpContext, _logger, elasticSearchConfiguration.Value);

                await foreach (var (document, index) in Search(esFacade, indexName, elastic).Select((value, index) => (value, index)))
                {
                    if (index > 0)
                        await streamWriter.WriteLineAsync(",");

                    var serializedDocument = JsonConvert.SerializeObject(document);
                    await streamWriter.WriteAsync(serializedDocument);
                }

                await streamWriter.WriteLineAsync("]"); // write end of json to stream

                _logger.LogDebug("Finished streaming");
            })
        {
            FileDownloadName = $"{indexName}_zoekresultaten.json",
        };

    private static async IAsyncEnumerable<IDocument> Search(
        ElasticSearchFacade esFacade,
        string indexName,
        Elastic elastic)
    {
        var maybeScrollResult = await esFacade.SearchWithDefaultScrolling(indexName, elastic, null!, null!, "ovoNumber");

        if (maybeScrollResult is not { } scrollResult)
            yield break;

        var lastOvoNumber = "";
        while (scrollResult.Documents.Any())
        {
            foreach (var document in scrollResult.Documents)
            {
                yield return document;
                lastOvoNumber = ((OrganisationDocument)document).OvoNumber;
            }

            scrollResult = await esFacade.ScrollSearch(elastic, indexName, scrollResult.ScrollId);

            if (scrollResult.IsValid) continue;

            maybeScrollResult = await esFacade.SearchWithDefaultScrolling(indexName, elastic, $"ovoNumber:{{{lastOvoNumber} TO *]", null!, "ovoNumber");
            if (maybeScrollResult is { } newScrollResult)
                scrollResult = newScrollResult;
        }
    }
}
