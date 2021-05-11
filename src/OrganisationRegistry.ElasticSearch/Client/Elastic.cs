namespace OrganisationRegistry.ElasticSearch.Client
{
    using System;
    using System.Threading.Tasks;
    using Bodies;
    using Configuration;
    using Microsoft.Extensions.Logging;
    using Microsoft.Extensions.Options;
    using Nest;
    using Organisations;
    using People;
    using Polly;
    using Polly.Retry;
    using Policy = Polly.Policy;

    public static class ElasticSearch6BugFix
    {
        public static async Task<bool> DoesIndexExist(this IElasticClient client, string indexName) => (await client.Indices.ExistsAsync(indexName)).Exists;
    }

    // Scoped as SingleInstance()
    public class Elastic
    {
        private readonly ILogger<Elastic> _logger;
        private readonly ElasticSearchConfiguration _configuration;
        private RetryPolicy _waitAndRetry;
        private AsyncRetryPolicy _waitAndRetryAsync;
        public const int MaxResultWindow = 10000;

        private Policy RetryPolicy { get; }
        private AsyncRetryPolicy AsyncRetryPolicy { get; }

        public ElasticClient ReadClient => GetElasticClient(write: false);
        public ElasticClient WriteClient => GetElasticClient(write: true);

        public Elastic(
            ILogger<Elastic> logger,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions)
        {
            _logger = logger;
            _configuration = elasticSearchOptions.Value;

            RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    _configuration.MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                        _logger.LogError(0, exception,
                            "Elasticsearch exception occurred, attempt #{RetryCount}, trying again in {RetrySeconds} seconds.",
                            retryCount, timeSpan.TotalSeconds));

            AsyncRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    _configuration.MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, context) =>
                        _logger.LogError(0, exception,
                            "Elasticsearch exception occurred, attempt #{RetryCount}, trying again in {RetrySeconds} seconds.",
                            retryCount, timeSpan.TotalSeconds));
        }

        private ElasticClient GetElasticClient(bool write)
        {
            var connectionSettings =
                new ConnectionSettings(new Uri(write ? _configuration.WriteConnectionString : _configuration.ReadConnectionString))
                    .DisableDirectStreaming();

            if (!string.IsNullOrEmpty(write ? _configuration.WriteUser : _configuration.ReadUser))
                connectionSettings.BasicAuthentication(
                    write ? _configuration.WriteUser : _configuration.ReadUser,
                    write ? _configuration.WritePass : _configuration.ReadPass);

            IConnectionSettingsValues settings = connectionSettings;

            settings.DefaultIndices.Add(typeof(OrganisationDocument), write ? _configuration.OrganisationsWriteIndex : _configuration.OrganisationsReadIndex);
            settings.DefaultRelationNames.Add(typeof(OrganisationDocument), _configuration.OrganisationType);

            settings.DefaultIndices.Add(typeof(PersonDocument), write ? _configuration.PeopleWriteIndex : _configuration.PeopleReadIndex);
            settings.DefaultRelationNames.Add(typeof(PersonDocument), _configuration.PersonType);
            settings.IdProperties.Add(typeof(PersonDocument), nameof(PersonDocument.Id));

            settings.DefaultIndices.Add(typeof(BodyDocument), write ? _configuration.BodyWriteIndex : _configuration.BodyReadIndex);
            settings.DefaultRelationNames.Add(typeof(BodyDocument), _configuration.BodyType);

            return new ElasticClient(settings);
        }

        public void Try(Action actionToTry)
        {
            RetryPolicy.Execute(actionToTry);
        }

        public async Task TryAsync(Func<Task> actionToTry)
        {
            await AsyncRetryPolicy.ExecuteAsync(async () => await actionToTry());
        }

        public T TryGet<T>(Func<T> actionToTry)
        {
            var result = RetryPolicy.ExecuteAndCapture(actionToTry);

            if (result.Outcome == OutcomeType.Successful)
                return result.Result;

            throw result.FinalException;
        }

        public async Task<T> TryGetAsync<T>(Func<Task<T>> actionToTry)
        {
            var result = await AsyncRetryPolicy.ExecuteAndCaptureAsync(async () => await actionToTry());

            if (result.Outcome == OutcomeType.Successful)
                return result.Result;

            throw result.FinalException;
        }
    }
}
