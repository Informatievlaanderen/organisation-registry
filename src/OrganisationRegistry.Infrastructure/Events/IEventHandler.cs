namespace OrganisationRegistry.Infrastructure.Events
{
    using System.Data.Common;
    using Messages;

    public interface IEventHandler<in T> : IHandler<T> where T : IEvent<T>
    {
        void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> message);
    }
}
