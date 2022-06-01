namespace OrganisationRegistry.Infrastructure.Configuration;

using System;

public class KboConfiguration : IKboConfiguration
{
    private readonly ApiConfigurationSection _configuration;

    public KboConfiguration(
        ApiConfigurationSection configuration,
        OrganisationTerminationConfigurationSection? terminationConfiguration)
    {
        _configuration = configuration;

        OrganisationCapacityIdsToTerminateEndOfNextYear =
            terminationConfiguration?.OrganisationCapacityIdsToTerminateEndOfNextYear.SplitGuids();

        OrganisationClassificationTypeIdsToTerminateEndOfNextYear =
            terminationConfiguration?.OrganisationClassificationTypeIdsToTerminateEndOfNextYear.SplitGuids();

        FormalFrameworkIdsToTerminateEndOfNextYear =
            terminationConfiguration?.FormalFrameworkIdsToTerminateEndOfNextYear.SplitGuids();
    }
    public Guid KboV2FormalNameLabelTypeId => _configuration.KboV2FormalNameLabelTypeId;

    public Guid KboV2RegisteredOfficeLocationTypeId => _configuration.KboV2RegisteredOfficeLocationTypeId;

    public Guid KboV2LegalFormOrganisationClassificationTypeId => _configuration.KboV2LegalFormOrganisationClassificationTypeId;

    public Guid[]? OrganisationCapacityIdsToTerminateEndOfNextYear { get; }
    public Guid[]? OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
    public Guid[]? FormalFrameworkIdsToTerminateEndOfNextYear { get; }
}
