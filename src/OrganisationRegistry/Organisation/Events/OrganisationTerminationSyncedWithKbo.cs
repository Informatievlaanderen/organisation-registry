namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;

    public class OrganisationTerminationSyncedWithKbo : BaseEvent<OrganisationTerminationSyncedWithKbo>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public DateTime? DateOfTermination { get; }
        public Guid? LegalFormOrganisationOrganisationClassificationIdToTerminate { get; }
        public Guid? FormalNameOrganisationLabelIdToTerminate { get; }
        public Guid? RegisteredOfficeOrganisationLocationIdToTerminate { get; }
        public List<Guid> OrganisationBankAccountIdsToTerminate { get; }
        public string KboNumber { get; }

        public OrganisationTerminationSyncedWithKbo(
            Guid organisationId,
            string kboNumber,
            string name,
            string ovoNumber,
            DateTime? dateOfTermination,
            Guid? legalFormOrganisationOrganisationClassificationIdToTerminate,
            Guid? formalNameOrganisationLabelIdToTerminate,
            Guid? registeredOfficeOrganisationLocationIdToTerminate,
            List<Guid> organisationBankAccountIdsToTerminate)
        {
            Id = organisationId;

            KboNumber = kboNumber;
            Name = name;
            OvoNumber = ovoNumber;
            DateOfTermination = dateOfTermination;
            LegalFormOrganisationOrganisationClassificationIdToTerminate = legalFormOrganisationOrganisationClassificationIdToTerminate;
            FormalNameOrganisationLabelIdToTerminate = formalNameOrganisationLabelIdToTerminate;
            RegisteredOfficeOrganisationLocationIdToTerminate = registeredOfficeOrganisationLocationIdToTerminate;
            OrganisationBankAccountIdsToTerminate = organisationBankAccountIdsToTerminate;
        }
    }
}
