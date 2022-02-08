namespace OrganisationRegistry.Organisation
{
    using System;

    public interface IOrganisationRegistryConfiguration
    {
        Guid OrafinKeyTypeId { get; }
        Guid VlimpersKeyTypeId { get; }
        string OrafinOvoCode { get; }
        Guid FormalNameLabelTypeId { get; }
        Guid FormalShortNameLabelTypeId { get; }
        public IKboConfiguration Kbo { get; }
        public IAuthorizationConfiguration Authorization { get; }
    }

    public interface IAuthorizationConfiguration
    {
        Guid[]? FormalFrameworkIdsAllowedForVlimpers { get; init; }
        Guid[]? FormalFrameworkIdsNotAllowedForOrganisationRegistryBeheerders { get; init; }
    }

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
