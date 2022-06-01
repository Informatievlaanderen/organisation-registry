namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Globalization;
using System.IO;
using CsvHelper;
using CsvHelper.Configuration;

public static class OutputSerializer
{
    public const string NewLine = "\r\n";

    public static string Serialize(CsvOutputResult outputResult)
    {
        var stringWriter = new StringWriter();
        var csvWriter = new CsvWriter(
            stringWriter,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = NewLine });

        csvWriter.WriteHeader(new { lijnnummer = 0, fout = string.Empty }.GetType());
        csvWriter.NextRecord();
        foreach (var issue in outputResult.Issues)
        {
            csvWriter.WriteRecord(new { lijnnummer = issue.RowNumber, fout = issue.Error });
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        return stringWriter.ToString();
    }
}
