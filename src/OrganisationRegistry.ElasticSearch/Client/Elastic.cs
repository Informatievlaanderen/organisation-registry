namespace OrganisationRegistry.ElasticSearch.Client
{
    using System;
    using Bodies;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Organisations;
    using People;
    using Polly;
    using Polly.Retry;

    public static class ElasticSearch6BugFix
    {
        public static bool DoesIndexExist(this IElasticClient client, string indexName) => client.IndexExistsAsync(indexName).GetAwaiter().GetResult().Exists;
    }

    // Scoped as SingleInstance()
    public class Elastic
    {
        private readonly ILogger<Elastic> _logger;
        private readonly ElasticSearchConfiguration _configuration;
        private RetryPolicy _waitAndRetry;
        public const int MaxResultWindow = 10000;

        private Policy RetryPolicy =>
            _waitAndRetry ??
            (_waitAndRetry = Policy
                //.Handle<ElasticsearchException>()
                .Handle<Exception>()
                .WaitAndRetry(
                    3,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                        _logger.LogError(0, exception, "Elasticsearch exception occurred, attempt #{RetryCount}, trying again in {RetrySeconds} seconds.", retryCount, timeSpan.TotalSeconds)));

        public ElasticClient ReadClient => GetElasticClient(write: false);
        public ElasticClient WriteClient => GetElasticClient(write: true);

        public Elastic(
            ILogger<Elastic> logger,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions)
        {
            _logger = logger;
            _configuration = elasticSearchOptions.Value;
        }

        private ElasticClient GetElasticClient(bool write)
        {
            var connectionSettings = new ConnectionSettings(new Uri(_configuration.ConnectionString))
                .DisableDirectStreaming();

            if (!string.IsNullOrEmpty(_configuration.User))
                connectionSettings.BasicAuthentication(_configuration.User, _configuration.Pass);

            IConnectionSettingsValues settings = connectionSettings;

            settings.DefaultIndices.Add(typeof(OrganisationDocument), write ? _configuration.OrganisationsWriteIndex : _configuration.OrganisationsReadIndex);
            settings.DefaultTypeNames.Add(typeof(OrganisationDocument), _configuration.OrganisationType);

            settings.DefaultIndices.Add(typeof(PersonDocument), write ? _configuration.PeopleWriteIndex : _configuration.PeopleReadIndex);
            settings.DefaultTypeNames.Add(typeof(PersonDocument), _configuration.PersonType);
            settings.IdProperties.Add(typeof(PersonDocument), nameof(PersonDocument.Id));

            settings.DefaultIndices.Add(typeof(BodyDocument), write ? _configuration.BodyWriteIndex : _configuration.BodyReadIndex);
            settings.DefaultTypeNames.Add(typeof(BodyDocument), _configuration.BodyType);

            return new ElasticClient(settings);
        }

        public void Try(Action actionToTry)
        {
            RetryPolicy.Execute(actionToTry);
        }

        public T TryGet<T>(Func<T> actionToTry)
        {
            var result = RetryPolicy.ExecuteAndCapture(actionToTry);

            if (result.Outcome == OutcomeType.Successful)
                return result.Result;

            throw result.FinalException;
        }
    }
}
