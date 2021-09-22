namespace OrganisationRegistry.Configuration.Database
{
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;

    public class ConfigurationValue
    {
        public string Key { get; set; }
        public string Description { get; set; }
        public string Value { get; set; }

        public ConfigurationValue() { }

        public ConfigurationValue(string key, string description, string value)
        {
            Key = key;
            Description = description;
            Value = value;
        }
    }

    public class ConfigurationValueMapping : EntityMappingConfiguration<ConfigurationValue>
    {
        public override void Map(EntityTypeBuilder<ConfigurationValue> b)
        {
            b.ToTable("Configuration", "OrganisationRegistry")
                .HasKey(p => p.Key)
                .IsClustered();

            b.Property(p => p.Description);
            b.Property(p => p.Value);
        }
    }
}
