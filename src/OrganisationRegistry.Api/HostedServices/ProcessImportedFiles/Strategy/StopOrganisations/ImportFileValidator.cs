namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System.Collections.Generic;
using System.Linq;
using Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportFileValidator
{
    public static ValidationResult<StopOrganisationsFromImportCommandItem> Validate(
        OrganisationRegistryContext context,
        List<ParsedRecord<DeserializedRecord>> parsedRecords)
    {
        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = ImportRecordValidator.Validate(importCache, parsedRecords);

        return !validationIssues.Items.Any()
            ? ValidationResult<StopOrganisationsFromImportCommandItem>.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ValidationResult<StopOrganisationsFromImportCommandItem>.ForIssues(validationIssues);
    }

    private static List<StopOrganisationsFromImportCommandItem> ToOutputRecords(ImportCache importCache, IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
        => parsedRecords
            .Select(r => ToOutputRecord(r, importCache))
            .ToList();

    private static StopOrganisationsFromImportCommandItem ToOutputRecord(ParsedRecord<DeserializedRecord> record, ImportCache importCache)
        => record.OutputRecord!.ToOutputRecord(
            importCache.GetOrganisationByOvoNumber(
                record.OutputRecord!.OvoNumber.Value!
            )!.Id
        );
}
