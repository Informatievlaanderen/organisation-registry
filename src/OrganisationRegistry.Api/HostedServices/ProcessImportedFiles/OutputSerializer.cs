namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Magda.Helpers;
using Organisation.Import;
using SqlServer.Import.Organisations;
using Validation;

public static class OutputSerializer
{
    public static string Serialize(ValidationIssues issues)
        => OrganisatieRegisterCsvWriter.WriteCsv(
            new { lijnnummer = 0, fout = string.Empty }.GetType(),
            issue => new { lijnnummer = issue.RowNumber, fout = issue.Error },
            issues.Items);

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
