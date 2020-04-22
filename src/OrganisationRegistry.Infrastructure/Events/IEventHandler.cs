namespace OrganisationRegistry.Infrastructure.Events
{
    using System.Data.Common;
    using System.Threading.Tasks;
    using Messages;

    public interface IEventHandler<in T> : IHandler<T> where T : IEvent<T>
    {
        Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> message);
    }
}
