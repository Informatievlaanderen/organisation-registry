namespace OrganisationRegistry.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Linq;

    public struct OrganisationTermination
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
        public Dictionary<Guid, DateTime> KboBankAccounts { get; set; }
        public KeyValuePair<Guid, DateTime>? KboRegisteredOfficeLocation { get; set; }
        public KeyValuePair<Guid, DateTime>? KboFormalNameLabel { get; set; }
        public KeyValuePair<Guid, DateTime>? KboLegalForm { get; set; }

        public static OrganisationTermination Calculate(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> classificationTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> formalFrameworkIdsToTerminateEndOfNextYear,
            KboState kboState,
            OrganisationState state)
        {
            var endOfNextYear = new DateTime(dateOfTermination.Year + 1, 12, 31);

            if (state.Validity.Start.IsInFutureOf(dateOfTermination))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return new OrganisationTermination
            {
                OrganisationNewValidTo = state.Validity.End.IsInFutureOf(dateOfTermination) ? dateOfTermination : null,
                Contacts = CalculateContacts(dateOfTermination, state.OrganisationContacts),
                BankAccounts = CalculateBankAccounts(dateOfTermination, state.OrganisationBankAccounts),
                KboBankAccounts = CalculateKboBankAccounts(kboState.TerminationInKbo, kboState.KboBankAccounts),
                Capacities = CalculateCapacities(dateOfTermination, capacityTypeIdsToTerminateEndOfNextYear, state.OrganisationCapacities, endOfNextYear),
                Functions = CalculateFunctions(dateOfTermination, state.OrganisationFunctionTypes),
                Locations = CalculateLocations(dateOfTermination, state.OrganisationLocations),
                KboRegisteredOfficeLocation = CalculateRegisteredOfficeLocation(kboState.TerminationInKbo, kboState.KboRegisteredOffice),
                Buildings = CalculateBuildings(dateOfTermination, state.OrganisationBuildings),
                Parents = CalculateParents(dateOfTermination, state.OrganisationParents),
                Labels = CalculateLabels(dateOfTermination, state.OrganisationLabels),
                KboFormalNameLabel = CalculateFormalNameLabel(kboState.TerminationInKbo, kboState.KboFormalNameLabel),
                Relations = CalculateRelations(dateOfTermination, state.OrganisationRelations),
                OpeningHours = CalculateOpeningHours(dateOfTermination, state.OrganisationOpeningHours),
                Classifications = CalculateClassifications(dateOfTermination, state.OrganisationOrganisationClassifications, classificationTypeIdsToTerminateEndOfNextYear, endOfNextYear),
                KboLegalForm = CalculateLegalForm(kboState.TerminationInKbo, kboState.KboLegalFormOrganisationClassification),
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

        private static Dictionary<Guid, DateTime> CalculateOpeningHours(DateTime dateOfTermination, IEnumerable<OrganisationOpeningHour> organisationOpeningHours)
        {
            return organisationOpeningHours
                .Where(openingHours => openingHours.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    openingHours => openingHours.OrganisationOpeningHourId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateRelations(DateTime dateOfTermination, IEnumerable<OrganisationRelation> organisationRelations)
        {
            return organisationRelations
                .Where(relation => relation.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    relation => relation.OrganisationRelationId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateLabels(DateTime dateOfTermination, IEnumerable<OrganisationLabel> organisationLabels)
        {
            var labels = organisationLabels
                .Where(label => label.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    label => label.OrganisationLabelId,
                    _ => dateOfTermination);

            return labels;
        }

        private static Dictionary<Guid, DateTime> CalculateParents(DateTime dateOfTermination, IEnumerable<OrganisationParent> organisationParents)
        {
            return organisationParents
                .Where(parent => parent.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    parent => parent.OrganisationOrganisationParentId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateBuildings(DateTime dateOfTermination, IEnumerable<OrganisationBuilding> organisationBuildings)
        {
            return organisationBuildings
                .Where(building => building.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    building => building.OrganisationBuildingId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateCapacities(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear, IEnumerable<OrganisationCapacity> organisationCapacities, DateTime endOfNextYear)
        {
            return organisationCapacities
                .ToDictionary(
                    capacity => capacity.OrganisationCapacityId,
                    capacity => capacityTypeIdsToTerminateEndOfNextYear.Contains(capacity.CapacityId)
                        ? endOfNextYear
                        : dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateLocations(DateTime dateOfTermination, OrganisationLocations organisationLocations)
        {
            return organisationLocations
                .Where(location => location.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    account => account.OrganisationLocationId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateContacts(DateTime dateOfTermination, IEnumerable<OrganisationContact> organisationContacts)
        {
            return organisationContacts
                .Where(contact => contact.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    contact => contact.OrganisationContactId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateFunctions(DateTime dateOfTermination, IEnumerable<OrganisationFunction> organisationFunctionTypes)
        {
            return organisationFunctionTypes
                .Where(function => function.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    function => function.OrganisationFunctionId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateBankAccounts(DateTime dateOfTermination, IEnumerable<OrganisationBankAccount> organisationBankAccounts)
        {
            return organisationBankAccounts
                .Where(account => account.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(account => account.OrganisationBankAccountId,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> CalculateKboBankAccounts(KboTermination? terminationInKbo, IEnumerable<OrganisationBankAccount> kboBankAccounts)
        {
            return terminationInKbo != null
                ? kboBankAccounts.ToDictionary(
                    account => account.OrganisationBankAccountId,
                    _ => terminationInKbo.Value.Date)
                : new Dictionary<Guid, DateTime>();
        }

        private static KeyValuePair<Guid, DateTime>? CalculateFormalNameLabel(KboTermination? terminationInKbo, OrganisationLabel? kboFormalNameLabel)
        {
            return terminationInKbo != null && kboFormalNameLabel != null
                ? new KeyValuePair<Guid, DateTime>(kboFormalNameLabel.OrganisationLabelId,
                    terminationInKbo.Value.Date)
                : null;
        }

        private static KeyValuePair<Guid, DateTime>? CalculateRegisteredOfficeLocation(KboTermination? terminationInKbo, OrganisationLocation? kboRegisteredOffice)
        {
            return terminationInKbo != null && kboRegisteredOffice != null
                ? (KeyValuePair<Guid, DateTime>?) new KeyValuePair<Guid, DateTime>(
                    kboRegisteredOffice.OrganisationLocationId,
                    terminationInKbo.Value.Date)
                : null;
        }

        private static KeyValuePair<Guid, DateTime>? CalculateLegalForm(KboTermination? terminationInKbo, OrganisationOrganisationClassification? kboLegalForm)
        {
            return terminationInKbo != null && kboLegalForm != null
                ? (KeyValuePair<Guid, DateTime>?) new KeyValuePair<Guid, DateTime>(
                    kboLegalForm.OrganisationOrganisationClassificationId,
                    terminationInKbo.Value.Date)
                : null;
        }
    }
}
