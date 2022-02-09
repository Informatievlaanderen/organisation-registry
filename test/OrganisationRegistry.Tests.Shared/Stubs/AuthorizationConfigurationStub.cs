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
        }
        public Guid[]? FormalFrameworkIdsOwnedByVlimpers { get; set; }
        public Guid[]? FormalFrameworkIdsOwnedByAuditVlaanderen { get; set; }
    }
}
