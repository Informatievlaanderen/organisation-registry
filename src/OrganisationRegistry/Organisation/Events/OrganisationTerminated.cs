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
                    organisationTermination.OrganisationNewValidTo,
                    organisationTermination.Buildings,
                    organisationTermination.BankAccounts,
                    organisationTermination.Capacities,
                    organisationTermination.Contacts,
                    organisationTermination.Classifications,
                    organisationTermination.Functions,
                    organisationTermination.Labels,
                    organisationTermination.Locations,
                    organisationTermination.Parents,
                    organisationTermination.Relations,
                    organisationTermination.FormalFrameworks,
                    organisationTermination.OpeningHours),
                new KboFieldsToTerminate(
                    organisationTerminationKboSummary.KboBankAccounts,
                    organisationTerminationKboSummary.KboRegisteredOfficeLocation,
                    organisationTerminationKboSummary.KboFormalNameLabel,
                    organisationTerminationKboSummary.KboLegalForm
                ),
                forceKboTermination,
                kboState.TerminationInKbo?.Date);
        }
    }

    public class FieldsToTerminate
    {
        public FieldsToTerminate(DateTime? organisationNewValidTo,
            Dictionary<Guid, DateTime> buildingsToTerminate,
            Dictionary<Guid, DateTime> bankAccountsToTerminate,
            Dictionary<Guid, DateTime> capacitiesToTerminate,
            Dictionary<Guid, DateTime> contactsToTerminate,
            Dictionary<Guid, DateTime> classificationsToTerminate,
            Dictionary<Guid, DateTime> functionsToTerminate,
            Dictionary<Guid, DateTime> labelsToTerminate,
            Dictionary<Guid, DateTime> locationsToTerminate,
            Dictionary<Guid, DateTime> parentsToTerminate,
            Dictionary<Guid, DateTime> relationsToTerminate,
            Dictionary<Guid, DateTime> formalFrameworksToTerminate,
            Dictionary<Guid, DateTime> openingHoursToTerminate)
        {
            OrganisationNewValidTo = organisationNewValidTo;
            BuildingsToTerminate = buildingsToTerminate;
            BankAccountsToTerminate = bankAccountsToTerminate;
            CapacitiesToTerminate = capacitiesToTerminate;
            ContactsToTerminate = contactsToTerminate;
            ClassificationsToTerminate = classificationsToTerminate;
            FunctionsToTerminate = functionsToTerminate;
            LabelsToTerminate = labelsToTerminate;
            LocationsToTerminate = locationsToTerminate;
            ParentsToTerminate = parentsToTerminate;
            RelationsToTerminate = relationsToTerminate;
            FormalFrameworksToTerminate = formalFrameworksToTerminate;
            OpeningHoursToTerminate = openingHoursToTerminate;
        }

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
    }

    public class KboFieldsToTerminate
    {
        public KboFieldsToTerminate(Dictionary<Guid, DateTime> kboBankAccountsToTerminate,
            KeyValuePair<Guid, DateTime>? kboRegisteredOfficeToTerminate,
            KeyValuePair<Guid, DateTime>? kboFormalNameToTerminate,
            KeyValuePair<Guid, DateTime>? kboLegalFormToTerminate)
        {
            KboBankAccountsToTerminate = kboBankAccountsToTerminate;
            KboRegisteredOfficeToTerminate = kboRegisteredOfficeToTerminate;
            KboFormalNameToTerminate = kboFormalNameToTerminate;
            KboLegalFormToTerminate = kboLegalFormToTerminate;
        }

        public Dictionary<Guid, DateTime> KboBankAccountsToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboRegisteredOfficeToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboFormalNameToTerminate { get; }
        public KeyValuePair<Guid, DateTime>? KboLegalFormToTerminate { get; }
    }
}
