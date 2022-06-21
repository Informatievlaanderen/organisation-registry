namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;
using Validators;

public class ImportFileValidator
{
    public static ValidationResult<CreateOrganisationsFromImportCommandItem> Validate(
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider,
        List<ParsedRecord<DeserializedRecord>> parsedRecords)
    {
        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = Validate(
            importCache,
            DateOnly.FromDateTime(dateTimeProvider.Today),
            parsedRecords);

        return !validationIssues.Items.Any()
            ? ValidationResult<CreateOrganisationsFromImportCommandItem>.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ValidationResult<CreateOrganisationsFromImportCommandItem>.ForIssues(validationIssues);
    }

    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IReadOnlyList<ParsedRecord<DeserializedRecord>> parsedRecords)
        => new ValidationIssues()
            .AddRange(ImportRecordValidator.Validate(importCache, today, parsedRecords))
            .AddRange(HasDuplicateReferences.Validate(parsedRecords));

    private static List<CreateOrganisationsFromImportCommandItem> ToOutputRecords(ImportCache importCache, IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
        => parsedRecords
            .Select(r => ToOutputRecord(r, importCache))
            .ToList();

    private static CreateOrganisationsFromImportCommandItem ToOutputRecord(ParsedRecord<DeserializedRecord> record, ImportCache importCache)
        => record.OutputRecord!.ToOutputRecord(
            importCache.LabelTypes,
            GetOrganisationParentidentifier(importCache, record.OutputRecord!.Parent.Value!),
            record.RowNumber);

    private static OrganisationParentIdentifier GetOrganisationParentidentifier(
        ImportCache importCache,
        string parentIdentifierValue)
        => (OrganisationParentIdentifier?)importCache.GetOrganisationByOvoNumber(parentIdentifierValue)?.OrganisationId
           ?? (OrganisationParentIdentifier)parentIdentifierValue;
}
