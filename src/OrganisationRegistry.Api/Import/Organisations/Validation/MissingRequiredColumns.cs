namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public static class MissingRequiredColumns
{
    public static ValidationIssue? Validate(IEnumerable<string> csvHeaderRecord, IEnumerable<string> requiredColumnNames)
        => ValidationIssuesFactory.Create(GetMissingColumns(csvHeaderRecord, requiredColumnNames), FormatMessage);

    public static string FormatMessage(string notfoundcolumns)
        => $"1 of meer verplichte kolommen werden niet gevonden: {notfoundcolumns}.";

    private static ImmutableList<string> GetMissingColumns(IEnumerable<string> csvHeaderRecord, IEnumerable<string> requiredColumnNames)
        => requiredColumnNames
            .Where(columnName => !csvHeaderRecord.Any(column => column == columnName))
            .ToImmutableList();
}
