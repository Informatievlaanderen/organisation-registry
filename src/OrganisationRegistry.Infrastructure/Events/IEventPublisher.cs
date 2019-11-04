namespace OrganisationRegistry.Infrastructure.Events
{
    using System.Data.Common;

    public interface IEventPublisher
    {
        void Publish<T>(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>;

        void ProcessReactions<T>(IEnvelope<T> envelope) where T : IEvent<T>;
    }
}
