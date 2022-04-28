namespace OrganisationRegistry.Api.Search
{
    using ElasticSearch.Bodies;
    using ElasticSearch.Client;
    using ElasticSearch.Common;
    using ElasticSearch.Organisations;
    using ElasticSearch.People;
    using Infrastructure;
    using Infrastructure.Search;
    using Infrastructure.Security;
    using Microsoft.AspNetCore.Mvc;
    using Microsoft.Extensions.Logging;
    using Osc;
    using Newtonsoft.Json;
    using Newtonsoft.Json.Linq;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Net;
    using System.Threading.Tasks;
    using Infrastructure.Helpers;
    using Infrastructure.Search.Pagination;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Infrastructure.Json;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using ElasticSearch;
    using Infrastructure.Search.Sorting;
    using Microsoft.AspNetCore.Http;
    using SortOrder = Osc.SortOrder;

    [ApiVersion("1.0")]
    [AdvertiseApiVersions("1.0")]
    [OrganisationRegistryRoute("search")]
    [ApiController]
    [ApiExplorerSettings(GroupName = "Zoeken")]
    public class SearchController : OrganisationRegistryController
    {
        // TODO: Add to configuration
        private const int DefaultResponseLimit = 100;

        private const string ScrollTimeout = "30s";
        private const int ScrollSize = 500;

        private const string OrganisationsIndexName = "organisations";
        private const string PeopleIndexName = "people";
        private const string BodiesIndexName = "bodies";

        private readonly ILogger<SearchController> _log;

        public SearchController(
            ICommandSender commandSender,
            ILogger<SearchController> log) : base(commandSender)
        {
            _log = log;
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
        /// Na deze request heb je 30 seconden de tijd om een call te doen naar `v1/search/people/scroll?id={SCROLLID}`
        /// Deze zal je de volgende pagina geven (opnieuw 500 items), samen met de `x-search-metadata` header en een nieuwe <b>scrollId</b>.
        /// Herhaal dit proces tot je geen nieuwe items meer krijgt.
        /// </remarks>
        /// <param name="indexName">ElasticSearch index naam.
        /// Keuze tussen `organisations`, `people`, and `bodies`.</param>
        /// <param name="elastic"></param>
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

            switch (indexName.ToLower())
            {
                case OrganisationsIndexName:
                    return await GetApiSearch<OrganisationDocument>(
                        elastic,
                        q,
                        offset,
                        limit,
                        FilterOrganisationFields(
                            fields,
                            (await HttpContext.GetAuthenticateInfoAsync())?.Succeeded ?? false),
                        sort,
                        scroll);

                case PeopleIndexName:
                    return await GetApiSearch<PersonDocument>(elastic, q, offset, limit, fields, sort, scroll);

                case BodiesIndexName:
                    return await GetApiSearch<BodyDocument>(elastic, q, offset, limit, fields, sort, scroll);

                default:
                    return NotFound();
            }
        }

        /// <summary>Search all organisations.</summary>
        /// <param name="indexName">Elasticsearch index name</param>
        /// <param name="elastic"></param>
        /// <param name="q">Elasticsearch querystring search.</param>
        /// <param name="offset">Elasticsearch starting index position.</param>
        /// <param name="limit">Elasticsearch number of hits to return.</param>
        /// <param name="fields">Elasticsearch source filter.</param>
        /// <param name="sort">Elasticsearch sorting.</param>
        /// <param name="scroll">Enable Elasticsearch scrolling.</param>
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
                case OrganisationsIndexName:
                {
                    var response = await GetSearch<OrganisationDocument>(
                        elastic,
                        q,
                        offset,
                        limit,
                        FilterOrganisationFields(
                            fields,
                            (await HttpContext.GetAuthenticateInfoAsync())?.Succeeded ?? false),
                        sort,
                        scroll);

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
                            jsonSerializerSettings)
                    };
                }

                case PeopleIndexName: // Possibly not used
                {
                    var response = await GetSearch<PersonDocument>(elastic, q, offset - 1, limit, fields, sort, scroll);

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
                            jsonSerializerSettings)
                    };
                }

                case BodiesIndexName: // Possibly not used
                {
                    var response = await GetSearch<BodyDocument>(elastic, q, offset - 1, limit, fields, sort, scroll);

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
                            jsonSerializerSettings)
                    };
                }

                default:
                    return NotFound();
            }
        }

        private static (string part, Infrastructure.Search.Sorting.SortOrder sortOrder) GetSorting(string sortPart)
            => sortPart.StartsWith("-")
                ? (sortPart[1..], Infrastructure.Search.Sorting.SortOrder.Descending)
                : (sortPart, Infrastructure.Search.Sorting.SortOrder.Ascending);

        /// <summary>Search all organisations.</summary>
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

            switch (indexName.ToLower())
            {
                case OrganisationsIndexName:
                    return await PostApiSearch<OrganisationDocument>(
                        elastic,
                        q,
                        offset,
                        limit,
                        FilterOrganisationFields(
                            fields,
                            (await HttpContext.GetAuthenticateInfoAsync())?.Succeeded ?? false),
                        sort,
                        scroll);

                case PeopleIndexName:
                    return await PostApiSearch<PersonDocument>(elastic, q, offset, limit, fields, sort, scroll);

                case BodiesIndexName:
                    return await PostApiSearch<BodyDocument>(elastic, q, offset, limit, fields, sort, scroll);

                default:
                    return NotFound();
            }
        }

        /// <summary>Search all organisations.</summary>
        /// <param name="indexName">Elasticsearch index name</param>
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

            switch (indexName.ToLower())
            {
                case OrganisationsIndexName:
                    return await ScrollApiSearch<OrganisationDocument>(elastic, id);

                case PeopleIndexName:
                    return await ScrollApiSearch<PersonDocument>(elastic, id);

                case BodiesIndexName:
                    return await ScrollApiSearch<BodyDocument>(elastic, id);

                default:
                    return NotFound();
            }
        }

        private async Task<IActionResult> GetApiSearch<T>(
            Elastic elastic,
            string q,
            int? offset,
            int? limit,
            string fields,
            string sort,
            bool? scroll)
            where T : class, IDocument
        {
            var searchResults = await elastic
                .ReadClient
                .SearchAsync<T>(
                    search => BuildApiSearch(
                        search,
                        offset,
                        limit,
                        fields,
                        sort,
                        scroll,
                        new Expression<Func<T, object>>[]
                        {
                            defaultField => defaultField.Id,
                            defaultField => defaultField.ChangeId,
                            defaultField => defaultField.ChangeTime
                        },
                        defaultSort => defaultSort.Name.Suffix("keyword"),
                        query => query
                            .Bool(
                                b => b
                                    .Must(m => m.QueryString(qs => qs.Query(q))))));

            return BuildApiSearchResult(searchResults);
        }

        private async Task<ISearchResponse<T>> GetSearch<T>(
            Elastic elastic,
            string q,
            int? offset,
            int? limit,
            string fields,
            string sort,
            bool? scroll)
            where T : class, IDocument
        {
            var searchResponse = await elastic
                .ReadClient
                .SearchAsync<T>(
                    search => BuildApiSearch(
                        search,
                        offset,
                        limit,
                        fields,
                        sort,
                        scroll,
                        new Expression<Func<T, object>>[]
                        {
                            defaultField => defaultField.Id,
                            defaultField => defaultField.ChangeId,
                            defaultField => defaultField.ChangeTime
                        },
                        defaultSort => defaultSort.Name.Suffix("keyword"),
                        query => query
                            .Bool(
                                b => b
                                    .Must(m => m.QueryString(qs => qs.Query(q))))));
            return searchResponse;
        }

        private async Task<IActionResult> PostApiSearch<T>(
            Elastic elastic,
            JObject q,
            int? offset,
            int? limit,
            string fields,
            string sort,
            bool? scroll)
            where T : class, IDocument
        {
            var searchResults = await elastic
                .ReadClient
                .SearchAsync<T>(
                    search => BuildApiSearch(
                        search,
                        offset,
                        limit,
                        fields,
                        sort,
                        scroll,
                        new Expression<Func<T, object>>[]
                        {
                            defaultField => defaultField.Id,
                            defaultField => defaultField.ChangeId,
                            defaultField => defaultField.ChangeTime
                        },
                        defaultSort => defaultSort.Name.Suffix("keyword"),
                        query => query.Raw(q.ToString())));

            return BuildApiSearchResult(searchResults);
        }

        private async Task<IActionResult> ScrollApiSearch<T>(
            Elastic elastic,
            string id) where T : class
        {
            var searchResults = await elastic.ReadClient.ScrollAsync<T>(ScrollTimeout, id);

            return BuildApiSearchResult(searchResults);
        }

        private static ISearchRequest BuildApiSearch<T>(
            SearchDescriptor<T> search,
            int? offset,
            int? limit,
            string fields,
            string sort,
            bool? scroll,
            Expression<Func<T, object>>[] defaultFieldsFunc,
            Expression<Func<T, object>> defaultSort,
            Func<QueryContainerDescriptor<T>, QueryContainer> queryFunc) where T : class
        {
            if (!offset.HasValue) offset = 0;
            if (!limit.HasValue) limit = DefaultResponseLimit;

            // When scrolling, you dont get to decide offset or limit!
            if (scroll.HasValue && scroll.Value)
            {
                offset = 0;
                limit = ScrollSize;
            }

            search = search
                .From(offset.Value)
                .Size(limit.Value);

            if (!string.IsNullOrWhiteSpace(sort))
            {
                var sortParts = sort.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries).Select(x => x.Trim())
                    .ToArray();
                var sortDescriptor = new SortDescriptor<T>();

                foreach (var sortPart in sortParts)
                {
                    var descending = sortPart.StartsWith("-");
                    var part = descending ? sortPart.Substring(1) : sortPart;
                    sortDescriptor.Field(part, descending ? SortOrder.Descending : SortOrder.Ascending);
                }

                search = search.Sort(_ => sortDescriptor);
            }
            else
            {
                search = search.Sort(s => s.Ascending(defaultSort));
            }

            if (!string.IsNullOrWhiteSpace(fields))
                search = search
                    .Source(
                        source => source.Includes(
                            x => x
                                .Fields(defaultFieldsFunc)
                                .Fields(
                                    fields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                                        .Select(y => y.Trim())
                                        .Distinct().ToArray())));

            if (scroll.HasValue && scroll.Value)
                search = search.Scroll(ScrollTimeout);

            return search.Query(queryFunc);
        }

        private IActionResult BuildApiSearchResult<T>(ISearchResponse<T> searchResults) where T : class
        {
            var jsonSerializerSettings = GetJsonSerializerSettings();

            if (!searchResults.IsValid)
            {
                _log.LogCritical("Elasticsearch error occured on search! {Error}", searchResults.FormatError());

                throw new ApiException("Er is een probleem opgetreden bij het uitvoeren van de zoekopdracht.");
            }

            Response.AddElasticsearchMetaDataResponse(new ElasticsearchMetaData<T>(searchResults));

            return new ContentResult
            {
                ContentType = "application/json",
                StatusCode = (int)HttpStatusCode.OK,
                Content = JsonConvert.SerializeObject(
                    searchResults.Hits.Select(x => x.Source),
                    Formatting.Indented,
                    jsonSerializerSettings)
            };
        }

        private static string FilterOrganisationFields(string fields, bool isAuthenticated)
        {
            if (!string.IsNullOrWhiteSpace(fields))
                fields = isAuthenticated
                    ? fields
                    : string.Join(
                        ',',
                        fields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                            .Select(y => y.Trim())
                            .Except(new List<string> { "bankAccounts" })
                            .Distinct());

            if (!string.IsNullOrWhiteSpace(fields)) return fields;

            //if the user made a request to fetch only field 'bankAccounts', but is not authenticated,
            //the fields parameter will be empty and default to all fields, so second validation is needed

            return isAuthenticated
                ? string.Join(
                    ',',
                    typeof(OrganisationDocument)
                        .GetProperties()
                        .Select(x => x.Name.ToCamelCase()))
                : string.Join(
                    ',',
                    typeof(OrganisationDocument)
                        .GetProperties()
                        .Select(x => x.Name.ToCamelCase())
                        .Except(new List<string> { "bankAccounts" }).ToList());
        }

        private static JsonSerializerSettings GetJsonSerializerSettings()
        {
            var getSerializerSettings = JsonConvert.DefaultSettings ?? (() => new JsonSerializerSettings());
            var jsonSerializerSettings = getSerializerSettings();
            jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
            jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

            var maybeResolver = (OrganisationRegistryContractResolver?)jsonSerializerSettings.ContractResolver;
            if (maybeResolver is not { } resolver)
                throw new NullReferenceException("Resolver should not be null");

            resolver.SetStringDefaultValueToEmptyString = true;
            return jsonSerializerSettings;
        }
    }
}
