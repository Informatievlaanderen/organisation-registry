namespace OrganisationRegistry.SqlServer.Reporting;

using System;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class BodySeatGenderRatioOrganisationClassificationItem
{
    public Guid OrganisationOrganisationClassificationId { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid OrganisationClassificationId { get; set; }
    public Guid OrganisationClassificationTypeId { get; set; }

    public DateTime? ClassificationValidFrom { get; set; }
    public DateTime? ClassificationValidTo { get; set; }
}

public class BodySeatGenderRatioOrganisationClassificationListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioOrganisationClassificationItem>
{
    public const string TableName = "BodySeatGenderRatioOrganisationClassificationList";

    public override void Map(EntityTypeBuilder<BodySeatGenderRatioOrganisationClassificationItem> b)
    {
        b.ToTable(TableName, WellknownSchemas.ReportingSchema)
            .HasKey(p => p.OrganisationOrganisationClassificationId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();
        b.Property(p => p.OrganisationClassificationId).IsRequired();
        b.Property(p => p.OrganisationClassificationTypeId).IsRequired();

        b.Property(p => p.ClassificationValidFrom);
        b.Property(p => p.ClassificationValidTo);
    }
}