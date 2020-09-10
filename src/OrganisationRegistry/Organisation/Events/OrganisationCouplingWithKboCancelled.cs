namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;

    public class OrganisationCouplingWithKboCancelled : BaseEvent<OrganisationCouplingWithKboCancelled>
    {
        public Guid OrganisationId => Id;

        public string NameBeforeKboCoupling { get; }
        public string ShortNameBeforeKboCoupling { get; }
        public string OvoNumber { get; }
        public Guid? LegalFormOrganisationOrganisationClassificationId { get; }
        public Guid? FormalNameOrganisationLabelId { get; }
        public Guid? RegisteredOfficeOrganisationLocationId { get; }
        public List<Guid> OrganisationBankAccountIds { get; }
        public DateTime? ValidFrom { get; }
        public string PreviousKboNumber { get; }
        public string NameFromKboCoupling { get; }
        public string ShortNameFromKboCoupling { get; }

        public OrganisationCouplingWithKboCancelled(Guid organisationId,
            string previousKboNumber,
            string nameBeforeKboCoupling,
            string shortNameBeforeKboCoupling,
            string nameFromKboCoupling,
            string shortNameFromKboCoupling,
            string ovoNumber,
            Guid? legalFormOrganisationOrganisationClassificationId,
            Guid? formalNameOrganisationLabelId,
            Guid? registeredOfficeOrganisationLocationId,
            List<Guid> organisationBankAccountIds,
            DateTime? validFrom)
        {
            Id = organisationId;

            PreviousKboNumber = previousKboNumber;
            NameBeforeKboCoupling = nameBeforeKboCoupling;
            ShortNameBeforeKboCoupling = shortNameBeforeKboCoupling;
            NameFromKboCoupling = nameFromKboCoupling;
            ShortNameFromKboCoupling = shortNameFromKboCoupling;
            OvoNumber = ovoNumber;
            LegalFormOrganisationOrganisationClassificationId = legalFormOrganisationOrganisationClassificationId;
            FormalNameOrganisationLabelId = formalNameOrganisationLabelId;
            RegisteredOfficeOrganisationLocationId = registeredOfficeOrganisationLocationId;
            OrganisationBankAccountIds = organisationBankAccountIds;
            ValidFrom = validFrom;
        }
    }
}
