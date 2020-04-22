namespace OrganisationRegistry.SqlServer.Configuration
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Infrastructure;

    public class ConfigurationListItem
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }
    }

    public class ConfigurationListConfiguration : EntityMappingConfiguration<ConfigurationListItem>
    {
        public override void Map(EntityTypeBuilder<ConfigurationListItem> b)
        {
            b.ToTable("Configuration", "OrganisationRegistry")
                .HasKey(p => p.Key)
                .IsClustered();

            b.Property(p => p.Key)
                .IsRequired()
                .HasMaxLength(450);

            b.Property(p => p.Description);

            b.Property(p => p.Value);
        }
    }
}
