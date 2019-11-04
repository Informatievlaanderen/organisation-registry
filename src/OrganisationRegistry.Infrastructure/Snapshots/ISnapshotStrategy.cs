namespace OrganisationRegistry.Infrastructure.Snapshots
{
    using System;
    using Domain;

    public interface ISnapshotStrategy
    {
        bool ShouldMakeSnapShot(AggregateRoot aggregate);
        bool IsSnapshotable(Type aggregateType);
    }
}
