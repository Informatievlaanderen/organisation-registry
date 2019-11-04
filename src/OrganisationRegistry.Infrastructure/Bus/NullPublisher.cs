namespace OrganisationRegistry.Infrastructure.Bus
{
    using System.Data.Common;
    using Events;

    public class NullPublisher : IEventPublisher
    {
        public void Publish<T>(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>
        {
        }

        public void ProcessReactions<T>(IEnvelope<T> envelope) where T : IEvent<T>
        {
        }
    }
}
