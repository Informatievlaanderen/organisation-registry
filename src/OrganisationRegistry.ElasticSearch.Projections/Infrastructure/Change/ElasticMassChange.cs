namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change
{
    using System;
    using Client;

    public class ElasticMassChange : IElasticChange
    {
        public ElasticMassChange(Action<Elastic> change)
        {
            Change = change;
        }

        public Action<Elastic> Change { get; set; }
    }
}
