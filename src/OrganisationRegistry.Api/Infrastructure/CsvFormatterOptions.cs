namespace OrganisationRegistry.Api.Infrastructure;

public class CsvFormatterOptions
{
    public bool UseSingleLineHeaderInCsv { get; set; } = true;

    public string CsvDelimiter { get; set; } = ";";
}