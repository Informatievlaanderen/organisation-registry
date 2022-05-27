namespace OrganisationRegistry.SqlServer.Reporting
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using OrganisationRegistry.Infrastructure;
    using SeatType;

    public class BodySeatGenderRatioPostsPerTypeItem
    {
        public Guid Id { get; set; }

        // Body
        public Guid BodyId { get; set; }
        public BodySeatGenderRatioBodyItem Body { get; set; } = null!;

        // Seat
        public Guid BodySeatId { get; set; }
        public bool EntitledToVote { get; set; }
        public DateTime? BodySeatValidFrom { get; set; }
        public DateTime? BodySeatValidTo { get; set; }

        // SeatType
        public Guid BodySeatTypeId { get; set; }
        public string? BodySeatTypeName { get; set; }

        public bool BodySeatTypeIsEffective { get; set; }
    }

    public class BodySeatGenderRatioPostsPerTypeListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioPostsPerTypeItem>
    {
        public const string TableName = "BodySeatGenderRatioPostsPerTypeList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioPostsPerTypeItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ReportingSchema)
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.BodySeatId).IsRequired();
            b.Property(p => p.EntitledToVote).HasDefaultValue(false);
            b.Property(p => p.BodySeatValidFrom);
            b.Property(p => p.BodySeatValidTo);

            b.Property(p => p.BodySeatTypeId);
            b.Property(p => p.BodySeatTypeName).HasMaxLength(SeatTypeListConfiguration.NameLength);

            b.HasOne(p => p.Body);
        }
    }
}
