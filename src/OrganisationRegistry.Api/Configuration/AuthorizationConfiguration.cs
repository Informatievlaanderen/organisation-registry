namespace OrganisationRegistry.Api.Configuration
{
    using System;
    using OrganisationRegistry.Configuration;

    public class AuthorizationConfiguration : IAuthorizationConfiguration
    {
        public Guid[]? FormalFrameworkIdsOwnedByVlimpers { get; init; }
        public Guid[]? FormalFrameworkIdsOwnedByAuditVlaanderen { get; init; }
    }
}
