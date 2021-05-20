namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change
{
    using System;
    using System.Collections.Generic;
    using Bodies;

    public class ElasticPerDocumentChange<T> : IElasticChange where T: IDocument
    {
        public ElasticPerDocumentChange(Guid id, Action<T> change)
        {
            // Id = id;
            // Change = change;
            Changes = new Dictionary<Guid, Action<T>> {{id, change}};
        }

        public ElasticPerDocumentChange(Dictionary<Guid, Action<T>> changes)
        {
            // Id = id;
            // Change = change;
            Changes = changes;
        }

        // public Guid Id { get; set; }
        // public Action<T> Change { get; set; }

        public Dictionary<Guid, Action<T>> Changes { get; init; }
    }
}
