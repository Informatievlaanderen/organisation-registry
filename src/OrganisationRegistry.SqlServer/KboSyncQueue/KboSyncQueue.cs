namespace OrganisationRegistry.SqlServer.KboSyncQueue
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class KboSyncQueueItem
    {
        public Guid Id { get; set; }
        public string? SourceFileName { get; set; }
        public string? SourceOrganisationKboNumber { get; set; }
        public string? SourceOrganisationName { get; set; }
        public DateTimeOffset SourceOrganisationModifiedAt { get; set; }
        public DateTimeOffset MutationReadAt { get; set; }
        public DateTimeOffset? SyncCompletedAt { get; set; }
        public string? SyncStatus { get; set; }
        public string? SyncInfo { get; set; }
    }

    public class KboSyncQueueItemConfiguration : EntityMappingConfiguration<KboSyncQueueItem>
    {
        public override void Map(EntityTypeBuilder<KboSyncQueueItem> b)
        {
            b.ToTable("KboSyncQueue", "Magda")
                .HasKey(p => p.Id);

            b.Property(p => p.SourceFileName);
            b.Property(p => p.SourceOrganisationKboNumber);
            b.Property(p => p.SourceOrganisationName);
            b.Property(p => p.SourceOrganisationModifiedAt);
            b.Property(p => p.SyncCompletedAt);
            b.Property(p => p.MutationReadAt);
            b.Property(p => p.SyncStatus);
            b.Property(p => p.SyncInfo);
        }
    }
}
