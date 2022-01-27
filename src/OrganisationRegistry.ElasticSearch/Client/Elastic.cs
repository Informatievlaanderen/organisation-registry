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
        public const int MaxResultWindow = 10000;

        private Policy RetryPolicy { get; }
        private AsyncRetryPolicy AsyncRetryPolicy { get; }

        public ElasticClient ReadClient { get; }
        public ElasticClient WriteClient { get; }

        public Elastic(
            ILogger<Elastic> logger,
            IOptions<ElasticSearchConfiguration> elasticSearchOptions)
        {
            var configuration = elasticSearchOptions.Value;
            WriteClient = GetElasticClient(configuration, write: true);
            ReadClient = GetElasticClient(configuration, write: false);

            RetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetry(
                    configuration.MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, _) =>
                        logger.LogError(0, exception,
                            "Elasticsearch exception occurred, attempt #{RetryCount}, trying again in {RetrySeconds} seconds.",
                            retryCount, timeSpan.TotalSeconds));

            AsyncRetryPolicy = Policy
                .Handle<Exception>()
                .WaitAndRetryAsync(
                    configuration.MaxRetryAttempts,
                    retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt)),
                    (exception, timeSpan, retryCount, _) =>
                        logger.LogError(0, exception,
                            "Elasticsearch exception occurred, attempt #{RetryCount}, trying again in {RetrySeconds} seconds.",
                            retryCount, timeSpan.TotalSeconds));
        }

        private static ElasticClient GetElasticClient(ElasticSearchConfiguration configuration, bool write)
        {
            var connectionSettings =
                new ConnectionSettings(new Uri(write ? configuration.WriteConnectionString : configuration.ReadConnectionString))
                    .DisableDirectStreaming();

            if (!string.IsNullOrEmpty(write ? configuration.WriteUser : configuration.ReadUser))
                connectionSettings.BasicAuthentication(
                    write ? configuration.WriteUser : configuration.ReadUser,
                    write ? configuration.WritePass : configuration.ReadPass);

            IConnectionSettingsValues settings = connectionSettings;

            settings.DefaultIndices.Add(typeof(OrganisationDocument), write ? configuration.OrganisationsWriteIndex : configuration.OrganisationsReadIndex);
            settings.DefaultRelationNames.Add(typeof(OrganisationDocument), configuration.OrganisationType);

            settings.DefaultIndices.Add(typeof(PersonDocument), write ? configuration.PeopleWriteIndex : configuration.PeopleReadIndex);
            settings.DefaultRelationNames.Add(typeof(PersonDocument), configuration.PersonType);
            settings.IdProperties.Add(typeof(PersonDocument), nameof(PersonDocument.Id));

            settings.DefaultIndices.Add(typeof(BodyDocument), write ? configuration.BodyWriteIndex : configuration.BodyReadIndex);
            settings.DefaultRelationNames.Add(typeof(BodyDocument), configuration.BodyType);

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
