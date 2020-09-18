#pragma warning disable 8618
namespace OrganisationRegistry.SqlServer.KboSyncQueue
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class KboTerminationSyncQueueItem
    {
        public Guid Id { get; set; }
        public string SourceFileName { get; set; }
        public string SourceOrganisationKboNumber { get; set; }
        public string SourceOrganisationName { get; set; }
        public DateTimeOffset SourceOrganisationModifiedAt { get; set; }
        public string SourceOrganisationTerminationCode { get; set; }
        public string SourceOrganisationTerminationReason { get; set; }
        public DateTimeOffset SourceOrganisationTerminationDate { get; set; }
        public DateTimeOffset MutationReadAt { get; set; }
        public DateTimeOffset? SyncCompletedAt { get; set; }
        public string? SyncStatus { get; set; }
        public string? SyncInfo { get; set; }
    }

    public class KboTerminationQueueItemConfiguration : EntityMappingConfiguration<KboTerminationSyncQueueItem>
    {
        public override void Map(EntityTypeBuilder<KboTerminationSyncQueueItem> b)
        {
            b.ToTable("KboTerminationSyncQueue", "Magda")
                .HasKey(p => p.Id);

            b.Property(p => p.SourceFileName);
            b.Property(p => p.SourceOrganisationKboNumber);
            b.Property(p => p.SourceOrganisationName);
            b.Property(p => p.SourceOrganisationModifiedAt);
            b.Property(p => p.SourceOrganisationTerminationCode);
            b.Property(p => p.SourceOrganisationTerminationReason);
            b.Property(p => p.SourceOrganisationTerminationDate);
            b.Property(p => p.SyncCompletedAt);
            b.Property(p => p.MutationReadAt);
            b.Property(p => p.SyncStatus);
            b.Property(p => p.SyncInfo);
        }
    }
}
