namespace OrganisationRegistry.Organisation.Events
{
    using System;
    using System.Collections.Generic;
    using OrganisationTermination;
    using State;

    public class OrganisationTerminated : BaseEvent<OrganisationTerminated>
    {
        public Guid OrganisationId => Id;

        public string Name { get; }
        public string OvoNumber { get; }
        public DateTime DateOfTermination { get; }
        public DateTime? DateOfTerminationAccordingToKbo { get; }
        public FieldsToTerminate FieldsToTerminate { get; }
        public KboFieldsToTerminate KboFieldsToTerminate { get; }
        public bool ForcedKboTermination { get; }

        public OrganisationTerminated(Guid organisationId,
            string name,
            string ovoNumber,
            DateTime dateOfTermination,
            FieldsToTerminate fieldsToTerminate,
            KboFieldsToTerminate kboFieldsToTerminate,
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

        public static OrganisationTerminated Create(Guid id,
            OrganisationState state,
            KboState kboState,
            OrganisationTerminationSummary organisationTermination,
            bool forceKboTermination,
            OrganisationTerminationKboSummary organisationTerminationKboSummary, DateTime dateOfTermination)
        {
            return new OrganisationTerminated(
                id,
                state.Name,
                state.OvoNumber,
                dateOfTermination,
                new FieldsToTerminate(
                    organisationValidity: organisationTermination.OrganisationNewValidTo,
                    buildings: organisationTermination.Buildings,
                    bankAccounts: organisationTermination.BankAccounts,
                    capacities: organisationTermination.Capacities,
                    contacts: organisationTermination.Contacts,
                    classifications: organisationTermination.Classifications,
                    functions: organisationTermination.Functions,
                    labels: organisationTermination.Labels,
                    locations: organisationTermination.Locations,
                    parents: organisationTermination.Parents,
                    relations: organisationTermination.Relations,
                    formalFrameworks: organisationTermination.FormalFrameworks,
                    openingHours: organisationTermination.OpeningHours),
                new KboFieldsToTerminate(
                    bankAccounts: organisationTerminationKboSummary.KboBankAccounts,
                    registeredOffice: organisationTerminationKboSummary.KboRegisteredOfficeLocation,
                    formalName: organisationTerminationKboSummary.KboFormalNameLabel,
                    legalForm: organisationTerminationKboSummary.KboLegalForm
                ),
                forceKboTermination,
                kboState.TerminationInKbo?.Date);
        }
    }

    public class FieldsToTerminate
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
        public Dictionary<Guid, DateTime> Parents { get; }
        public Dictionary<Guid, DateTime> Relations { get; }
        public Dictionary<Guid, DateTime> FormalFrameworks { get; }
        public Dictionary<Guid, DateTime> OpeningHours { get; }
        
        public FieldsToTerminate(
            DateTime? organisationValidity,
            Dictionary<Guid, DateTime> buildings,
            Dictionary<Guid, DateTime> bankAccounts,
            Dictionary<Guid, DateTime> capacities,
            Dictionary<Guid, DateTime> contacts,
            Dictionary<Guid, DateTime> classifications,
            Dictionary<Guid, DateTime> functions,
            Dictionary<Guid, DateTime> labels,
            Dictionary<Guid, DateTime> locations,
            Dictionary<Guid, DateTime> parents,
            Dictionary<Guid, DateTime> relations,
            Dictionary<Guid, DateTime> formalFrameworks,
            Dictionary<Guid, DateTime> openingHours)
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
            Parents = parents;
            Relations = relations;
            FormalFrameworks = formalFrameworks;
            OpeningHours = openingHours;
        }
    }

    public class KboFieldsToTerminate
    {
        public Dictionary<Guid, DateTime> BankAccounts { get; }
        public KeyValuePair<Guid, DateTime>? RegisteredOffice { get; }
        public KeyValuePair<Guid, DateTime>? FormalName { get; }
        public KeyValuePair<Guid, DateTime>? LegalForm { get; }
        
        public KboFieldsToTerminate(
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
