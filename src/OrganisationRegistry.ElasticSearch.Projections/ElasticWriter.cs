namespace OrganisationRegistry.ElasticSearch.Projections
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Client;
    using Microsoft.Extensions.Logging;
    using Osc;

    public static class ElasticWriter
    {
        private static readonly TimeSpan ScrollTimeout = TimeSpan.FromMinutes(5);

        public static async Task UpdateByScroll<T>(
            Elastic elastic,
            ILogger logger,
            Func<QueryContainerDescriptor<T>, QueryContainer> query,
            Func<T, Task> updateAction) where T : class
        {
            await elastic.WriteClient.Indices.RefreshAsync(Indices.Index<T>());

            var searchResponse = await elastic.TryGetAsync(async () => (await elastic.WriteClient.SearchAsync<T>(
                search => search
                    .From(0)
                    .Size(Elastic.MaxResultWindow)
                    .Query(query)
                    .Scroll(new Time(ScrollTimeout)))).ThrowOnFailure());

            var documents = new List<T>();

            while (searchResponse.Documents.Any())
            {
                foreach (var document in searchResponse.Documents.ToList())
                {
                    await updateAction(document);
                }

                documents.AddRange(searchResponse.Documents);

                var response = searchResponse;
                searchResponse = await elastic.TryGetAsync(async () => (await
                    elastic.WriteClient.ScrollAsync<T>(new Time(ScrollTimeout),
                        response.ScrollId)).ThrowOnFailure());

            }

            await elastic.TryAsync(async () => (await
                    elastic.WriteClient.ClearScrollAsync(new ClearScrollRequest(searchResponse.ScrollId)))
                .ThrowOnFailure());

            if (!documents.Any())
                return;

            await elastic.TryAsync(async () =>
            {
                elastic.WriteClient.BulkAll(documents, b => b
                    .BackOffTime("30s")
                    .BackOffRetries(5)
                    .RefreshOnCompleted(false)
                    .MaxDegreeOfParallelism(Environment.ProcessorCount)
                    .Size(1000)
                )
                .Wait(TimeSpan.FromMinutes(15), next =>
                {
                    logger.LogInformation("Writing page {PageNumber}", next.Page);
                });

                await Task.CompletedTask;
            });
        }
    }
}
