namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change
{
    using System.Collections.Generic;

    public class ElasticChanges
    {
        public ElasticChanges(int envelopeNumber, IEnumerable<IElasticChange> changes)
        {
            EnvelopeNumber = envelopeNumber;
            Changes = changes;
        }

        public int EnvelopeNumber { get; set; }
        public IEnumerable<IElasticChange> Changes { get; set; }
    }
}
