namespace OrganisationRegistry.Api.Search;

using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
using ElasticSearch;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using ElasticSearch.Configuration;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Infrastructure;
using Infrastructure.Search;
using Infrastructure.Search.Pagination;
using Infrastructure.Search.Sorting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrganisationRegistry.Infrastructure.Infrastructure.Json;
using Osc;
using SortOrder = Infrastructure.Search.Sorting.SortOrder;

[ApiVersion("1.0")]
[AdvertiseApiVersions("1.0")]
[OrganisationRegistryRoute("search")]
[ApiController]
[ApiExplorerSettings(GroupName = "Zoeken")]
public class SearchController : OrganisationRegistryController
{
    private readonly IElasticSearchFacade _elasticSearchFacade;
    private readonly ILogger<SearchController> _log;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public SearchController(
        IElasticSearchFacade elasticSearchFacade,
        ILogger<SearchController> log,
        IHttpContextAccessor httpContextAccessor)
    {
        _elasticSearchFacade = elasticSearchFacade;
        _log = log;
        _httpContextAccessor = httpContextAccessor;
    }

    /// <summary>Entiteiten opzoeken.</summary>
    /// <remarks>Dit endpoint laat toe entiteiten op te vragen op de ElasticSearch indexes.
    /// <br />
    /// De volgende indexes zijn beschikbaar: <br />
    ///     - `organisations` <br />
    ///     - `people` <br />
    ///     - `bodies`
    /// <br /><br />
    /// Voor het ophalen van grote hoeveelheden data, gebruiken we de <b>scroll</b> functionaliteit.
    /// <br /><br />
    /// Maak een request zoals je normaal doet, maar maak geen gebruik van `offset` of `limit`.
    /// Gebruik in plaats daarvan `scroll=true`.
    /// <br />
    /// Dit geeft 500 resultaten die je gewoon kan verwerken. In aanvulling krijg je hierdoor ook een `http-header x-search-metadata` die een <b>scrollId</b> bevat, samen met nog wat andere info. (deze header is een json-string).
    /// <br /><br />
    /// Na deze request heb je 30 seconden de tijd om een call te doen naar `v1/search/{INDEXNAME}/scroll?id={SCROLLID}`
    /// Deze zal je de volgende pagina geven (opnieuw 500 items), samen met de `x-search-metadata` header en een nieuwe <b>scrollId</b>.
    /// Herhaal dit proces tot je geen nieuwe items meer krijgt.
    /// <br /><br />
    /// <b>Omgaan met data wijzigingen (incrementele synchronisatie)</b>
    /// <br />
    /// Om enkel gewijzigde data op te halen sinds je laatste synchronisatie, kan je het `changeId` veld gebruiken:
    /// <br /><br />
    /// 1. Sla na elke synchronisatie het hoogste `changeId` op dat je hebt ontvangen
    /// <br />
    /// 2. Bij de volgende synchronisatie, gebruik je dit nummer + 1 als startpunt in je query:
    /// <br />
    /// `https://api.wegwijs.vlaanderen.be/v1/search/people?q=changeId:[43626 TO *]&amp;sort=changeId,id&amp;scroll=true`
    /// <br /><br />
    /// 3. Verwerk alle resultaten via de scroll API (zie hierboven)
    /// <br />
    /// 4. Sla opnieuw het hoogste `changeId` op uit de ontvangen resultaten
    /// <br />
    /// 5. Herhaal dit proces bij elke synchronisatie
    /// <br /><br />
    /// Op deze manier haal je enkel de records op die gewijzigd zijn sinds je laatste synchronisatie.
    /// </remarks>
    /// <param name="indexName">ElasticSearch index naam.
    /// Keuze tussen `organisations`, `people`, and `bodies`.</param>
    /// <param name="elastic"></param>
    /// <param name="elasticSearchConfiguration"></param>
    /// <param name="q">ElasticSearch querystring.</param>
    /// <param name="offset">Startpunt van de zoekresultaten (voor paginering).</param>
    /// <param name="limit">Aantal resultaten, 100 indien niet meegegeven (voor paginering).</param>
    /// <param name="fields">Veldnamen die in respons zullen zitten.</param>
    /// <param name="sort">Sortering van de resultaten.</param>
    /// <param name="scroll">Maak gebruik van de scrolling functionaliteit.</param>
    [HttpGet("{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetApiSearch(
        string indexName,
        [FromServices] Elastic elastic,
        [FromServices] IOptions<ElasticSearchConfiguration> elasticSearchConfiguration,
        [FromQuery] string q,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromQuery] string fields,
        [FromQuery] string sort,
        [FromQuery] bool? scroll)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest();

        _log.LogDebug(
            "[{IndexName}] Searched for '{SearchTerm}' (Offset: {Offset}, Limit: {Limit}, Fields: {Fields}, Sort: {Sort}, Scroll: {Scroll})",
            indexName,
            q,
            offset,
            limit,
            fields,
            sort,
            scroll);

        var esFacade = _elasticSearchFacade;

        return indexName.ToLower() switch
        {
            ElasticSearchFacade.OrganisationsIndexName => MapWithoutBankAccounts(await esFacade.SearchOrganisations(elastic, q, offset, limit, fields, sort, scroll)),
            ElasticSearchFacade.PeopleIndexName => await esFacade.GetSearch<PersonDocument>(elastic, q, offset, limit, fields, sort, scroll),
            ElasticSearchFacade.BodiesIndexName => await esFacade.GetSearch<BodyDocument>(elastic, q, offset, limit, fields, sort, scroll),
            _ => default(ISearchResponse<IDocument>),
        } is { } searchResult
            ? ToIActionResult(searchResult.ThrowIfInvalid(_log))
            : NotFound();
    }

    /// <summary>Entiteiten opzoeken (zoekbalk).</summary>
    /// <param name="indexName">ElasticSearch index naam.
    /// Keuze tussen `organisations`, `people`, and `bodies`.</param>
    /// <param name="elastic"></param>
    /// <param name="q">ElasticSearch querystring.</param>
    /// <param name="offset">Startpunt van de zoekresultaten (voor paginering).</param>
    /// <param name="limit">Aantal resultaten, 100 indien niet meegegeven (voor paginering).</param>
    /// <param name="fields">Veldnamen die in respons zullen zitten.</param>
    /// <param name="sort">Sortering van de resultaten.</param>
    /// <param name="scroll">Maak gebruik van de scrolling functionaliteit.</param>
    [HttpGet("box/{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> GetSearch(
        string indexName,
        [FromServices] Elastic elastic,
        [FromQuery] string q,
        [FromQuery] int offset,
        [FromQuery] int limit,
        [FromQuery] string fields,
        [FromQuery] string sort,
        [FromQuery] bool? scroll)
    {
        if (string.IsNullOrWhiteSpace(q))
            return BadRequest();

        _log.LogDebug(
            "[{IndexName}] Searched for '{SearchTerm}' (Offset: {Offset}, Limit: {Limit}, Fields: {Fields}, Sort: {Sort}, Scroll: {Scroll})",
            indexName,
            q,
            offset,
            limit,
            fields,
            sort,
            scroll);

        var jsonSerializerSettings = GetJsonSerializerSettings();

        switch (indexName.ToLower())
        {
            case ElasticSearchFacade.OrganisationsIndexName:
            {
                var response = MapWithoutBankAccounts(await _elasticSearchFacade.SearchOrganisations(elastic, q, offset, limit, fields, sort, scroll));

                Response.AddElasticsearchMetaDataResponse(
                    new ElasticsearchMetaData<OrganisationDocument>(response));
                Response.AddPaginationResponse(
                    new PaginationInfo(
                        offset / limit + 1,
                        limit,
                        (int)response.Total,
                        (int)Math.Ceiling((double)response.Total / limit)));

                var sortPart = sort.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(x => x.Trim())
                    .First();

                var (part, sortOrder) = GetSorting(sortPart);
                Response.AddSortingResponse(part, sortOrder);

                var organisations = response.Hits.Select(x => x.Source);

                foreach (var organisation in organisations)
                {
                    if (organisation.Parents.IsNullOrEmpty())
                        continue;

                    organisation.Parents = organisation.Parents
                        .Where(
                            x => (!x.Validity.Start.HasValue || x.Validity.Start.Value <= DateTime.Now) &&
                                 (!x.Validity.End.HasValue || x.Validity.End.Value >= DateTime.Now))
                        .ToList();
                }

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(
                        organisations,
                        Formatting.Indented,
                        jsonSerializerSettings),
                };
            }

            case ElasticSearchFacade.PeopleIndexName: // Possibly not used
            {
                var response = await _elasticSearchFacade.GetSearch<PersonDocument>(elastic, q, offset - 1, limit, fields, sort, scroll);

                Response.AddElasticsearchMetaDataResponse(new ElasticsearchMetaData<PersonDocument>(response));
                Response.AddPaginationResponse(
                    new PaginationInfo(
                        offset,
                        limit,
                        (int)response.Total,
                        (int)Math.Ceiling((double)response.Total / limit)));

                var people = response.Hits.Select(x => x.Source);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(
                        people,
                        Formatting.Indented,
                        jsonSerializerSettings),
                };
            }

            case ElasticSearchFacade.BodiesIndexName: // Possibly not used
            {
                var response = await _elasticSearchFacade.GetSearch<BodyDocument>(elastic, q, offset - 1, limit, fields, sort, scroll);

                Response.AddElasticsearchMetaDataResponse(new ElasticsearchMetaData<BodyDocument>(response));
                Response.AddPaginationResponse(
                    new PaginationInfo(
                        offset,
                        limit,
                        (int)response.Total,
                        (int)Math.Ceiling((double)response.Total / limit)));

                var bodies = response.Hits.Select(x => x.Source);

                return new ContentResult
                {
                    ContentType = "application/json",
                    StatusCode = (int)HttpStatusCode.OK,
                    Content = JsonConvert.SerializeObject(
                        bodies,
                        Formatting.Indented,
                        jsonSerializerSettings),
                };
            }

            default:
                return NotFound();
        }
    }

    private static (string part, SortOrder sortOrder) GetSorting(string sortPart)
        => sortPart.StartsWith("-")
            ? (sortPart[1..], SortOrder.Descending)
            : (sortPart, SortOrder.Ascending);

    /// <summary>Entiteiten opzoeken.</summary>
    [HttpPost("{indexName}")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> PostApiSearch(
        string indexName,
        [FromServices] Elastic elastic,
        [FromBody] JObject q,
        [FromQuery] int? offset,
        [FromQuery] int? limit,
        [FromQuery] string fields,
        [FromQuery] string sort,
        [FromQuery] bool? scroll)
    {
        var searchTerm = q.ToString();
        if (string.IsNullOrWhiteSpace(searchTerm))
            return BadRequest();

        _log.LogDebug(
            "[{IndexName}] Searched for '{SearchTerm}' (Offset: {Offset}, Limit: {Limit}, Fields: {Fields}, Sort: {Sort}, Scroll: {Scroll})",
            indexName,
            searchTerm,
            offset,
            limit,
            fields,
            sort,
            scroll);

        return indexName.ToLower() switch
        {
            ElasticSearchFacade.OrganisationsIndexName => ToIActionResult(MapWithoutBankAccounts(await _elasticSearchFacade.PostApiSearchOrganisations(elastic, q, offset, limit, fields, sort, scroll))),
            ElasticSearchFacade.PeopleIndexName => ToIActionResult(await _elasticSearchFacade.PostApiSearch<PersonDocument>(elastic, q, offset, limit, fields, sort, scroll)),
            ElasticSearchFacade.BodiesIndexName => ToIActionResult(await _elasticSearchFacade.PostApiSearch<BodyDocument>(elastic, q, offset, limit, fields, sort, scroll)),
            _ => NotFound(),
        };
    }

    /// <summary>Entiteiten opzoeken (scrolling).</summary>
    /// <remarks>Dit endpoint laat toe om volgende resultaten op te halen met behulp van de scroll api.
    /// Hiervoor moet de vorige call uitgevoerd zijn met `scroll=true`.
    /// <br /><br />
    /// Na deze request heb je 30 seconden de tijd om een call te doen naar `v1/search/{INDEXNAME}/scroll?id={SCROLLID}`
    /// Deze zal je de volgende pagina geven (opnieuw 500 items), samen met de `x-search-metadata` header en een nieuwe <b>scrollId</b>.
    /// Herhaal dit proces tot je geen nieuwe items meer krijgt.
    /// </remarks>
    /// <param name="indexName">ElasticSearch index naam.
    /// Keuze tussen `organisations`, `people`, and `bodies`.</param>
    /// <param name="elastic"></param>
    /// <param name="id">Elasticsearch scroll id.</param>
    [HttpGet("{indexName}/scroll")]
    [ProducesResponseType(StatusCodes.Status200OK)]
    public async Task<IActionResult> ScrollApiSearch(
        string indexName,
        [FromServices] Elastic elastic,
        [FromQuery] string id)
    {
        if (string.IsNullOrWhiteSpace(id))
            return BadRequest();

        _log.LogDebug("[{IndexName}] Scrolling for '{ScrollId}'", indexName, id);

        return indexName.ToLower() switch
        {
            ElasticSearchFacade.OrganisationsIndexName => ToIActionResult(MapWithoutBankAccounts(await _elasticSearchFacade.ScrollApiSearch<OrganisationDocument>(elastic, id))),
            ElasticSearchFacade.PeopleIndexName => ToIActionResult(await _elasticSearchFacade.ScrollApiSearch<PersonDocument>(elastic, id)),
            ElasticSearchFacade.BodiesIndexName => ToIActionResult(await _elasticSearchFacade.ScrollApiSearch<BodyDocument>(elastic, id)),
            _ => NotFound(),
        };
    }

    private ISearchResponse<OrganisationDocument> MapWithoutBankAccounts(ISearchResponse<OrganisationDocument> searchOrganisations)
    {
        foreach (var organisationsHit in searchOrganisations.Hits)
        {
            organisationsHit.Source.BankAccounts = [];
        }

        return searchOrganisations;
    }

    private static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var jsonSerializerSettings = JsonSerializerSettingsProvider
            .CreateSerializerSettings()
            .ConfigureForOrganisationRegistry();

        jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

        var maybeResolver = (OrganisationRegistryContractResolver?)jsonSerializerSettings.ContractResolver;
        if (maybeResolver is not { } resolver)
            throw new NullReferenceException("Resolver should not be null");

        resolver.SetStringDefaultValueToEmptyString = true;
        resolver.RemoveEmptyArrays = true;

        return jsonSerializerSettings;
    }

    private IActionResult ToIActionResult<T>(ISearchResponse<T> searchResults)
        where T : class, IDocument
    {
        var jsonSerializerSettings = GetJsonSerializerSettings();

        _httpContextAccessor.HttpContext?.Response.AddElasticsearchMetaDataResponse(new ElasticsearchMetaData<T>(searchResults));

        return new ContentResult
        {
            ContentType = "application/json",
            StatusCode = (int)HttpStatusCode.OK,
            Content = JsonConvert.SerializeObject(
                searchResults.Hits.Select(x => x.Source),
                Formatting.Indented,
                jsonSerializerSettings),
        };
    }
}
