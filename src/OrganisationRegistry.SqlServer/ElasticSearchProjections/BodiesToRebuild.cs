namespace OrganisationRegistry.SqlServer.ElasticSearchProjections;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class BodyToRebuild
{
    public Guid BodyId { get; set; }
}

public class BodyToRebuildConfiguration : EntityMappingConfiguration<BodyToRebuild>
{
    public const string TableName = "BodiesToRebuild";

    public override void Map(EntityTypeBuilder<BodyToRebuild> b)
    {
        b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
            .HasKey(p => p.BodyId)
            .IsClustered(false);
    }
}
