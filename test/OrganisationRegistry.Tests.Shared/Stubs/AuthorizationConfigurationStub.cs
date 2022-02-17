namespace OrganisationRegistry.Tests.Shared.Stubs
{
    using System;
    using Configuration;

    public class AuthorizationConfigurationStub: IAuthorizationConfiguration
    {
        public AuthorizationConfigurationStub()
        {
            FormalFrameworkIdsOwnedByVlimpers = Array.Empty<Guid>();
            FormalFrameworkIdsOwnedByAuditVlaanderen = Array.Empty<Guid>();
            LabelIdsAllowedForVlimpers = Array.Empty<Guid>();
            KeyIdsAllowedForVlimpers = Array.Empty<Guid>();
            KeyIdsAllowedOnlyForOrafin = Array.Empty<Guid>();
        }
        public Guid[]? FormalFrameworkIdsOwnedByVlimpers { get; }
        public Guid[]? FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
        public Guid[] LabelIdsAllowedForVlimpers { get; set; }
        public Guid[] KeyIdsAllowedForVlimpers { get; }
        public Guid[] KeyIdsAllowedOnlyForOrafin { get; }
    }
}
