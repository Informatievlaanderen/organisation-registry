namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Be.Vlaanderen.Basisregisters.Utilities;
using Client;
using Humanizer;

public class ElasticPerDocumentChange<T> : IElasticChange where T: IDocument
{
    public ElasticPerDocumentChange(Guid id, Action<T> change)
    {
        Changes = new Dictionary<Guid, Func<T, Task>> {{id, doc =>
            {
                try{
                    change(doc);
                    return Task.CompletedTask;
                }catch (InvalidOperationException)
                {
                    return Task.FromException(new ElasticsearchPerDocumentChangeException(doc.Id, doc.ChangeId));
                }
            }
        }};
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

