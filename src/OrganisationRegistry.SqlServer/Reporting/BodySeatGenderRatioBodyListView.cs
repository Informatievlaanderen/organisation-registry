namespace OrganisationRegistry.SqlServer.Reporting
{
    using System;
    using System.Collections.Generic;
    using Body;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Organisation;

    public class BodySeatGenderRatioBodyItem
    {
        public Guid BodyId { get; set; }
        public string? BodyName { get; set; }

        // Organisation
        public Guid? OrganisationId { get; set; }
        public string? OrganisationName { get; set; }
        public bool OrganisationIsActive { get; set; }

        public ICollection<BodySeatGenderRatioBodyLifecyclePhaseValidityItem> LifecyclePhaseValidities { get; set; }
        public ICollection<BodySeatGenderRatioPostsPerTypeItem> PostsPerType { get; set; }

        public BodySeatGenderRatioBodyItem()
        {
            LifecyclePhaseValidities = new List<BodySeatGenderRatioBodyLifecyclePhaseValidityItem>();
            PostsPerType = new List<BodySeatGenderRatioPostsPerTypeItem>();
        }
    }

    public class BodySeatGenderRatioBodyListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioBodyItem>
    {
        public const string TableName = "BodySeatGenderRatioBodyList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioBodyItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.BodyId)
                .IsClustered(false);

            b.Property(p => p.BodyId).HasColumnName("BodyId");

            b.Property(p => p.BodyName).HasMaxLength(BodyListConfiguration.NameLength);

            b.Property(p => p.OrganisationId);
            b.Property(p => p.OrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength);
            b.Property(p => p.OrganisationIsActive);

            b.HasMany(p => p.LifecyclePhaseValidities).WithOne(p => p.Body).HasForeignKey(p => p.BodyId);
            b.HasMany(p => p.PostsPerType).WithOne(p => p.Body).HasForeignKey(p => p.BodyId);
        }
    }
}
