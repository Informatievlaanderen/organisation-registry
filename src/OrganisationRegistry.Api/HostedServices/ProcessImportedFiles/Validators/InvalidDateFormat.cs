namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using System;
using System.Collections.Generic;
using System.Globalization;

public static class InvalidDateFormat
{
    public static IEnumerable<ValidationIssue> Validate(int rowNumber, DeserializedRecord record)
    {
        if (ValidateField(rowNumber, record.Validity_Start) is { } validityStartIsInvalid)
            yield return validityStartIsInvalid;
        if (ValidateField(rowNumber, record.OperationalValidity_Start) is { } operationalValidityStartIsInvalid)
            yield return operationalValidityStartIsInvalid;
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
