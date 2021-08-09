namespace OrganisationRegistry.Infrastructure.Events
{
    using System.Data.Common;
    using System.Threading.Tasks;
    using Authorization;

    public interface IEventPublisher
    {
        Task Publish<T>(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<T> envelope) where T : IEvent<T>;

        Task ProcessReactions<T>(IEnvelope<T> envelope, IUser user) where T : IEvent<T>;
    }
}
