namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using CsvHelper;
using CsvHelper.Configuration;
using OrganisationRegistry.SqlServer.Import.Organisations;
using ProcessImportedFiles.Validators;

public static class ImportFileParser
{
    public static List<ParsedRecord> Parse(ImportOrganisationsStatusListItem importFile)
        => ParseContent(importFile.FileContent).ToList();

    public static IEnumerable<ParsedRecord> ParseContent(string importFileFileContent)
    {
        using var reader = new StringReader(importFileFileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        csv.Read();
        csv.ReadHeader();

        var csvHeaderRecord = GetHeaders(csv);

        var labelColumns = csvHeaderRecord
            .Where(item => item.Key.Trim().StartsWith("label#", StringComparison.InvariantCultureIgnoreCase))
            .ToDictionary(item => item.Key, item => item.Value);

        var importedRecords = new List<ParsedRecord>();
        while (csv.Read())
        {
            importedRecords.Add(GetImportRecord(csv, csvHeaderRecord, labelColumns));
        }

        return importedRecords;
    }

    private static ParsedRecord GetImportRecord(IReaderRow csv, IReadOnlyDictionary<string, int> csvHeaderRecord, Dictionary<string, int> labelColumns)
    {
        if (InvalidColumnCount.Validate(csv) is { } invalidColumnCount)
            return new ParsedRecord(csv.Parser.Row, OutputRecord: null, new[] { invalidColumnCount });

        var reference = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Reference);
        var name = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Name);
        var parent = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Parent);
        var article = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Article);
        var shortName = MaybeGetField(csv, csvHeaderRecord, ColumnNames.ShortName);
        var validity_start = MaybeGetField(csv, csvHeaderRecord, ColumnNames.Validity_Start);
        var operationalValidity_start = MaybeGetField(csv, csvHeaderRecord, ColumnNames.OperationalValidity_Start);

        var labels = GetLabelFields(csv, labelColumns);

        return new ParsedRecord(
            csv.Parser.Row,
            new DeserializedRecord
            {
                Reference = reference,
                Name = name, Parent = parent,
                Article = article,
                ShortName = shortName,
                Validity_Start = validity_start,
                OperationalValidity_Start = operationalValidity_start,
                Labels = labels,
            },
            new List<ValidationIssue>());
    }

    private static ImmutableList<Field> GetLabelFields(IReaderRow csv, IReadOnlyDictionary<string, int> labelColumns)
        => labelColumns
            .Select(column => MaybeGetField(csv, labelColumns, column.Key))
            .Where(field => field.HasValue)
            .ToImmutableList();

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
