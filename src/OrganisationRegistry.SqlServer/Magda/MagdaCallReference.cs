namespace OrganisationRegistry.SqlServer.Magda;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class MagdaCallReference
{
    public Guid Reference { get; set; }
    public string? UserClaims { get; set; }
    public DateTimeOffset CalledAt { get; set; }
}

public class MagdaReferencesConfiguration : EntityMappingConfiguration<MagdaCallReference>
{
    public override void Map(EntityTypeBuilder<MagdaCallReference> b)
    {
        b.ToTable("CallReferences", WellknownSchemas.MagdaSchema)
            .HasKey(p => p.Reference);

        b.Property(p => p.UserClaims);
        b.Property(p => p.CalledAt);
    }
}