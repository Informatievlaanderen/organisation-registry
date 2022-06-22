namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations;

using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;
using Validation;

public class ImportFileValidator
{
    public static ValidationResult<TerminateOrganisationsFromImportCommandItem> Validate(
        OrganisationRegistryContext context,
        List<ParsedRecord<DeserializedRecord>> parsedRecords)
    {
        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = ImportRecordValidator.Validate(importCache, parsedRecords);

        return !validationIssues.Items.Any()
            ? ValidationResult<TerminateOrganisationsFromImportCommandItem>.ForRecords(ToCommandItems(importCache, parsedRecords))
            : ValidationResult<TerminateOrganisationsFromImportCommandItem>.ForIssues(validationIssues);
    }

    private static List<TerminateOrganisationsFromImportCommandItem> ToCommandItems(ImportCache importCache, IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
        => parsedRecords
            .Select(r => ToCommandItem(r, importCache))
            .ToList();

    private static TerminateOrganisationsFromImportCommandItem ToCommandItem(ParsedRecord<DeserializedRecord> record, ImportCache importCache)
        => record.DeserializedRecord!.ToCommandItem(
            importCache.GetOrganisationByOvoNumber(
                record.DeserializedRecord!.OvoNumber.Value!
            )!.Id
        );
}
