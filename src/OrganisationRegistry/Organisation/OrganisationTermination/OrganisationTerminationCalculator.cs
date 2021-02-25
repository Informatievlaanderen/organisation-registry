namespace OrganisationRegistry.Organisation.OrganisationTermination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using State;

    public static class OrganisationTerminationCalculator
    {
        public static OrganisationTerminationSummary GetFieldsToTerminate(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> classificationTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> formalFrameworkIdsToTerminateEndOfNextYear,
            OrganisationState state)
        {
            if (state.Validity.Start.IsInFutureOf(dateOfTermination))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return new OrganisationTerminationSummary
            {
                OrganisationNewValidTo = state.Validity.End.IsInFutureOf(dateOfTermination) ? dateOfTermination : null,
                Contacts = FieldsToTerminate(state.OrganisationContacts, dateOfTermination),
                BankAccounts = FieldsToTerminate(state.OrganisationBankAccounts, dateOfTermination),
                Functions = FieldsToTerminate(state.OrganisationFunctionTypes, dateOfTermination),
                Locations = FieldsToTerminate(state.OrganisationLocations, dateOfTermination),
                Buildings = FieldsToTerminate(state.OrganisationBuildings, dateOfTermination),
                Parents = FieldsToTerminate(state.OrganisationParents, dateOfTermination),
                Labels = FieldsToTerminate(state.OrganisationLabels, dateOfTermination),
                Relations = FieldsToTerminate(state.OrganisationRelations, dateOfTermination),
                OpeningHours = FieldsToTerminate(state.OrganisationOpeningHours, dateOfTermination),
                Capacities = FieldsToTerminateWithEndOfNextYear(capacityTypeIdsToTerminateEndOfNextYear,
                    state.OrganisationCapacities,
                    dateOfTermination,
                    field => field.CapacityId),
                Classifications = FieldsToTerminateWithEndOfNextYear(classificationTypeIdsToTerminateEndOfNextYear,
                    state.OrganisationOrganisationClassifications,
                    dateOfTermination,
                    field => field.OrganisationClassificationTypeId),
                FormalFrameworks = FieldsToTerminateWithEndOfNextYear(formalFrameworkIdsToTerminateEndOfNextYear,
                    state.OrganisationFormalFrameworks,
                    dateOfTermination,
                    field => field.FormalFrameworkId)
            };
        }

        public static OrganisationTerminationKboSummary GetKboFieldsToForceTerminate(DateTime dateOfTermination, KboState kboState)
        {
            return new OrganisationTerminationKboSummary
            {
                KboRegisteredOfficeLocation = KboFieldToTerminate(dateOfTermination, kboState.KboRegisteredOffice),
                KboFormalNameLabel = KboFieldToTerminate(dateOfTermination, kboState.KboFormalNameLabel),
                KboLegalForm = KboFieldToTerminate(dateOfTermination, kboState.KboLegalFormOrganisationClassification),
                KboBankAccounts = KboBankAccountsToTerminate(dateOfTermination, kboState.KboBankAccounts),
            };
        }

        private static Dictionary<Guid, DateTime> FieldsToTerminate(IEnumerable<IOrganisationField> fields, DateTime dateOfTermination)
        {
            if (fields.Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return fields
                .Where(x => x.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    x => x.Id,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> FieldsToTerminateWithEndOfNextYear<T>(
            IEnumerable<Guid> IdsToTerminateEndOfNextYear,
            IEnumerable<T> fields,
            DateTime dateOfTermination,
            Func<T, Guid> fieldToMatchWithIdToTerminateEndOfNextYear) where T : IOrganisationField
        {
            var endOfNextYear = new DateTime(dateOfTermination.Year + 1, 12, 31);

            var fieldsToTerminate = new List<T>();
            var fieldsToTerminateEndOfNextYear = new List<T>();

            foreach (var field in fields)
            {
                if (IdsToTerminateEndOfNextYear.Contains(fieldToMatchWithIdToTerminateEndOfNextYear(field)))
                {
                    fieldsToTerminateEndOfNextYear.Add(field);
                }
                else if (field.Validity.End.IsInFutureOf(dateOfTermination))
                {
                    fieldsToTerminate.Add(field);
                }
            }

            if (fieldsToTerminateEndOfNextYear
                .Any(x => x.Validity.Start.IsInFutureOf(endOfNextYear)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            if (fieldsToTerminate
                .Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return fieldsToTerminate
                .ToDictionary(
                    field => field.Id,
                    _ => dateOfTermination)
                .Union(fieldsToTerminateEndOfNextYear
                    .ToDictionary(
                        formalFramework => formalFramework.Id,
                        _ => endOfNextYear))
                .ToDictionary(x => x.Key, x => x.Value);
        }

        private static KeyValuePair<Guid, DateTime>? KboFieldToTerminate(DateTime dateOfTermination, IOrganisationField? kboField)
        {
            return kboField != null
                ? new KeyValuePair<Guid, DateTime>(
                    kboField.Id,
                    dateOfTermination)
                : null;
        }

        private static Dictionary<Guid, DateTime> KboBankAccountsToTerminate(DateTime dateOfTermination, IEnumerable<OrganisationBankAccount> kboBankAccounts)
        {
            return kboBankAccounts.ToDictionary(
                account => account.OrganisationBankAccountId,
                _ => dateOfTermination);
        }
    }
}
