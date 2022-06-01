namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change;

using System;
using System.Collections.Generic;

public class ElasticDocumentCreation<T> : IElasticChange where T: IDocument
{
    public ElasticDocumentCreation(Guid id, Func<T> change)
    {
        Changes = new Dictionary<Guid, Func<T>> {{id, change}};
    }

    public Dictionary<Guid, Func<T>> Changes { get; init; }
}
