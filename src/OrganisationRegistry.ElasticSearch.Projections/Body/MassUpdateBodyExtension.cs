namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Bodies;
    using Client;
    using Osc;

    public static class MassUpdateBodyExtension
    {
        /// <summary>
        /// For each Body Document where `queryFieldSelector` equals `queryFieldValue`,
        /// update in the list of `listPropertyName` where `idProperty` equals `queryFieldValue`,
        /// the value of the `namePropertyName` field in the list of `listPropertyName` to `newName`.
        /// Also sets the `changeId` and `changeTime` for each of those changes.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queryFieldSelector"></param>
        /// <param name="queryFieldValue"></param>
        /// <param name="listPropertyName"></param>
        /// <param name="idPropertyName"></param>
        /// <param name="namePropertyName"></param>
        /// <param name="newName"></param>
        /// <param name="changeId"></param>
        /// <param name="changeTime"></param>
        /// <param name="scrollSize"></param>
        public static void MassUpdateBody(
            this OpenSearchClient client,
            Expression<Func<BodyDocument, object>> queryFieldSelector,
            object queryFieldValue,
            string listPropertyName,
            string idPropertyName,
            string namePropertyName,
            object newName,
            int changeId,
            DateTimeOffset changeTime,
            int scrollSize = 100)
        {
            client.Indices.Refresh(Indices.Index<BodyDocument>());

            client
                .UpdateByQuery<BodyDocument>(x => x
                    .Query(q => q
                        .Term(t => t
                            .Field(queryFieldSelector)
                            .Value(queryFieldValue)))
                    .ScrollSize(scrollSize)
                    .Script(s => s
                        .Source($"for (int i = 0; i < ctx._source.{listPropertyName}.size(); i++) {{" +
                                $"if (ctx._source.{listPropertyName}[i].{idPropertyName} == params.idToChange) {{" +
                                $"ctx._source.{listPropertyName}[i].{namePropertyName} = params.newName;" +
                                "}" +
                                "}" +
                                "ctx._source.changeId = params.newChangeId;" +
                                "ctx._source.changeTime = params.newChangeTime;")
                        .Lang("painless")
                        .Params(p => p
                            .Add("idToChange", queryFieldValue)
                            .Add("newName", newName)
                            .Add("newChangeId", changeId)
                            .Add("newChangeTime", changeTime))))
                .ThrowOnFailure();
        }

                /// <summary>
        /// For each Body Document where `queryFieldSelector` equals `queryFieldValue`,
        /// update in the list of `listPropertyName` where `idProperty` equals `queryFieldValue`,
        /// the value of the `namePropertyName` field in the list of `listPropertyName` to `newName`.
        /// Also sets the `changeId` and `changeTime` for each of those changes.
        /// </summary>
        /// <param name="client"></param>
        /// <param name="queryFieldSelector"></param>
        /// <param name="queryFieldValue"></param>
        /// <param name="listPropertyName"></param>
        /// <param name="idPropertyName"></param>
        /// <param name="namePropertyName"></param>
        /// <param name="newName"></param>
        /// <param name="changeId"></param>
        /// <param name="changeTime"></param>
        /// <param name="scrollSize"></param>
        public static async Task MassUpdateBodyAsync(
            this Elastic client,
            Expression<Func<BodyDocument, object>> queryFieldSelector,
            object queryFieldValue,
            string listPropertyName,
            string idPropertyName,
            string namePropertyName,
            object newName,
            int changeId,
            DateTimeOffset changeTime,
            int scrollSize = 100)
        {
            await client.TryGetAsync(async () =>
                (await client.WriteClient.Indices.RefreshAsync(Indices.Index<BodyDocument>())).ThrowOnFailure());

            (await client.WriteClient
                    .UpdateByQueryAsync<BodyDocument>(x => x
                        .Query(q => q
                            .Term(t => t
                                .Field(queryFieldSelector)
                                .Value(queryFieldValue)))
                        .ScrollSize(scrollSize)
                        .Script(s => s
                            .Source($"for (int i = 0; i < ctx._source.{listPropertyName}.size(); i++) {{" +
                                    $"if (ctx._source.{listPropertyName}[i].{idPropertyName} == params.idToChange) {{" +
                                    $"ctx._source.{listPropertyName}[i].{namePropertyName} = params.newName;" +
                                    "}" +
                                    "}" +
                                    "ctx._source.changeId = params.newChangeId;" +
                                    "ctx._source.changeTime = params.newChangeTime;")
                            .Lang("painless")
                            .Params(p => p
                                .Add("idToChange", queryFieldValue)
                                .Add("newName", newName)
                                .Add("newChangeId", changeId)
                                .Add("newChangeTime", changeTime)))))
                .ThrowOnFailure();
        }
    }
}
