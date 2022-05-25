namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public static class InvalidColumns
{
    public static ValidationIssue? Validate(IEnumerable<string> csvHeaderRecord, ImmutableList<string> validColumnNames)
        => ValidationIssuesFactory.Create(GetInvalidColumns(csvHeaderRecord, validColumnNames), FormatMessage);

    public static string FormatMessage(string invalidColumns)
        => $"Er werden ongeldige kolomnamen gevonden: {invalidColumns}";

    private static ImmutableList<string> GetInvalidColumns(IEnumerable<string> csvHeaderRecord, ImmutableList<string> validColumnNames)
        => csvHeaderRecord
            .Where(column => !validColumnNames.Any(validColumn => column == validColumn))
            .ToImmutableList();
}
