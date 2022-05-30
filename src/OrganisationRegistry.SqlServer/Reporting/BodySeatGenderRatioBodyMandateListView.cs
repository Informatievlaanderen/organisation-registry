namespace OrganisationRegistry.SqlServer.Reporting;

using System;
using System.Collections.Generic;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OrganisationRegistry.Infrastructure;

public class BodySeatGenderRatioBodyMandateItem
{
    public Guid BodyMandateId { get; set; }

    public DateTime? BodyMandateValidFrom { get; set; }
    public DateTime? BodyMandateValidTo { get; set; }

    // Body
    public Guid BodyId { get; set; }

    //  BodySeat
    public Guid BodySeatId { get; set; }
    public Guid BodySeatTypeId { get; set; }
    public bool BodySeatTypeIsEffective { get; set; }

    public ICollection<BodySeatGenderRatioAssignmentItem> Assignments { get; set; }

    public BodySeatGenderRatioBodyMandateItem()
    {
        Assignments = new List<BodySeatGenderRatioAssignmentItem>();
    }
}

public class BodySeatGenderRatioBodyMandateListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioBodyMandateItem>
{
    public const string TableName = "BodySeatGenderRatioBodyMandateList";

    public override void Map(EntityTypeBuilder<BodySeatGenderRatioBodyMandateItem> b)
    {
        b.ToTable(TableName, WellknownSchemas.ReportingSchema)
            .HasKey(p => p.BodyMandateId)
            .IsClustered(false);

        b.Property(p => p.BodyMandateValidFrom);
        b.Property(p => p.BodyMandateValidTo);

        b.Property(p => p.BodyId).IsRequired();

        b.Property(p => p.BodySeatId);

        b.Property(p => p.BodySeatTypeId);

        b.HasMany(p => p.Assignments).WithOne(p => p.BodyMandate).OnDelete(DeleteBehavior.Cascade);
    }
}