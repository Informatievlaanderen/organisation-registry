namespace OrganisationRegistry.SqlServer.KboSyncQueue
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class KboSyncQueueItem
    {
        public Guid Id { get; set; }
        public string SourceFileName { get; set; }
        public string SourceKboNumber { get; set; }
        public string SourceName { get; set; }
        public DateTimeOffset SourceAddressModifiedAt { get; set; }
        public DateTimeOffset SourceModifiedAt { get; set; }
        public DateTimeOffset MutationReadAt { get; set; }
        public DateTimeOffset? SyncCompletedAt { get; set; }
        public string SyncStatus { get; set; }
        public string SyncInfo { get; set; }
    }

    public class KboSyncQueueItemConfiguration : EntityMappingConfiguration<KboSyncQueueItem>
    {
        public override void Map(EntityTypeBuilder<KboSyncQueueItem> b)
        {
            b.ToTable("KboSyncQueue", "Magda")
                .HasKey(p => p.Id);

            b.Property(p => p.SourceFileName);
            b.Property(p => p.SourceKboNumber);
            b.Property(p => p.SourceName);
            b.Property(p => p.SourceAddressModifiedAt);
            b.Property(p => p.SourceModifiedAt);
            b.Property(p => p.SyncCompletedAt);
            b.Property(p => p.MutationReadAt);
            b.Property(p => p.SyncStatus);
            b.Property(p => p.SyncInfo);
        }
    }
}
