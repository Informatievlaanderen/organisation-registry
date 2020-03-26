namespace OrganisationRegistry.SqlServer.Reporting
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class BodySeatGenderRatioBodyLifecyclePhaseValidityItem
    {
        public Guid LifecyclePhaseId { get; set; }
        public Guid BodyId { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }
        public bool RepresentsActivePhase { get; set; }
    }

    public class BodySeatGenderRatioLifecyclePhaseValidityListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioBodyLifecyclePhaseValidityItem>
    {
        public const string TableName = "BodySeatGenderRatioLifecyclePhaseValidityList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioBodyLifecyclePhaseValidityItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.LifecyclePhaseId)
                .IsClustered(false);

            b.Property(p => p.BodyId).IsRequired();
            b.Property(p => p.ValidFrom);
            b.Property(p => p.ValidTo);
            b.Property(p => p.RepresentsActivePhase);
        }
    }
}
