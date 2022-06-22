namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;
using Validation;
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
            ? ValidationResult<CreateOrganisationsFromImportCommandItem>.ForRecords(ToCommandItems(importCache, parsedRecords))
            : ValidationResult<CreateOrganisationsFromImportCommandItem>.ForIssues(validationIssues);
    }

    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IReadOnlyList<ParsedRecord<DeserializedRecord>> parsedRecords)
        => new ValidationIssues()
            .AddRange(ImportRecordValidator.Validate(importCache, today, parsedRecords))
            .AddRange(HasDuplicateReferences.Validate(parsedRecords));

    private static List<CreateOrganisationsFromImportCommandItem> ToCommandItems(ImportCache importCache, IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
        => parsedRecords
            .Select(r => ToCommandItem(r, importCache))
            .ToList();

    private static CreateOrganisationsFromImportCommandItem ToCommandItem(ParsedRecord<DeserializedRecord> record, ImportCache importCache)
        => record.OutputRecord!.ToCommandItem(
            importCache.LabelTypes,
            GetOrganisationParentidentifier(importCache, record.OutputRecord!.Parent.Value!),
            record.RowNumber);

    private static OrganisationParentIdentifier GetOrganisationParentidentifier(
        ImportCache importCache,
        string parentIdentifierValue)
        => (OrganisationParentIdentifier?)importCache.GetOrganisationByOvoNumber(parentIdentifierValue)?.OrganisationId
           ?? (OrganisationParentIdentifier)parentIdentifierValue;
}
