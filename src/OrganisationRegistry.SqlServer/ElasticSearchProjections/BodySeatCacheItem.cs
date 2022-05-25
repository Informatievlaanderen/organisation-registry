namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Infrastructure;

    public class BodySeatCacheItem
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = null!;
        public string Number { get; set; } = null!;
        public bool IsPaid { get; set; }
    }

    public class BodySeatCacheForEsConfiguration : EntityMappingConfiguration<BodySeatCacheItem>
    {
        public const string TableName = "BodySeatCache";

        public override void Map(EntityTypeBuilder<BodySeatCacheItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.Name).IsRequired();
            b.Property(p => p.Number).IsRequired();
            b.Property(p => p.IsPaid).IsRequired();
        }
    }
}
