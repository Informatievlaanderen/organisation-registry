namespace OrganisationRegistry.Infrastructure.Events;

using System.Data.Common;
using System.Threading.Tasks;

public interface IEventPublisher
{
    Task Publish<T>(DbConnection? dbConnection, DbTransaction? dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>;
}
