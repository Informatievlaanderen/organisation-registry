namespace OrganisationRegistry.Infrastructure.Domain
{
    using System;
    using System.Threading.Tasks;

    public interface ISession
    {
        void Add<T>(T aggregate) where T : AggregateRoot;

        T Get<T>(Guid id, int? expectedVersion = null) where T : AggregateRoot;

        Task Commit();
    }
}
