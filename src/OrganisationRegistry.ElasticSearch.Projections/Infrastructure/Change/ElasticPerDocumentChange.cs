namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;

public class ElasticPerDocumentChange<T> : IElasticChange where T: IDocument
{
    public ElasticPerDocumentChange(Guid id, Action<T> change)
    {
        Changes = new Dictionary<Guid, Func<T, Task>> {{id, doc =>
            {
                change(doc);
                return Task.CompletedTask;
            }
        }};
    }

    public ElasticPerDocumentChange(Dictionary<Guid, Action<T>> changes)
    {
        Changes = new Dictionary<Guid, Func<T, Task>>();
        foreach (var change in changes)
        {
            Func<T, Task> changeAction = doc =>
            {
                change.Value(doc);
                return Task.CompletedTask;
            };

            Changes.Add(change.Key, changeAction);
        }
    }

    public ElasticPerDocumentChange(Guid id, Func<T, Task> change)
    {
        Changes = new Dictionary<Guid, Func<T, Task>> {{id, change}};
    }

    public ElasticPerDocumentChange(Dictionary<Guid, Func<T, Task>> changes)
    {
        Changes = changes;
    }

    public Dictionary<Guid, Func<T, Task>> Changes { get; init; }
}