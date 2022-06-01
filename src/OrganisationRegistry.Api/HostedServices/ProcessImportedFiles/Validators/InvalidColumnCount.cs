namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validators;

using CsvHelper;

public static class InvalidColumnCount
{
    public static ValidationIssue? Validate(IReaderRow row)
        => row.Parser.Record.Length != row.HeaderRecord.Length
            ? new ValidationIssue(row.Parser.Row, FormatMessage())
            : null;

    public static string FormatMessage()
        => "Rij heeft incorrect aantal kolommen.";
}
