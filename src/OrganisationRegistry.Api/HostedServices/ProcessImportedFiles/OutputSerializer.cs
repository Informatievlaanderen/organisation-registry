namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Magda.Helpers;
using Organisation.Import;
using SqlServer.Import.Organisations;
using Validation;

public static class OutputSerializer
{
    public const string NewLine = "\r\n";

    public static string Serialize(ValidationIssues issues)
    {
        var stringWriter = new StringWriter();
        var csvWriter = new CsvWriter(
            stringWriter,
            new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";", NewLine = NewLine });

        csvWriter.WriteHeader(new { lijnnummer = 0, fout = string.Empty }.GetType());
        csvWriter.NextRecord();
        foreach (var issue in issues.Items)
        {
            csvWriter.WriteRecord(new { lijnnummer = issue.RowNumber, fout = issue.Error });
            csvWriter.NextRecord();
        }

        csvWriter.Flush();
        return stringWriter.ToString();
    }

    public static async Task<string> Serialize(
        ImportOrganisationsStatusListItem importFile,
        IEnumerable<CreateOrganisationsFromImportCommandItem> records)
    {
        var writer = new Utf8StringWriter();
        var reader = new StringReader(importFile.FileContent);

        var headerLine = await reader.ReadLineAsync();
        await writer.WriteLineAsync($"{headerLine};ovonumber");

        foreach (var item in records)
        {
            var lineInInputFile = await reader.ReadLineAsync();
            await writer.WriteLineAsync($"{lineInInputFile};{item.OvoNumber}");
        }

        return writer.ToString();
    }

    public static string Serialize(ImportOrganisationsStatusListItem importFile)
        => importFile.FileContent;
}
