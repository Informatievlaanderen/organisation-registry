namespace OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers
{
    using System;
    using OrganisationRegistry.Infrastructure.Snapshots;

    internal class SpecSnapShotStorage : ISnapshotStore
    {
        public Snapshot Snapshot { get; set; }

        public SpecSnapShotStorage(Snapshot snapshot)
            => Snapshot = snapshot;

        public Snapshot Get(Guid id)
            => Snapshot;

        public void Save(Snapshot snapshot)
            => Snapshot = snapshot;
    }
}
