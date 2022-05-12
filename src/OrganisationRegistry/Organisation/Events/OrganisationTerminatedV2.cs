namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;
    using State;


    public class OrganisationTerminatedV2 : BaseEvent<OrganisationTerminatedV2>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public DateTime DateOfTermination { get; }
        public DateTime? DateOfTerminationAccordingToKbo { get; }
        public FieldsToTerminateV2 FieldsToTerminate { get; }
        public KboFieldsToTerminateV2 KboFieldsToTerminate { get; }
        public bool ForcedKboTermination { get; }

        public OrganisationTerminatedV2(Guid organisationId,
            string name,
            string ovoNumber,
            DateTime dateOfTermination,
            FieldsToTerminateV2 fieldsToTerminate,
            KboFieldsToTerminateV2 kboFieldsToTerminate,
            bool forcedKboTermination,
            DateTime? dateOfTerminationAccordingToKbo = null)
        {
            Id = organisationId;

            Name = name;
            OvoNumber = ovoNumber;
            DateOfTermination = dateOfTermination;
            FieldsToTerminate = fieldsToTerminate;
            KboFieldsToTerminate = kboFieldsToTerminate;
            ForcedKboTermination = forcedKboTermination;
            DateOfTerminationAccordingToKbo = dateOfTerminationAccordingToKbo;
        }

        public static OrganisationTerminatedV2 Create(Guid id,
            OrganisationState state,
            KboState kboState,
            OrganisationTerminationSummary organisationTermination,
            bool forceKboTermination,
            OrganisationTerminationKboSummary organisationTerminationKboSummary, DateTime dateOfTermination)
        {
            return new OrganisationTerminatedV2(
                id,
                state.Name,
                state.OvoNumber,
                dateOfTermination,
                new FieldsToTerminateV2(
                    organisationValidity: organisationTermination.OrganisationNewValidTo,
                    buildings: organisationTermination.Buildings,
                    bankAccounts: organisationTermination.BankAccounts,
                    capacities: organisationTermination.Capacities,
                    contacts: organisationTermination.Contacts,
                    classifications: organisationTermination.Classifications,
                    functions: organisationTermination.Functions,
                    labels: organisationTermination.Labels,
                    locations: organisationTermination.Locations,
                    relations: organisationTermination.Relations,
                    formalFrameworks: organisationTermination.FormalFrameworks,
                    openingHours: organisationTermination.OpeningHours,
                    regulations: organisationTermination.Regulations,
                    keys: organisationTermination.Keys),
                new KboFieldsToTerminateV2(
                    bankAccounts: organisationTerminationKboSummary.KboBankAccounts,
                    registeredOffice: organisationTerminationKboSummary.KboRegisteredOfficeLocation,
                    formalName: organisationTerminationKboSummary.KboFormalNameLabel,
                    legalForm: organisationTerminationKboSummary.KboLegalForm
                ),
                forceKboTermination,
                kboState.TerminationInKbo?.Date);
        }
    }

    public class FieldsToTerminateV2
    {
        public DateTime? OrganisationValidity { get; }
        public Dictionary<Guid, DateTime> Buildings { get; }
        public Dictionary<Guid, DateTime> BankAccounts { get; }
        public Dictionary<Guid, DateTime> Capacities { get; }
        public Dictionary<Guid, DateTime> Contacts { get; }
        public Dictionary<Guid, DateTime> Classifications { get; }
        public Dictionary<Guid, DateTime> Functions { get; }
        public Dictionary<Guid, DateTime> Labels { get; }
        public Dictionary<Guid, DateTime> Locations { get; }
        public Dictionary<Guid, DateTime> Relations { get; }
        public Dictionary<Guid, DateTime> FormalFrameworks { get; }
        public Dictionary<Guid, DateTime> OpeningHours { get; }
        public Dictionary<Guid, DateTime> Regulations { get; }
        public Dictionary<Guid, DateTime>? Keys { get; }

        public FieldsToTerminateV2(
            DateTime? organisationValidity,
            Dictionary<Guid, DateTime> buildings,
            Dictionary<Guid, DateTime> bankAccounts,
            Dictionary<Guid, DateTime> capacities,
            Dictionary<Guid, DateTime> contacts,
            Dictionary<Guid, DateTime> classifications,
            Dictionary<Guid, DateTime> functions,
            Dictionary<Guid, DateTime> labels,
            Dictionary<Guid, DateTime> locations,
            Dictionary<Guid, DateTime> relations,
            Dictionary<Guid, DateTime> formalFrameworks,
            Dictionary<Guid, DateTime> openingHours,
            Dictionary<Guid, DateTime> regulations,
            Dictionary<Guid, DateTime> keys)
        {
            OrganisationValidity = organisationValidity;
            Buildings = buildings;
            BankAccounts = bankAccounts;
            Capacities = capacities;
            Contacts = contacts;
            Classifications = classifications;
            Functions = functions;
            Labels = labels;
            Locations = locations;
            Relations = relations;
            FormalFrameworks = formalFrameworks;
            OpeningHours = openingHours;
            Regulations = regulations;
            Keys = keys;
        }
    }

    public class KboFieldsToTerminateV2
    {
        public Dictionary<Guid, DateTime> BankAccounts { get; }
        public KeyValuePair<Guid, DateTime>? RegisteredOffice { get; }
        public KeyValuePair<Guid, DateTime>? FormalName { get; }
        public KeyValuePair<Guid, DateTime>? LegalForm { get; }

        public KboFieldsToTerminateV2(
            Dictionary<Guid, DateTime> bankAccounts,
            KeyValuePair<Guid, DateTime>? registeredOffice,
            KeyValuePair<Guid, DateTime>? formalName,
            KeyValuePair<Guid, DateTime>? legalForm)
        {
            BankAccounts = bankAccounts;
            RegisteredOffice = registeredOffice;
            FormalName = formalName;
            LegalForm = legalForm;
        }
    }
}
