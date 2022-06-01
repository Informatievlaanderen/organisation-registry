namespace OrganisationRegistry.Infrastructure.Bus;

using System.Data.Common;
using System.Threading.Tasks;
using Events;

public class NullPublisher : IEventPublisher
{
    public Task Publish<T>(DbConnection? dbConnection, DbTransaction? dbTransaction, IEnvelope<T> envelope)
        where T : IEvent<T>
    {
        return Task.CompletedTask;
    }
}
