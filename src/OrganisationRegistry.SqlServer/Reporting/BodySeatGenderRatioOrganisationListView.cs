namespace OrganisationRegistry.SqlServer.Reporting
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;

    public class BodySeatGenderRatioOrganisationListItem
    {
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationActive { get; set; }
    }

    public class BodySeatGenderRatioOrganisationListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioOrganisationListItem>
    {
        public const string TableName = "BodySeatGenderRatio_OrganisationList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioOrganisationListItem> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.OrganisationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();
            b.Property(p => p.OrganisationName).IsRequired();
            b.Property(p => p.OrganisationActive);
        }
    }
}
