namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure.Change;

using System;
using System.Threading.Tasks;
using Client;

public class ElasticMassChange : IElasticChange
{
    public ElasticMassChange(Func<Elastic,Task> change)
    {
        Change = change;
    }

    public Func<Elastic,Task> Change { get; set; }
}
