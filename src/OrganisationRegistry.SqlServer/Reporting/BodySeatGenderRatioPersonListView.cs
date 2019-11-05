namespace OrganisationRegistry.SqlServer.Reporting
{
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using System;
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
            b.ToTable(TableName, "OrganisationRegistry")
                .HasKey(p => p.PersonId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.PersonSex);
        }
    }
}
