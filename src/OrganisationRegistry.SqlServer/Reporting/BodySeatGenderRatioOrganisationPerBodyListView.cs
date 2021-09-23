namespace OrganisationRegistry.SqlServer.Reporting
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
    using OrganisationRegistry.Infrastructure;

    public class BodySeatGenderRatioOrganisationPerBodyListItem
    {
        public Guid BodyId { get; set; }
        public Guid BodyOrganisationId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
        public bool OrganisationActive { get; set; }
    }

    public class BodySeatGenderRatioOrganisationPerBodyListConfiguration : EntityMappingConfiguration<BodySeatGenderRatioOrganisationPerBodyListItem>
    {
        public const string TableName = "BodySeatGenderRatio_OrganisationPerBodyList";

        public override void Map(EntityTypeBuilder<BodySeatGenderRatioOrganisationPerBodyListItem> b)
        {
            b.ToTable(TableName, WellknownSchemas.ReportingSchema)
                .HasKey(p => p.BodyId)
                .IsClustered(false);

            b.Property(p => p.BodyOrganisationId).IsRequired();
            b.Property(p => p.OrganisationId).IsRequired();
            b.Property(p => p.OrganisationName).IsRequired();
            b.Property(p => p.OrganisationActive);
        }
    }
}
