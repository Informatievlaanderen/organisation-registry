namespace OrganisationRegistry.Configuration
{
    using System;

    public interface IAuthorizationConfiguration
    {
        Guid[] FormalFrameworkIdsOwnedByVlimpers { get; }
        Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
    }
}
