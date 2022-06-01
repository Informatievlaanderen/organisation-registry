namespace OrganisationRegistry.Infrastructure.Configuration;

using System;

public interface IAuthorizationConfiguration
{
    Guid[] FormalFrameworkIdsOwnedByVlimpers { get; }
    Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; }
    Guid[] FormalFrameworkIdsOwnedByRegelgevingDbBeheerder { get; }
    Guid[] LabelIdsAllowedForVlimpers { get; }
    Guid[] KeyIdsAllowedForVlimpers { get; }
    Guid[] KeyIdsAllowedOnlyForOrafin { get; }
    Guid[] OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder { get; }
    Guid[] CapacityIdsOwnedByRegelgevingDbBeheerder { get; }
}
