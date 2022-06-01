namespace OrganisationRegistry.Tests.Shared.Stubs;

using System;
using Infrastructure.Configuration;

public class KboConfigurationStub: IKboConfiguration
{
    public KboConfigurationStub()
    {
        OrganisationCapacityIdsToTerminateEndOfNextYear = Array.Empty<Guid>();
        OrganisationClassificationTypeIdsToTerminateEndOfNextYear = Array.Empty<Guid>();
        FormalFrameworkIdsToTerminateEndOfNextYear = Array.Empty<Guid>();
    }
    public Guid KboV2FormalNameLabelTypeId { get; set; }
    public Guid KboV2RegisteredOfficeLocationTypeId { get; set; }
    public Guid KboV2LegalFormOrganisationClassificationTypeId { get; set; }
    public Guid[]? OrganisationCapacityIdsToTerminateEndOfNextYear { get; set; }
    public Guid[]? OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; set; }
    public Guid[]? FormalFrameworkIdsToTerminateEndOfNextYear { get; set; }
}
