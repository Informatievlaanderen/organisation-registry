namespace OrganisationRegistry.Organisation.OrganisationTermination
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Exceptions;
    using State;

    public static class OrganisationTerminationCalculator
    {
        public static OrganisationTerminationSummary GetFieldsToTerminate(DateTime dateOfTermination,
            IEnumerable<Guid> capacityTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> classificationTypeIdsToTerminateEndOfNextYear,
            IEnumerable<Guid> formalFrameworkIdsToTerminateEndOfNextYear,
            Guid vlimpersKeyTypeId,
            OrganisationState state)
        {
            if (state.Validity.Start.IsInFutureOf(dateOfTermination))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            var newValidTo = state.Validity.End.IsInFutureOf(dateOfTermination) ? dateOfTermination : (DateTime?)null;
            return new OrganisationTerminationSummary(
                newValidTo,
                FieldsToTerminate(state.OrganisationContacts, dateOfTermination),
                FieldsToTerminate(state.OrganisationBankAccounts, dateOfTermination),
                FieldsToTerminate(state.OrganisationFunctionTypes, dateOfTermination),
                FieldsToTerminate(state.OrganisationLocations, dateOfTermination),
                FieldsToTerminateWithEndOfNextYear(
                    capacityTypeIdsToTerminateEndOfNextYear,
                    state.OrganisationCapacities,
                    dateOfTermination,
                    field => field.CapacityId),
                FieldsToTerminate(state.OrganisationBuildings, dateOfTermination),
                FieldsToTerminate(state.OrganisationLabels, dateOfTermination),
                FieldsToTerminate(state.OrganisationRelations, dateOfTermination),
                FieldsToTerminate(state.OrganisationOpeningHours, dateOfTermination),
                FieldsToTerminateWithEndOfNextYear(
                    classificationTypeIdsToTerminateEndOfNextYear,
                    state.OrganisationOrganisationClassifications,
                    dateOfTermination,
                    field => field.OrganisationClassificationTypeId),
                FieldsToTerminateWithEndOfNextYear(
                    formalFrameworkIdsToTerminateEndOfNextYear,
                    state.OrganisationFormalFrameworks,
                    dateOfTermination,
                    field => field.FormalFrameworkId),
                FieldsToTerminate(state.OrganisationRegulations, dateOfTermination),
                KeyFieldsToTerminate(state.OrganisationKeys, dateOfTermination, vlimpersKeyTypeId)
            );
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

        private static Dictionary<Guid, DateTime> FieldsToTerminate(IReadOnlyList<IOrganisationField> fields, DateTime dateOfTermination)
        {
            if (fields.Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination)))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return fields
                .Where(x => x.Validity.End.IsInFutureOf(dateOfTermination))
                .ToDictionary(
                    x => x.Id,
                    _ => dateOfTermination);
        }

        private static Dictionary<Guid, DateTime> KeyFieldsToTerminate(IReadOnlyList<OrganisationKey> fields, DateTime dateOfTermination, Guid keyTypeId)
        {
            if (fields.Any(x => x.Validity.Start.IsInFutureOf(dateOfTermination) && x.KeyTypeId == keyTypeId))
                throw new OrganisationCannotBeTerminatedWithFieldsInTheFuture();

            return fields
                .Where(x => x.Validity.End.IsInFutureOf(dateOfTermination)  && x.KeyTypeId == keyTypeId)
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
