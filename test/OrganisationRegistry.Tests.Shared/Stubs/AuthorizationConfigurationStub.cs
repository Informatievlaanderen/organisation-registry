namespace OrganisationRegistry.Tests.Shared.Stubs;

using System;
using Infrastructure.Configuration;

public class AuthorizationConfigurationStub: IAuthorizationConfiguration
{
    public AuthorizationConfigurationStub()
    {
        FormalFrameworkIdsOwnedByVlimpers = Array.Empty<Guid>();
        FormalFrameworkIdsOwnedByAuditVlaanderen = Array.Empty<Guid>();
        FormalFrameworkIdsOwnedByRegelgevingDbBeheerder = Array.Empty<Guid>();
        OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder = Array.Empty<Guid>();
        CapacityIdsOwnedByRegelgevingDbBeheerder = Array.Empty<Guid>();
        LabelIdsAllowedForVlimpers = Array.Empty<Guid>();
        KeyIdsAllowedForVlimpers = Array.Empty<Guid>();
        KeyIdsAllowedOnlyForOrafin = Array.Empty<Guid>();
        OrganisationClassificationTypeIdsOwnedByCjm = Array.Empty<Guid>();
    }
    public Guid[] FormalFrameworkIdsOwnedByVlimpers { get; set; }
    public Guid[] FormalFrameworkIdsOwnedByAuditVlaanderen { get; set; }
    public Guid[] FormalFrameworkIdsOwnedByRegelgevingDbBeheerder { get; set; }
    public Guid[] LabelIdsAllowedForVlimpers { get; set; }
    public Guid[] KeyIdsAllowedForVlimpers { get; set; }
    public Guid[] KeyIdsAllowedOnlyForOrafin { get; set; }
    public Guid[] OrganisationClassificationTypeIdsOwnedByRegelgevingDbBeheerder { get; set; }
    public Guid[] CapacityIdsOwnedByRegelgevingDbBeheerder { get; set; }
    public Guid[] OrganisationClassificationTypeIdsOwnedByCjm { get; set; }
}
