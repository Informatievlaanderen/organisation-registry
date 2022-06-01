namespace OrganisationRegistry.SqlServer.Reporting;

using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Person;

public class BodySeatGenderRatioPersonListItem
{
    public Guid PersonId { get; set; }
    public Sex? PersonSex { get; set; }
}

public class BodySeatGenderRatioPersonListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioPersonListItem>
{
    public const string TableName = "BodySeatGenderRatio_PersonList";

    public override void Map(EntityTypeBuilder<BodySeatGenderRatioPersonListItem> b)
    {
        b.ToTable(TableName, WellknownSchemas.ReportingSchema)
            .HasKey(p => p.PersonId)
            .IsClustered(false);

        b.Property(p => p.PersonSex);
    }
}
