namespace OrganisationRegistry.SqlServer.ProjectionState
{
    using System;
    using System.Runtime.Serialization;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Infrastructure;

    public class ProjectionStateItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public int EventNumber { get; set; }
        public DateTimeOffset? LastUpdatedUtc { get; set; }
    }

    public class ProjectionStateListConfiguration : EntityMappingConfiguration<ProjectionStateItem>
    {
        public const int NameLength = 500;

        public override void Map(EntityTypeBuilder<ProjectionStateItem> b)
        {
            b.ToTable("ProjectionStateList", WellknownSchemas.BackofficeSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
            b.Property(p => p.EventNumber).IsRequired();
            b.Property(p => p.LastUpdatedUtc);

            b.HasIndex(x => x.Name).IsUnique().IsClustered();
        }
    }
}
