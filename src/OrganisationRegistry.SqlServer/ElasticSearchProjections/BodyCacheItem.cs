namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Infrastructure;

    public class BodyCacheItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
    }

    public class BodyCacheForEsConfiguration : EntityMappingConfiguration<BodyCacheItem>
    {
        public const string TableName = "BodyCache";

        public override void Map(EntityTypeBuilder<BodyCacheItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
        }
    }
}
