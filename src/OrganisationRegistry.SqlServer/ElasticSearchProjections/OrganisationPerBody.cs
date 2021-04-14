namespace OrganisationRegistry.SqlServer.ElasticSearchProjections
{
    using System;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    /// <summary>
    /// Cache for the ElasticSearch projection.
    /// This table serves as a cache by keeping the organisation id and name for each body.
    /// If a body does not occur in this table, it can be assumed the body does not currently have an organisation
    /// assigned to it.
    /// </summary>
    public class OrganisationPerBody
    {
        public Guid BodyId { get; set; }
        public Guid OrganisationId { get; set; }
        public string OrganisationName { get; set; }
    }

    public class OrganisationPerBodyListConfiguration : EntityMappingConfiguration<OrganisationPerBody>
    {
        public const string TableName = "OrganisationPerBodyListForES";

        public override void Map(EntityTypeBuilder<OrganisationPerBody> b)
        {
            b.ToTable(TableName, WellknownSchemas.ElasticSearchProjectionsSchema)
                .HasKey(p => p.BodyId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();
            b.Property(p => p.OrganisationName).IsRequired();
        }
    }
}
