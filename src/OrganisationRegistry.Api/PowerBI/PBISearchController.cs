namespace OrganisationRegistry.Api.PowerBI;

using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http.Headers;
using ElasticSearch;
using ElasticSearch.Client;
using ElasticSearch.Configuration;
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
    /// <param name="q">ElasticSearch querystring. (optioneel, indien niet voorzien wordt alles teruggegeven)</param>
    /// <param name="fields">Veldnamen die in respons zullen zitten. (optioneel, indien niet voorzien wordt alles teruggegeven)</param>
    /// <param name="sort">Sortering van de resultaten. (optioneel, indien niet voorzien wordt standaard sortering toegepast)</param>
    [HttpGet("{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public IActionResult GetApiSearch(
        [FromRoute] string indexName,
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration,
        [FromQuery] string q,
        [FromQuery] string fields,
        [FromQuery] string sort)
        => new FileCallbackResult(new MediaTypeHeaderValue("application/json"),
            async (outputStream, _) =>
            {
                _logger.LogDebug("Start streaming");
                var streamWriter = new StreamWriter(outputStream)
                {
                    AutoFlush = true,
                };

                await streamWriter.WriteLineAsync("["); // write start of json to stream

                var esFacade = new ElasticSearchFacade(HttpContext, _logger,elasticSearchConfiguration.Value);

                await foreach (var (document, index) in Search(esFacade, indexName, elastic, q, fields, sort).Select((value, index) => (value, index)))
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
        Elastic elastic,
        string q,
        string fields,
        string sort)
    {
        var maybeScrollResult = await esFacade.SearchWithDefaultScrolling(indexName, elastic, q, fields, sort);

        if (maybeScrollResult is not { } scrollResult)
            yield break;

        while (scrollResult.Documents.Any())
        {
            if (!scrollResult.IsValid)
            {
                scrollResult = await esFacade.ScrollSearch(elastic, indexName, maybeScrollResult.ScrollId);
                continue;
            }

            foreach (var document in scrollResult.Documents)
                yield return document;

            scrollResult = await esFacade.ScrollSearch(elastic, indexName, maybeScrollResult.ScrollId);
        }
    }
}
