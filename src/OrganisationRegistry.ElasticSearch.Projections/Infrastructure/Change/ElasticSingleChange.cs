namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change
{
    using System;
    using Bodies;

    public class ElasticSingleChange : IElasticChange
    {
        public ElasticSingleChange(Guid id, Action<BodyDocument> change)
        {
            Id = id;
            Change = change;
        }

        public Guid Id { get; set; }
        public Action<BodyDocument> Change { get; set; }
    }
}
