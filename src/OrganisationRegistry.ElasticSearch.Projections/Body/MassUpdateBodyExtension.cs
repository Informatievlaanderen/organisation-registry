namespace OrganisationRegistry.ElasticSearch.Projections.Body
{
    using System;
    using System.Linq.Expressions;
    using Bodies;
    using Client;
    using Nest;

    public static class MassUpdateBodyExtension
    {
        public static void MassUpdateBody(
            this ElasticClient client,
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
            client.Refresh(Indices.Index<BodyDocument>());

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
    }
}
