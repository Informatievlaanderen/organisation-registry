namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using Infrastructure.OrganisationRegistryConfiguration;
    using OrganisationRegistry.Configuration;
    using OrganisationRegistry.Infrastructure.Configuration;

    public class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public AuthorizationConfiguration(AuthorizationConfigurationSection? authorizationConfiguration)
        {
            FormalFrameworkIdsOwnedByVlimpers =
                authorizationConfiguration?.FormalFrameworkIdsOwnedByVlimpers.SplitGuids() ??
                Array.Empty<Guid>();

            FormalFrameworkIdsOwnedByAuditVlaanderen =
                authorizationConfiguration?.FormalFrameworkIdsOwnedByAuditVlaanderen.SplitGuids() ??
                Array.Empty<Guid>();
        }
        public Guid[] FormalFrameworkIdsOwnedByVlimpers { get; }
        public Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
    }
}
