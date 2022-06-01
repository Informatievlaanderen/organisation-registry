namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using Validators;

public static class ImportFileParser
{
    public static IEnumerable<ParsedRecord> Parse(string importFileFileContent)
    {
        using var reader = new StringReader(importFileFileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        csv.Read();
        csv.ReadHeader();

        var csvHeaderRecord = GetHeaders(csv);
        var importedRecords = new List<ParsedRecord>();
        while (csv.Read())
        {
            importedRecords.Add(GetImportRecord(csv, csvHeaderRecord));
        }

        return importedRecords;
    }

    private static ParsedRecord GetImportRecord(IReaderRow csv, IReadOnlyDictionary<string, int> csvHeaderRecord)
    {
        if (InvalidColumnCount.Validate(csv) is { } invalidColumnCount)
            return new ParsedRecord(csv.Parser.Row, null, new[] { invalidColumnCount });

        var reference = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Reference);
        var name = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Name);
        var parent = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Parent);
        var article = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Article);
        var shortName = MaybeGetField(csv, csvHeaderRecord, ColumnNames.ShortName);
        var validity_start = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Validity_Start);
        var operationalValidity_Start = MaybeGetField(csv, csvHeaderRecord, ColumnNames.OperationalValidity_Start);

        return new ParsedRecord(
            csv.Parser.Row,
            new DeserializedRecord
            {
                Reference = reference,
                Name = name, Parent = parent,
                Article = article,
                ShortName = shortName,
                Validity_Start = validity_start,
                OperationalValidity_Start = operationalValidity_Start
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
