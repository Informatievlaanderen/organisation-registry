namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Linq;

public static class FileValidator
{
    public static ValidationIssues Validate(IReadOnlyList<ParsedRecord> parsedRecords)
        => new ValidationIssues()
            .AddRange(RecordValidator.Validate(parsedRecords))
            .AddRange(ValidationDuplicateReferences(parsedRecords));

    private static ValidationIssues ValidationDuplicateReferences(IEnumerable<ParsedRecord> parsedRecords)
    {
        var validRecords = parsedRecords
            .Where(x => !x.ValidationIssues.Any())
            .ToList();

        var allReferences = validRecords
            .Select(record => record.OutputRecord!.Reference.Value)
            .Where(reference => !string.IsNullOrWhiteSpace(reference));

        var groupedReferences = from reference in allReferences
            group reference by reference
            into g
            select new { Reference = g.Key, Count = g.Count() };

        var duplicateReferences = groupedReferences
            .Where(g => g.Count > 1)
            .Select(g => g.Reference)
            .ToList();

        var recordsWithDuplicateReferences = validRecords
            .Where(record => duplicateReferences.Contains(record.OutputRecord!.Reference.Value))
            .Select(record => record)
            .Select(record => new ValidationIssue(record.RowNumber, $"De waarde '{record.OutputRecord!.Reference}' komt meerdere keren voor in de kolom 'reference'."))
            .ToList();

        var result = new ValidationIssues();

        return result.AddRange(recordsWithDuplicateReferences);
    }
}
