namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public static class DuplicateColumns
{
    public static IEnumerable<ValidationIssue> Validate(IEnumerable<string> csvHeaderRecord)
        => ValidationIssuesFactory.Create(GetDuplicateColumns(csvHeaderRecord), FormatMessage);

    public static string FormatMessage(string duplicatecolumns)
        => $"Er werden dubbele kolomnamen gevonden: {duplicatecolumns}.";

    private static ImmutableList<string> GetDuplicateColumns(IEnumerable<string> csvHeaderRecord)
        => csvHeaderRecord
            .GroupBy(x => x)
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToImmutableList();
}
