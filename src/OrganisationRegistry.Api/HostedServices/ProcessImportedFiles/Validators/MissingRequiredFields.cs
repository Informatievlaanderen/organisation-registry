namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System.Collections.Generic;
using System.Linq;

public static class MissingRequiredFields
{
    public static ValidationIssue? Validate(int rowNumber, params Field[] requiredFields)
        => ValidationIssuesFactory.Create(rowNumber, DetermineMissingRequiredFields(requiredFields).ToList(), FormatMessage);

    public static string FormatMessage(string missingRequiredFields)
        => $"Rij ontbreekt waarde voor volgende kolommen: {missingRequiredFields}.";

    private static IEnumerable<string> DetermineMissingRequiredFields(Field[] requiredFields)
        => requiredFields
            .Where(requiredField => !requiredField.HasValue)
            .Select(requiredField => requiredField.ColumnName);
}
