namespace OrganisationRegistry.SqlServer.ElasticSearchProjections;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class OrganisationToRebuild
{
    public Guid OrganisationId { get; set; }
}

public class OrganisationToRebuildConfiguration : EntityMappingConfiguration<OrganisationToRebuild>
{
    public const string TableName = "OrganisationsToRebuild";

    public override void Map(EntityTypeBuilder<OrganisationToRebuild> b)
    {
        b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
            .HasKey(p => p.OrganisationId)
            .IsClustered(false);
    }
}