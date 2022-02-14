namespace OrganisationRegistry.Configuration
{
    using System;

    public interface IAuthorizationConfiguration
    {
        Guid[] FormalFrameworkIdsOwnedByVlimpers { get; }
        Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
        Guid[] LabelIdsAllowedForVlimpers { get; }
        Guid[] KeyIdsAllowedForVlimpers { get; }
    }
}
