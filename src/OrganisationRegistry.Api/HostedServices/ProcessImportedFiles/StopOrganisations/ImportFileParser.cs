namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using OrganisationRegistry.SqlServer.Import.Organisations;
using Validation;

public static class ImportFileParser
{
    public static List<ParsedRecord<DeserializedRecord>> Parse(ImportOrganisationsStatusListItem importFile)
        => ParseContent(importFile.FileContent).ToList();

    public static IEnumerable<ParsedRecord<DeserializedRecord>> ParseContent(string importFileFileContent)
    {
        using var reader = new StringReader(importFileFileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        csv.Read();
        csv.ReadHeader();

        var csvHeaderRecord = GetHeaders(csv);

        var importedRecords = new List<ParsedRecord<DeserializedRecord>>();
        while (csv.Read())
        {
            importedRecords.Add(GetImportRecord(csv, csvHeaderRecord));
        }

        return importedRecords;
    }

    private static ParsedRecord<DeserializedRecord> GetImportRecord(IReaderRow csv, IReadOnlyDictionary<string, int> csvHeaderRecord)
    {
        if (InvalidColumnCount.Validate(csv) is { } invalidColumnCount)
            return new ParsedRecord<DeserializedRecord>(csv.Parser.Row, OutputRecord: null, new[] { invalidColumnCount });

        var ovoNumber = MaybeGetField(csv, csvHeaderRecord, ColumnNames.OvoNumber);
        var name = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Name);
        var organisation_end = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Organisation_End);

        return new ParsedRecord<DeserializedRecord>(
            csv.Parser.Row,
            new DeserializedRecord
            {
                OvoNumber = ovoNumber,
                Name = name,
                Organisation_End = organisation_end,
            },
            new List<ValidationIssue>());
    }

    private static Field MaybeGetField(
        IReaderRow csv,
        IReadOnlyDictionary<string, int> csvHeaderRecord,
        string columnName)
        => csvHeaderRecord.ContainsKey(columnName)
            ? Field.FromValue(columnName, csv.GetField(csvHeaderRecord[columnName]).Trim())
            : Field.NoValue(columnName);

    private static Dictionary<string, int> GetHeaders(IReaderRow csv)
        => csv.HeaderRecord
            .Select(columnName => columnName.Trim().ToLower())
            .Select((item, index) => (item, index))
            .ToDictionary(tuple => tuple.item, tuple => tuple.index);
}
