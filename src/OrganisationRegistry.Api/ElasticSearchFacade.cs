namespace OrganisationRegistry.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net;
using System.Threading.Tasks;
using ElasticSearch;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using ElasticSearch.Common;
using ElasticSearch.Configuration;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Infrastructure;
using Infrastructure.Helpers;
using Infrastructure.Search;
using Infrastructure.Security;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OrganisationRegistry.Infrastructure.Infrastructure.Json;
using Osc;

public class ElasticSearchFacade
{
    public const string OrganisationsIndexName = "organisations";
    public const string PeopleIndexName = "people";
    public const string BodiesIndexName = "bodies";

    private const string BankaccountsFieldName = "bankAccounts";

    private const string KeywordSuffix = "keyword";

    private const int DefaultResponseLimit = 100;

    private const int ScrollSize = 500;

    private readonly HttpContext _httpContext;
    private readonly ILogger _logger;
    private readonly ElasticSearchConfiguration _configuration;

    public ElasticSearchFacade(HttpContext httpContext, ILogger logger, ElasticSearchConfiguration configuration)
    {
        _httpContext = httpContext;
        _logger = logger;
        _configuration = configuration;
    }

    public async Task<ISearchResponse<IDocument>?> Search(string indexName, Elastic elastic, string q, int? offset, int? limit, string fields, string sort, bool? scroll)
        => indexName.ToLower() switch
        {
            OrganisationsIndexName => await SearchOrganisations(elastic, q, offset, limit, fields, sort, scroll),
            PeopleIndexName => await GetSearch<PersonDocument>(elastic, q, offset, limit, fields, sort, scroll),
            BodiesIndexName => await GetSearch<BodyDocument>(elastic, q, offset, limit, fields, sort, scroll),
            _ => default,
        };

    public async Task<ISearchResponse<IDocument>?> SearchWithDefaultScrolling(string indexName, Elastic elastic, string q, string fields, string sort)
        => await Search(indexName, elastic, q, ScrollSize, DefaultResponseLimit, fields, sort, true);

    public async Task<ISearchResponse<OrganisationDocument>> SearchOrganisations(Elastic elastic, string q, int? offset, int? limit, string fields, string sort, bool? scroll)
        => await GetSearch<OrganisationDocument>(
            elastic,
            q,
            offset,
            limit,
            FilterOrganisationFields(fields, (await _httpContext.GetAuthenticateInfoAsync())?.Succeeded ?? false),
            sort,
            scroll);

    private static string FilterOrganisationFields(string fields, bool isAuthenticated)
    {
        if (!string.IsNullOrWhiteSpace(fields))
            fields = isAuthenticated
                ? fields
                : string.Join(
                    ',',
                    fields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                        .Select(y => y.Trim())
                        .Except(new List<string> { BankaccountsFieldName })
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
                    .Except(new List<string> { BankaccountsFieldName }).ToList());
    }

    public async Task<IActionResult> GetApiSearch<T>(
        Elastic elastic,
        string q,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll)
        where T : class, IDocument
    {
        var searchResults = await GetSearch<T>(elastic, q, offset, limit, fields, sort, scroll);

        return BuildApiSearchResult(searchResults);
    }

    public async Task<ISearchResponse<T>> GetSearch<T>(
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
                        defaultField => defaultField.ChangeTime,
                    },
                    defaultSort => defaultSort.Name.Suffix(KeywordSuffix),
                    query => query
                        .Bool(
                            b => b
                                .Must(m => m.QueryString(qs => qs.Query(q)))),
                    _configuration.ScrollTimeout));

        return searchResponse;
    }

    public async Task<IActionResult> PostApiSearch<T>(
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
                        defaultField => defaultField.ChangeTime,
                    },
                    defaultSort => defaultSort.Name.Suffix(KeywordSuffix),
                    query => query.Raw(q.ToString()),
                    _configuration.ScrollTimeout));

        return BuildApiSearchResult(searchResults);
    }

    public async Task<IActionResult> PostApiSearchOrganisations(Elastic elastic, JObject q, int? offset, int? limit, string fields, string sort, bool? scroll)
        => await PostApiSearch<OrganisationDocument>(
            elastic,
            q,
            offset,
            limit,
            FilterOrganisationFields(fields, (await _httpContext.GetAuthenticateInfoAsync())?.Succeeded ?? false),
            sort,
            scroll);

    public async Task<IActionResult> ScrollApiSearch<T>(
        Elastic elastic,
        string id) where T : class
    {
        var searchResults = await ScrollSearch<T>(elastic, id, _configuration.ScrollTimeout);

        return BuildApiSearchResult(searchResults);
    }

    private static async Task<ISearchResponse<T>> ScrollSearch<T>(Elastic elastic, string id, string scrollTimeout) where T : class
        => await elastic.ReadClient.ScrollAsync<T>(scrollTimeout, id);

    public async Task<ISearchResponse<IDocument>> ScrollSearch(Elastic elastic, string indexName, string id)
        => indexName.ToLower() switch
        {
            OrganisationsIndexName => await elastic.ReadClient.ScrollAsync<OrganisationDocument>(_configuration.ScrollTimeout, id),
            PeopleIndexName => await elastic.ReadClient.ScrollAsync<PersonDocument>(_configuration.ScrollTimeout, id),
            BodiesIndexName => await elastic.ReadClient.ScrollAsync<BodyDocument>(_configuration.ScrollTimeout, id),
            _ => throw new IndexOutOfRangeException(indexName),
        };

    private static ISearchRequest BuildApiSearch<T>(
        SearchDescriptor<T> search,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll,
        Expression<Func<T, object>>[] defaultFieldsFunc,
        Expression<Func<T, object>> defaultSort,
        Func<QueryContainerDescriptor<T>, QueryContainer> queryFunc,
        string scrollTimeout) where T : class
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
            search = search.Scroll(scrollTimeout);

        return search.Query(queryFunc);
    }

    public IActionResult BuildApiSearchResult<T>(ISearchResponse<T> searchResults) where T : class
    {
        var jsonSerializerSettings = GetJsonSerializerSettings();

        if (!searchResults.IsValid)
        {
            _logger.LogCritical("Elasticsearch error occured on search! {Error}", searchResults.FormatError());

            throw new ApiException("Er is een probleem opgetreden bij het uitvoeren van de zoekopdracht.");
        }

        _httpContext.Response.AddElasticsearchMetaDataResponse(new ElasticsearchMetaData<T>(searchResults));

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

    public static JsonSerializerSettings GetJsonSerializerSettings()
    {
        var getSerializerSettings = JsonConvert.DefaultSettings ?? (() => new JsonSerializerSettings());
        var jsonSerializerSettings = getSerializerSettings();
        jsonSerializerSettings.NullValueHandling = NullValueHandling.Ignore;
        jsonSerializerSettings.DefaultValueHandling = DefaultValueHandling.Ignore;

        var maybeResolver = (OrganisationRegistryContractResolver?)jsonSerializerSettings.ContractResolver;
        if (maybeResolver is not { } resolver)
            throw new NullReferenceException("Resolver should not be null");

        resolver.SetStringDefaultValueToEmptyString = true;
        resolver.RemoveEmptyArrays = true;

        return jsonSerializerSettings;
    }
}
