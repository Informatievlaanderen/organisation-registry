namespace OrganisationRegistry.Configuration
{
    using Infrastructure.Configuration;

    public class OrganisationManagementConfiguration : IOrganisationManagementConfiguration
    {

        public OrganisationManagementConfiguration(OrganisationManagementConfigurationSection organisationManagementConfigurationSection)
        {
            Vlimpers = organisationManagementConfigurationSection.Vlimpers;
        }

        public string Vlimpers { get; }
    }
}
