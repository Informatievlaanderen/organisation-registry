namespace OrganisationRegistry.Api.Import.Organisations.Validation;

public static class InvalidFilename
{
    public static ValidationIssue? Validate(string filename)
        => ValidationIssuesFactory.Create(CheckFilename(filename), FormatMessage);

    public static string FormatMessage(string filename)
        => $"bestand met naam '{filename}' is geen geldig CSV bestand.";

    private static string? CheckFilename(string filename)
        => !filename.ToLower().EndsWith(".csv")
            ? filename
            : null;
}
