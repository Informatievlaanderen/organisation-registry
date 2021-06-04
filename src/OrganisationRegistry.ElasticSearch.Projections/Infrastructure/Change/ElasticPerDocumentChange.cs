namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change
{
    using System;
    using System.Collections.Generic;
    using Bodies;

    public class ElasticPerDocumentChange<T> : IElasticChange where T: IDocument
    {
        public ElasticPerDocumentChange(Guid id, Action<T> change)
        {
            Changes = new Dictionary<Guid, Action<T>> {{id, change}};
        }

        public ElasticPerDocumentChange(Dictionary<Guid, Action<T>> changes)
        {
            Changes = changes;
        }

        public Dictionary<Guid, Action<T>> Changes { get; init; }
    }
}
