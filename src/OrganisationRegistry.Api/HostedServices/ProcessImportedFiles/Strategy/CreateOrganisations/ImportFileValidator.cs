namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;
using Validators;

public class ImportFileValidator
{
    public static ValidationResult Validate(
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider,
        List<ParsedRecord> parsedRecords)
    {
        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = Validate(
            importCache,
            DateOnly.FromDateTime(dateTimeProvider.Today),
            parsedRecords);

        return !validationIssues.Items.Any()
            ? ValidationResult.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ValidationResult.ForIssues(validationIssues);
    }

    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IReadOnlyList<ParsedRecord> parsedRecords)
        => new ValidationIssues()
            .AddRange(RecordValidator.Validate(importCache, today, parsedRecords))
            .AddRange(HasDuplicateReferences.Validate(parsedRecords));

    private static List<CreateOrganisationsFromImportCommandItem> ToOutputRecords(ImportCache importCache, IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Select(r => ToOutputRecord(r, importCache))
            .ToList();

    private static CreateOrganisationsFromImportCommandItem ToOutputRecord(ParsedRecord record, ImportCache importCache)
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
