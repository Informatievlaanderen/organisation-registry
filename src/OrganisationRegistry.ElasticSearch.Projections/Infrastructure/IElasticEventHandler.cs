namespace OrganisationRegistry.ElasticSearch.Projections.Infrastructure;

using System.Data.Common;
using System.Threading.Tasks;
using Change;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Infrastructure.Messages;

public interface IElasticEventHandler<in T> : IHandler<T> where T : IEvent<T>
{
    Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> message);
}
