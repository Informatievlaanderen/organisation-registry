namespace OrganisationRegistry.Api;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using ElasticSearch;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using ElasticSearch.Configuration;
using ElasticSearch.Organisations;
using ElasticSearch.People;
using Infrastructure.Helpers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using Osc;

public interface IElasticSearchFacade
{
    Task<ISearchResponse<OrganisationDocument>?> SearchOrganisationsWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken);
    Task<ISearchResponse<PersonDocument>?> SearchPeopleWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken);
    Task<ISearchResponse<BodyDocument>?> SearchBodiesWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken);
    Task<ISearchResponse<OrganisationDocument>> SearchOrganisations(Elastic elastic, string q, int? offset, int? limit, string fields, string sort, bool? scroll, CancellationToken cancellationToken = default);

    Task<ISearchResponse<T>> GetApiSearch<T>(
        Elastic elastic,
        string q,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll)
        where T : class, IDocument;

    Task<ISearchResponse<T>> GetSearch<T>(
        Elastic elastic,
        string q,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll,
        CancellationToken cancellationToken = default)
        where T : class, IDocument;

    Task<ISearchResponse<T>> PostApiSearch<T>(
        Elastic elastic,
        JObject q,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll)
        where T : class, IDocument;

    Task<ISearchResponse<OrganisationDocument>> PostApiSearchOrganisations(Elastic elastic, JObject q, int? offset, int? limit, string fields, string sort, bool? scroll);

    Task<ISearchResponse<T>> ScrollApiSearch<T>(
        Elastic elastic,
        string id) where T : class, IDocument;

    Task<ISearchResponse<TDocument>> ScrollSearch<TDocument>(Elastic elastic, string id, CancellationToken cancellationToken = default) where TDocument : class, IDocument;
}

public class ElasticSearchFacade : IElasticSearchFacade
{
    public const string OrganisationsIndexName = "organisations";
    public const string PeopleIndexName = "people";
    public const string BodiesIndexName = "bodies";

    private const string BankaccountsFieldName = "bankAccounts";

    private const string KeywordSuffix = "keyword";

    private const int DefaultResponseLimit = 100;

    private readonly ILogger<ElasticSearchFacade> _logger;
    private readonly ElasticSearchConfiguration _configuration;

    public ElasticSearchFacade(ILogger<ElasticSearchFacade> logger, IOptions<ElasticSearchConfiguration> elasticSearchConfiguration)
    {
        _logger = logger;
        _configuration = elasticSearchConfiguration.Value;
    }

    public async Task<ISearchResponse<OrganisationDocument>?> SearchOrganisationsWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken)
        => await SearchOrganisations(elastic, q, _configuration.ScrollSize, DefaultResponseLimit, fields, sort, true, cancellationToken);

    public async Task<ISearchResponse<PersonDocument>?> SearchPeopleWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken)
        => await GetSearch<PersonDocument>(elastic, q, _configuration.ScrollSize, DefaultResponseLimit, fields, sort, true, cancellationToken);

    public async Task<ISearchResponse<BodyDocument>?> SearchBodiesWithDefaultScrolling(Elastic elastic, string q, string fields, string sort, CancellationToken cancellationToken)
        => await GetSearch<BodyDocument>(elastic, q, _configuration.ScrollSize, DefaultResponseLimit, fields, sort, true, cancellationToken);

    public async Task<ISearchResponse<OrganisationDocument>> SearchOrganisations(Elastic elastic, string q, int? offset, int? limit, string fields, string sort, bool? scroll, CancellationToken cancellationToken = default)
        => await GetSearch<OrganisationDocument>(
            elastic,
            q,
            offset,
            limit,
            RemoveBankAccountFields(fields),
            sort,
            scroll,
            cancellationToken);

    private static string RemoveBankAccountFields(string fields)
    {
        if (!string.IsNullOrWhiteSpace(fields))
            fields = string.Join(
                ',',
                fields.Split(new[] { "," }, StringSplitOptions.RemoveEmptyEntries)
                    .Select(y => y.Trim())
                    .Except(new List<string> { BankaccountsFieldName })
                    .Distinct());

        if (!string.IsNullOrWhiteSpace(fields)) return fields;

        //if the user made a request to fetch only field 'bankAccounts',
        //the fields parameter will be empty and default to all fields, so second validation is needed

        return string.Join(
            ',',
            typeof(OrganisationDocument)
                .GetProperties()
                .Select(x => x.Name.ToCamelCase())
                .Except(new List<string> { BankaccountsFieldName }).ToList());
    }

    public async Task<ISearchResponse<T>> GetApiSearch<T>(
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

        return searchResults.ThrowIfInvalid(_logger);
    }

    public async Task<ISearchResponse<T>> GetSearch<T>(
        Elastic elastic,
        string q,
        int? offset,
        int? limit,
        string fields,
        string sort,
        bool? scroll,
        CancellationToken cancellationToken = default)
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
                    _configuration.ScrollTimeout),
                cancellationToken);

        return searchResponse;
    }

    public async Task<ISearchResponse<T>> PostApiSearch<T>(
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

        return searchResults.ThrowIfInvalid(_logger);
    }

    public async Task<ISearchResponse<OrganisationDocument>> PostApiSearchOrganisations(Elastic elastic, JObject q, int? offset, int? limit, string fields, string sort, bool? scroll)
        => await PostApiSearch<OrganisationDocument>(
            elastic,
            q,
            offset,
            limit,
            RemoveBankAccountFields(fields),
            sort,
            scroll);

    public async Task<ISearchResponse<T>> ScrollApiSearch<T>(
        Elastic elastic,
        string id) where T : class, IDocument
    {
        var searchResults = await ScrollSearch<T>(elastic, id);

        return searchResults.ThrowIfInvalid(_logger);
    }

    public async Task<ISearchResponse<TDocument>> ScrollSearch<TDocument>(Elastic elastic, string id, CancellationToken cancellationToken = default) where TDocument : class, IDocument
        => await elastic.ReadClient.ScrollAsync<TDocument>(_configuration.ScrollTimeout, id, ct: cancellationToken);

    private ISearchRequest BuildApiSearch<T>(
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
            limit = _configuration.ScrollSize;
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
}
