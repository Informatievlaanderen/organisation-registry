namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ShowOnVlaamseOverheidSitesPerOrganisation
    {
        public Guid Id { get; set; }
        public bool ShowOnVlaamseOverheidSites { get; set; }
    }

    public class ShowOnVlaamseOverheidSitesPerOrganisationListConfiguration : EntityMappingConfiguration<ShowOnVlaamseOverheidSitesPerOrganisation>
    {
        public const string TableName = "ShowOnVlaamseOverheidSitesPerOrganisationList";

        public override void Map(EntityTypeBuilder<ShowOnVlaamseOverheidSitesPerOrganisation> b)
        {
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.Id)
                .IsClustered(false);

            b.Property(p => p.ShowOnVlaamseOverheidSites).IsRequired();
        }
    }
}
