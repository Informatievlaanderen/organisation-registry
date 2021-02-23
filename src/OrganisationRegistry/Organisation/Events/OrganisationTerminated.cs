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
        public DateTime? OrganisationNewValidTo { get; }
        public Dictionary<Guid, DateTime> BuildingsToTerminate { get; }
        public Dictionary<Guid, DateTime> BankAccountsToTerminate { get; }
        public Dictionary<Guid, DateTime> CapacitiesToTerminate { get; }
        public Dictionary<Guid, DateTime> ContactsToTerminate { get; }
        public Dictionary<Guid, DateTime> ClassificationsToTerminate { get; }
        public Dictionary<Guid, DateTime> FunctionsToTerminate { get; }
        public Dictionary<Guid, DateTime> LabelsToTerminate { get; }
        public Dictionary<Guid, DateTime> LocationsToTerminate { get; }
        public Dictionary<Guid, DateTime> ParentsToTerminate { get; }
        public Dictionary<Guid, DateTime> RelationsToTerminate { get; }
        public Dictionary<Guid, DateTime> FormalFrameworksToTerminate { get; }
        public Dictionary<Guid, DateTime> OpeningHoursToTerminate { get; }

        public bool ForcedKboTermination { get; }
        public Dictionary<Guid, DateTime> KboBankAccountsToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboRegisteredOfficeToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboFormalNameToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboLegalFormToTerminate { get; }

        public OrganisationTerminated(Guid organisationId,
            string name,
            string ovoNumber,
            DateTime dateOfTermination,
            DateTime? organisationNewValidTo,
            Dictionary<Guid, DateTime> organisationTerminationBuildings,
            Dictionary<Guid, DateTime> organisationTerminationCapacities,
            Dictionary<Guid, DateTime> organisationTerminationContacts,
            Dictionary<Guid, DateTime> organisationTerminationClassifications,
            Dictionary<Guid, DateTime> organisationTerminationFunctions,
            Dictionary<Guid, DateTime> organisationTerminationLabels,
            Dictionary<Guid, DateTime> organisationTerminationLocations,
            Dictionary<Guid, DateTime> organisationTerminationParents,
            Dictionary<Guid, DateTime> organisationTerminationRelations,
            Dictionary<Guid, DateTime> organisationTerminationBankAccounts,
            Dictionary<Guid, DateTime> organisationTerminationFormalFrameworks,
            Dictionary<Guid, DateTime> organisationTerminationOpeningHours,
            bool forcedKboTermination,
            Dictionary<Guid, DateTime> kboBankAccounts,
            KeyValuePair<Guid, DateTime>? kboRegisteredOfficeToTerminate,
            KeyValuePair<Guid, DateTime>? kboFormalNameToTerminate,
            KeyValuePair<Guid, DateTime>? kboLegalFormToTerminate,
            DateTime? dateOfTerminationAccordingToKbo = null)

        {
            Id = organisationId;

            Name = name;
            OvoNumber = ovoNumber;
            DateOfTermination = dateOfTermination;
            OrganisationNewValidTo = organisationNewValidTo;
            BuildingsToTerminate = organisationTerminationBuildings;
            CapacitiesToTerminate = organisationTerminationCapacities;
            ContactsToTerminate = organisationTerminationContacts;
            ClassificationsToTerminate = organisationTerminationClassifications;
            FunctionsToTerminate = organisationTerminationFunctions;
            LabelsToTerminate = organisationTerminationLabels;
            LocationsToTerminate = organisationTerminationLocations;
            ParentsToTerminate = organisationTerminationParents;
            RelationsToTerminate = organisationTerminationRelations;
            BankAccountsToTerminate = organisationTerminationBankAccounts;
            FormalFrameworksToTerminate = organisationTerminationFormalFrameworks;
            OpeningHoursToTerminate = organisationTerminationOpeningHours;
            DateOfTerminationAccordingToKbo = dateOfTerminationAccordingToKbo;
            ForcedKboTermination = forcedKboTermination;
            KboBankAccountsToTerminate = kboBankAccounts;
            KboRegisteredOfficeToTerminate = kboRegisteredOfficeToTerminate;
            KboFormalNameToTerminate = kboFormalNameToTerminate;
            KboLegalFormToTerminate = kboLegalFormToTerminate;
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
                organisationTermination.OrganisationNewValidTo,
                organisationTermination.Buildings,
                organisationTermination.Capacities,
                organisationTermination.Contacts,
                organisationTermination.Classifications,
                organisationTermination.Functions,
                organisationTermination.Labels,
                organisationTermination.Locations,
                organisationTermination.Parents,
                organisationTermination.Relations,
                organisationTermination.BankAccounts,
                organisationTermination.FormalFrameworks,
                organisationTermination.OpeningHours,
                forceKboTermination,
                organisationTerminationKboSummary.KboBankAccounts,
                organisationTerminationKboSummary.KboRegisteredOfficeLocation,
                organisationTerminationKboSummary.KboFormalNameLabel,
                organisationTerminationKboSummary.KboLegalForm,
                kboState.TerminationInKbo?.Date);
        }
    }
}
