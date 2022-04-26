namespace OrganisationRegistry.Infrastructure.Configuration
{
    using System;

    public interface IKboConfiguration
    {
        Guid KboV2FormalNameLabelTypeId { get; }
        Guid KboV2RegisteredOfficeLocationTypeId { get; }
        Guid KboV2LegalFormOrganisationClassificationTypeId { get; }
        Guid[]? OrganisationCapacityIdsToTerminateEndOfNextYear { get; }
        Guid[]? OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
        Guid[]? FormalFrameworkIdsToTerminateEndOfNextYear { get; }
    }
}
