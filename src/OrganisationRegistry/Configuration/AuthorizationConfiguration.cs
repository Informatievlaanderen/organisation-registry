namespace OrganisationRegistry.Api.Configuration
{
    using System;
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

            OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder =
                authorizationConfiguration?.OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder.SplitGuids() ??
                Array.Empty<Guid>();

            CapacityIdsOwnedByRegelgevingDbBeheerder =
                authorizationConfiguration?.CapacityIdsOwnedByRegelgevingDbBeheerder.SplitGuids() ??
                Array.Empty<Guid>();

            FormalFrameworkIdsOwnedByRegelgevingDbBeheerder =
                authorizationConfiguration?.FormalFrameworkIdsOwnedByRegelgevingDbBeheerder.SplitGuids() ??
                Array.Empty<Guid>();

            LabelIdsAllowedForVlimpers =
                authorizationConfiguration?.LabelIdsAllowedForVlimpers.SplitGuids() ??
                Array.Empty<Guid>();

            KeyIdsAllowedForVlimpers =
                authorizationConfiguration?.KeyIdsAllowedForVlimpers.SplitGuids() ??
                Array.Empty<Guid>();

            KeyIdsAllowedOnlyForOrafin =
                authorizationConfiguration?.KeyIdsAllowedOnlyForOrafin.SplitGuids() ??
                Array.Empty<Guid>();
        }
        public Guid[] FormalFrameworkIdsOwnedByVlimpers { get; }
        public Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
        public Guid[] FormalFrameworkIdsOwnedByRegelgevingDbBeheerder { get; }
        public Guid[] LabelIdsAllowedForVlimpers { get; }
        public Guid[] KeyIdsAllowedForVlimpers { get; }
        public Guid[] KeyIdsAllowedOnlyForOrafin { get; }
        public Guid[] OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder { get; }
        public Guid[] CapacityIdsOwnedByRegelgevingDbBeheerder { get; }
    }
}
