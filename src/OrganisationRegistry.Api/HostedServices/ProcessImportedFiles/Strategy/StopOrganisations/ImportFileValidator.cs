namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System.Collections.Generic;
using System.Linq;
using Organisation.Import;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportFileValidator
{
    public static ValidationResult Validate(
        OrganisationRegistryContext context,
        List<ParsedRecord> parsedRecords)
    {
        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = RecordValidator.Validate(importCache, parsedRecords);

        return !validationIssues.Items.Any()
            ? ValidationResult.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ValidationResult.ForIssues(validationIssues);
    }

    private static List<StopOrganisationsFromImportCommandItem> ToOutputRecords(ImportCache importCache, IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Select(r => ToOutputRecord(r, importCache))
            .ToList();

    private static StopOrganisationsFromImportCommandItem ToOutputRecord(ParsedRecord record, ImportCache importCache)
        => record.OutputRecord!.ToOutputRecord(
            importCache.GetOrganisationByOvoNumber(
                record.OutputRecord!.OvoNumber.Value!
            )!.Id
        );
}
