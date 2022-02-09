namespace OrganisationRegistry.Configuration
{
    using System;

    public interface IKboConfiguration
    {
        Guid KboV2FormalNameLabelTypeId { get; }
        Guid KboV2RegisteredOfficeLocationTypeId { get; }
        Guid KboV2LegalFormOrganisationClassificationTypeId { get; }
        Guid[]? OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; }
        Guid[]? OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
        Guid[]? FormalFrameworkIdsToTerminateEndOfNextYear { get; }
    }
}
