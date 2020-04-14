namespace OrganisationRegistry.Infrastructure.Domain
{
    using System;
    using System.Threading.Tasks;

    public interface IRepository
    {
        Task Save<T>(T aggregate, int? expectedVersion = null) where T : AggregateRoot;

        T Get<T>(Guid aggregateId) where T : AggregateRoot;
    }
}
