namespace OrganisationRegistry.ElasticSearch.Projections.People;

using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Client;
using Osc;
using ElasticSearch.People;

public static class MassUpdatePersonExtension
{
    public static void MassUpdatePerson(
        this OpenSearchClient client,
        Expression<Func<PersonDocument, object>> queryFieldSelector,
        object queryFieldValue,
        string listPropertyName,
        string idPropertyName,
        string namePropertyName,
        object newName,
        int changeId,
        DateTimeOffset changeTime,
        int scrollSize = 100)
    {
        client.Indices.Refresh(Indices.Index<PersonDocument>());

        client
            .UpdateByQuery<PersonDocument>(x => x
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

    public static async Task MassUpdatePersonAsync(
        this Elastic client,
        Expression<Func<PersonDocument, object?>> queryFieldSelector,
        object queryFieldValue,
        string listPropertyName,
        string idPropertyName,
        string namePropertyName,
        object? newName,
        int changeId,
        DateTimeOffset changeTime,
        int scrollSize = 100)
    {
        await client.TryGetAsync(async () =>
            (await client.WriteClient.Indices.RefreshAsync(Indices.Index<PersonDocument>())).ThrowOnFailure());

        (await client.WriteClient
                .UpdateByQueryAsync<PersonDocument>(x => x
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

    public static async Task MassTerminatePersonFunctionsAsync(
        this Elastic client,
        Expression<Func<PersonDocument, object?>> queryFieldSelector,
        Guid organisationId,
        IDictionary<Guid, DateTime> functionsToTerminate,
        int changeId,
        DateTimeOffset changeTime,
        int scrollSize = 100)
    {
        await client.TryGetAsync(async () =>
            (await client.WriteClient.Indices.RefreshAsync(Indices.Index<PersonDocument>())).ThrowOnFailure());

        var updateByQueryResponse = await client.WriteClient
            .UpdateByQueryAsync<PersonDocument>(x => x
                .Query(q => q
                    .Term(t => t
                        .Field(queryFieldSelector)
                        .Value(organisationId)))
                .ScrollSize(scrollSize)
                .Script(s => s
                    .Source(
                        @"for (int i = 0; i < ctx._source.functions.size(); i++) {
                                 if (params.functionsToTerminate.containsKey(ctx._source.functions[i].personFunctionId)) {
                                     ctx._source.functions[i].validity.end = params.functionsToTerminate[ctx._source.functions[i].personFunctionId];
                                 }
                             }
                             ctx._source.changeId = params.newChangeId;
                             ctx._source.changeTime = params.newChangeTime;")
                    .Lang("painless")
                    .Params(p => p
                        .Add("functionsToTerminate", functionsToTerminate)
                        .Add("newChangeId", changeId)
                        .Add("newChangeTime", changeTime))));
        updateByQueryResponse.ThrowOnFailure();
    }
}
