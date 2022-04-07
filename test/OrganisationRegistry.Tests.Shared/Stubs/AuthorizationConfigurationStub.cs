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
        public Guid[]? FormalFrameworkIdsOwnedByVlimpers { get; set; }
        public Guid[]? FormalFrameworkIdsOwnedByAuditVlaanderen { get; set; }
        public Guid[] LabelIdsAllowedForVlimpers { get; set; }
        public Guid[] KeyIdsAllowedForVlimpers { get; set; }
        public Guid[] KeyIdsAllowedOnlyForOrafin { get; set; }
    }
}