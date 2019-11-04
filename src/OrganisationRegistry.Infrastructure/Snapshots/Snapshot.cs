namespace OrganisationRegistry.Infrastructure.Snapshots
{
    using System;

    public abstract class Snapshot
    {
        public Guid Id { get; set; }
        public int Version { get; set; }
    }
}
