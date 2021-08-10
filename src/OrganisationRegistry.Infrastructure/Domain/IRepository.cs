namespace OrganisationRegistry.Infrastructure.Domain
{
    using System;
    using System.Threading.Tasks;
    using Authorization;

    public interface IRepository
    {
        Task Save<T>(T aggregate, int? expectedVersion = null, IUser? user = null) where T : AggregateRoot;

        T Get<T>(Guid aggregateId) where T : AggregateRoot;
    }
}
