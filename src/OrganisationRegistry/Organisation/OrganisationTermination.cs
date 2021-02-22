namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using State;

    public class OrganisationTerminationSummary
    {
        public DateTime? OrganisationNewValidTo { get; init; }
        public Dictionary<Guid, DateTime> Contacts { get; init; }
        public Dictionary<Guid, DateTime> BankAccounts { get; init; }
        public Dictionary<Guid, DateTime> Functions { get; init; }
        public Dictionary<Guid, DateTime> Locations { get; init; }
        public Dictionary<Guid, DateTime> Capacities { get; init; }
        public Dictionary<Guid, DateTime> Buildings { get; init; }
        public Dictionary<Guid, DateTime> Parents { get; init; }
        public Dictionary<Guid, DateTime> Labels { get; init; }
        public Dictionary<Guid, DateTime> Relations { get; init; }
        public Dictionary<Guid, DateTime> OpeningHours { get; init; }
        public Dictionary<Guid, DateTime> Classifications { get; init; }
        public Dictionary<Guid, DateTime> FormalFrameworks { get; init; }

        public OrganisationTerminationSummary()
        {
            Contacts = new Dictionary<Guid, DateTime>();
            BankAccounts = new Dictionary<Guid, DateTime>();
            Functions = new Dictionary<Guid, DateTime>();
            Locations = new Dictionary<Guid, DateTime>();
            Capacities = new Dictionary<Guid, DateTime>();
            Buildings = new Dictionary<Guid, DateTime>();
            Parents = new Dictionary<Guid, DateTime>();
            Labels = new Dictionary<Guid, DateTime>();
            Relations = new Dictionary<Guid, DateTime>();
            OpeningHours = new Dictionary<Guid, DateTime>();
            Classifications = new Dictionary<Guid, DateTime>();
            FormalFrameworks = new Dictionary<Guid, DateTime>();
        }
    }

    public class OrganisationTerminationKboSummary
    {
        public Dictionary<Guid, DateTime> KboBankAccounts { get; init; }
        public KeyValuePair<Guid, DateTime>? KboRegisteredOfficeLocation { get; init; }
        public KeyValuePair<Guid, DateTime>? KboFormalNameLabel { get; init; }
        public KeyValuePair<Guid, DateTime>? KboLegalForm { get; init; }

        public OrganisationTerminationKboSummary()
        {
            KboBankAccounts = new Dictionary<Guid, DateTime>();
        }

    }

    public static class OrganisationTermination
    {

        public static OrganisationTerminationKboSummary CalculateForcedKboTermination(DateTime dateOfTermination, KboState kboState)
        {
            return new OrganisationTerminationKboSummary
            {
                KboRegisteredOfficeLocation = CalculateRegisteredOfficeLocation(dateOfTermination, kboState.KboRegisteredOffice),
                KboFormalNameLabel = CalculateFormalNameLabel(dateOfTermination, kboState.KboFormalNameLabel),
                KboBankAccounts = CalculateKboBankAccounts(dateOfTermination, kboState.KboBankAccounts),
                KboLegalForm = CalculateLegalForm(dateOfTermination, kboState.KboLegalFormOrganisationClassification),
            };
        }

        public static OrganisationTerminationSummary CalculateTermination(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> classificationTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> formalFrameworkIdsToTerminateEndOfNextYear,
            OrganisationState state)
        {
            var endOfNextYear = new DateTime(dateOfTermination.Year + 1, 12, 31);

            if (state.Validity.Start.IsInFutureOf(dateOfTermination))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return new OrganisationTerminationSummary
            {
                OrganisationNewValidTo = state.Validity.End.IsInFutureOf(dateOfTermination) ? dateOfTermination : null,
                Contacts = Calculate(state.OrganisationContacts, dateOfTermination),
                BankAccounts = Calculate(state.OrganisationBankAccounts, dateOfTermination),
                Capacities = CalculateCapacities(dateOfTermination, capacityTypeIdsToTerminateEndOfNextYear, state.OrganisationCapacities, endOfNextYear),
                Functions = Calculate(state.OrganisationFunctionTypes, dateOfTermination),
                Locations = Calculate(state.OrganisationLocations, dateOfTermination),
                Buildings = Calculate(state.OrganisationBuildings, dateOfTermination),
                Parents = Calculate(state.OrganisationParents, dateOfTermination),
                Labels = Calculate(state.OrganisationLabels, dateOfTermination),
                Relations = Calculate(state.OrganisationRelations, dateOfTermination),
                OpeningHours = Calculate(state.OrganisationOpeningHours, dateOfTermination),
                Classifications = CalculateClassifications(dateOfTermination, state.OrganisationOrganisationClassifications, classificationTypeIdsToTerminateEndOfNextYear, endOfNextYear),
                FormalFrameworks = CalculateFormalFrameworks(dateOfTermination, state.OrganisationFormalFrameworks, endOfNextYear, formalFrameworkIdsToTerminateEndOfNextYear)
            };
        }

        private static Dictionary<Guid, DateTime> CalculateFormalFrameworks(DateTime dateOfTermination,
            IEnumerable<OrganisationFormalFramework> organisationFormalFrameworks, DateTime endOfNextYear,
            IEnumerable<Guid> formalFrameworkIdsToTerminateEndOfNextYear)
        {

            var formalFrameworksList = organisationFormalFrameworks.ToList();
            var organisationFormalFrameworksToTerminateEndOfNextYear =
                formalFrameworksList
                    .Where(formalFramework => formalFrameworkIdsToTerminateEndOfNextYear.Contains(formalFramework.FormalFrameworkId))
                    .ToDictionary(
                        formalFramework => formalFramework.OrganisationFormalFrameworkId,
                        _ => endOfNextYear);

            var organisationFormalFrameworksToTerminate =
                formalFrameworksList
                    .Where(formalFramework => formalFramework.Validity.End.IsInFutureOf(dateOfTermination))
                    .Where(formalFramework => !formalFrameworkIdsToTerminateEndOfNextYear.Contains(formalFramework.FormalFrameworkId))
                    .ToDictionary(
                        formalFramework => formalFramework.OrganisationFormalFrameworkId,
                        _ => dateOfTermination);

            return organisationFormalFrameworksToTerminate
                .Union(organisationFormalFrameworksToTerminateEndOfNextYear)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<Guid, DateTime> CalculateClassifications(DateTime dateOfTermination,
            IEnumerable<OrganisationOrganisationClassification> organisationClassifications,
            IEnumerable<Guid> classificationTypeIdsToTerminateEndOfNextYear,
            DateTime endOfNextYear)
        {
            var classificationsList = organisationClassifications.ToList();
            var organisationClassificationsToTerminateEndOfNextYear =
                classificationsList
                    .Where(classification => classificationTypeIdsToTerminateEndOfNextYear.Contains(classification.OrganisationClassificationTypeId))
                    .ToDictionary(
                        classification => classification.OrganisationOrganisationClassificationId,
                        _ => endOfNextYear);

            var organisationClassificationsToTerminate =
                classificationsList
                    .Where(classification => classification.Validity.End.IsInFutureOf(dateOfTermination))
                    .Where(classification => !classificationTypeIdsToTerminateEndOfNextYear.Contains(classification.OrganisationClassificationTypeId))
                    .ToDictionary(
                        classification => classification.OrganisationOrganisationClassificationId,
                        _ => dateOfTermination);

            return organisationClassificationsToTerminate
                .Union(organisationClassificationsToTerminateEndOfNextYear)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static Dictionary<Guid, DateTime> CalculateCapacities(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear, IEnumerable<OrganisationCapacity> organisationCapacities, DateTime endOfNextYear)
        {
            if (organisationCapacities.Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return organisationCapacities
                .Where(x => x.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    capacity => capacity.OrganisationCapacityId,
                    capacity => capacityTypeIdsToTerminateEndOfNextYear.Contains(capacity.CapacityId)
                        ? endOfNextYear
                        : dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateKboBankAccounts(DateTime dateOfTermination, IEnumerable<OrganisationBankAccount> kboBankAccounts)
        {
            return dateOfTermination != null
                ? kboBankAccounts.ToDictionary(
                    account => account.OrganisationBankAccountId,
                    _ => dateOfTermination)
                : new Dictionary<Guid, DateTime>();
        }

        private static KeyValuePair<Guid, DateTime>? CalculateFormalNameLabel(DateTime dateOfTermination, OrganisationLabel? kboFormalNameLabel)
        {
            return kboFormalNameLabel != null
                ? new KeyValuePair<Guid, DateTime>(kboFormalNameLabel.OrganisationLabelId,
                    dateOfTermination)
                : null;
        }

        private static KeyValuePair<Guid, DateTime>? CalculateRegisteredOfficeLocation(DateTime dateOfTermination, OrganisationLocation? kboRegisteredOffice)
        {
            return kboRegisteredOffice != null
                ? (KeyValuePair<Guid, DateTime>?) new KeyValuePair<Guid, DateTime>(
                    kboRegisteredOffice.OrganisationLocationId,
                    dateOfTermination)
                : null;
        }

        private static KeyValuePair<Guid, DateTime>? CalculateLegalForm(DateTime dateOfTermination, OrganisationOrganisationClassification? kboLegalForm)
        {
            return kboLegalForm != null
                ? (KeyValuePair<Guid, DateTime>?) new KeyValuePair<Guid, DateTime>(
                    kboLegalForm.OrganisationOrganisationClassificationId,
                    dateOfTermination)
                : null;
        }

        public static Dictionary<Guid, DateTime> Calculate(IEnumerable<IOrganisationField> fields, DateTime dateOfTermination)
        {
            if (fields.Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return fields
                .Where(x => x.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    x => x.Id,
                    _ => dateOfTermination);

        }
    }

    public interface IOrganisationField
    {
        Guid Id { get; }
        Period Validity { get; }
    }
}
