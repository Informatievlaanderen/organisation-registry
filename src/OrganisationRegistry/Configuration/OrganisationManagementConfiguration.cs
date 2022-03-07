namespace OrganisationRegistry.Configuration
{
    using Infrastructure.Configuration;

    public class OrganisationManagementConfigurationConfiguration : IOrganisationManagementConfiguration
    {

        public OrganisationManagementConfigurationConfiguration(OrganisationManagementConfigurationSection organisationManagementConfigurationSection)
        {
            Vlimpers = organisationManagementConfigurationSection.Vlimpers;
        }

        public string Vlimpers { get; }
    }
}
