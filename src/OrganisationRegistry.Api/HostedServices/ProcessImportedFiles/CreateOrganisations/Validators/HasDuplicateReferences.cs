namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.CreateOrganisations.Validators;

using System.Collections.Generic;
using System.Linq;
using Validation;

public static class HasDuplicateReferences
{
    public static ValidationIssues Validate(IEnumerable<ParsedRecord<DeserializedRecord>> parsedRecords)
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
            .Select(
                record => new ValidationIssue(record.RowNumber, FormatMessage(record.OutputRecord!.Reference.Value!)))
            .ToList();

        var result = new ValidationIssues();

        return result.AddRange(recordsWithDuplicateReferences);
    }

    public static string FormatMessage(string reference)
        => $"De waarde '{reference}' komt meerdere keren voor in de kolom 'reference'.";
}
