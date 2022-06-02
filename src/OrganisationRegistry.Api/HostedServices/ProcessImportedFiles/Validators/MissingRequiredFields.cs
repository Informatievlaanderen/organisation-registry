namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System.Collections.Generic;
using System.Linq;

public static class MissingRequiredFields
{
    public static ValidationIssue? Validate(int rowNumber, DeserializedRecord record)
        => ValidationIssuesFactory.Create(rowNumber, DetermineMissingRequiredFields(record).ToList(), FormatMessage);

    public static string FormatMessage(string missingRequiredFields)
        => $"Rij ontbreekt waarde voor volgende kolommen: {missingRequiredFields}.";

    private static IEnumerable<string> DetermineMissingRequiredFields(DeserializedRecord record)
    {
        if (!record.Reference.HasValue) yield return ColumnNames.Reference;
        if (!record.Parent.HasValue) yield return ColumnNames.Parent;
        if (!record.Name.HasValue) yield return ColumnNames.Name;
    }
}
