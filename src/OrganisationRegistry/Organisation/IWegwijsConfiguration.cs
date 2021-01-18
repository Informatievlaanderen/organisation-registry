namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;

    public interface IOrganisationRegistryConfiguration
    {
        Guid KboKeyTypeId { get; }

        Guid KboLegalFormClassificationTypeId { get; }

        [Obsolete("Use KboV2RegisteredOfficeLocationTypeId instead.")]
        Guid RegisteredOfficeLocationTypeId { get; }

        Guid KboV2FormalNameLabelTypeId { get; }

        Guid KboV2RegisteredOfficeLocationTypeId { get; }

        Guid KboV2LegalFormOrganisationClassificationTypeId { get; }
        Guid[] OrganisationCapacityTypeIdsToTerminateEndOfNextYear { get; }
        Guid[] OrganisationClassificationTypeIdsToTerminateEndOfNextYear { get; }
    }
}
