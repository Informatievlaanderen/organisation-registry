namespace OrganisationRegistry.Infrastructure.Snapshots
{
    using System;

    public interface ISnapshotStore
    {
        Snapshot Get(Guid id);
        void Save(Snapshot snapshot);
    }
}
