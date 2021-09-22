﻿namespace OrganisationRegistry.SqlServer.Reporting
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Person;

    public class BodySeatGenderRatioAssignmentItem
    {
        public Guid Id { get; set; }

        public Guid BodyMandateId { get; set; }
        public BodySeatGenderRatioBodyMandateItem BodyMandate { get; set; }


        public Guid? DelegationAssignmentId { get; set; }

        public DateTime? AssignmentValidFrom { get; set; }
        public DateTime? AssignmentValidTo { get; set; }

        public Guid PersonId { get; set; }
        public Sex? Sex { get; set; }
    }

    public class BodySeatGenderRatioAssignmentListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioAssignmentItem>
    {
        public const string TableName = "BodySeatGenderRatioAssignmentList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioAssignmentItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.BodyMandateId).IsRequired();

            b.Property(p => p.DelegationAssignmentId);

            b.Property(p => p.AssignmentValidFrom);
            b.Property(p => p.AssignmentValidTo);

            b.Property(p => p.PersonId).IsRequired();
            b.Property(p => p.Sex);

            b.HasOne(p => p.BodyMandate);
        }
    }
}
