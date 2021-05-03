namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class OrganisationCacheItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string OvoNumber { get; set; }
    }

    public class OrganisationCacheForEsConfiguration : EntityMappingConfiguration<OrganisationCacheItem>
    {
        public const string TableName = "OrganisationCache";

        public override void Map(EntityTypeBuilder<OrganisationCacheItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
            b.Property(p => p.OvoNumber).IsRequired();
        }
    }
}
