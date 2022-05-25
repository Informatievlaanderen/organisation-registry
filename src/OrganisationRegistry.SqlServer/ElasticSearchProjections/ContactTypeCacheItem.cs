namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Infrastructure;

    public class ContactTypeCacheItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
    }

    public class ContactTypeCacheForEsConfiguration : EntityMappingConfiguration<ContactTypeCacheItem>
    {
        public const string TableName = "ContactTypeCache";

        public override void Map(EntityTypeBuilder<ContactTypeCacheItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
        }
    }
}
