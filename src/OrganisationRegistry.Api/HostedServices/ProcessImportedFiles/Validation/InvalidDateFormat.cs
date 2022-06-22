namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validation;

using System;
using System.Collections.Generic;
using System.Globalization;

public static class InvalidDateFormat
{
    public static IEnumerable<ValidationIssue> Validate(int rowNumber, params Field[] dateFieldsToValidate)
    {
        foreach (var field in dateFieldsToValidate)
        {
            if (ValidateField(rowNumber, field) is { } fieldIsInvalid)
                yield return fieldIsInvalid;
        }
    }

    private static ValidationIssue? ValidateField(int rowNumber, Field field)
    {
        if (!field.ShouldHaveValue)
            return null;
        if (!field.HasValue)
            return null;

        return DateTime.TryParseExact(field.Value, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out _)
            ? null
            : new ValidationIssue(rowNumber, FormatMessage(field.Value, field.ColumnName));
    }

    public static string FormatMessage(string? fieldValue, string columnName)
        => $"De waarde '{fieldValue}' is ongeldig voor kolom '{columnName}' (Vereist formaat: 'YYYY-MM-DD').";
}
