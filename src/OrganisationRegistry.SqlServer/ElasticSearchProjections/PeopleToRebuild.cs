namespace OrganisationRegistry.SqlServer.ElasticSearchProjections;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class PersonToRebuild
{
    public Guid PersonId { get; set; }
}

public class PersonToRebuildConfiguration : EntityMappingConfiguration<PersonToRebuild>
{
    public const string TableName = "PeopleToRebuild";

    public override void Map(EntityTypeBuilder<PersonToRebuild> b)
    {
        b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
            .HasKey(p => p.PersonId)
            .IsClustered(false);
    }
}
