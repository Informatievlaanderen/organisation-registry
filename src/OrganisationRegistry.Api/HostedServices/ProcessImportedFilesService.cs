namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CsvHelper;
using CsvHelper.Configuration;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SqlServer;
using SqlServer.Import.Organisations;
using MissingFieldException = CsvHelper.MissingFieldException;

public class ProcessImportedFilesService : BackgroundService
{
    private readonly IContextFactory _contextFactory;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ILogger<ProcessImportedFilesService> _logger;

    public ProcessImportedFilesService(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger<ProcessImportedFilesService> logger) : base(logger)
    {
        _contextFactory = contextFactory;
        _dateTimeProvider = dateTimeProvider;
        _logger = logger;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessNextFile(_contextFactory, _dateTimeProvider, _logger, cancellationToken);
            await Task.Delay(15000, cancellationToken); // todo: move to config
        }
    }

    private static async Task ProcessNextFile(
        IContextFactory contextFactory,
        IDateTimeProvider dateTimeProvider,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        // 0) context ophalen
        var context = contextFactory.Create();

        // 1) ophalen van oudste importfile in status 'Processing'
        var maybeImportFile = await context.ImportOrganisationsStatusList
            .Where(listItem => listItem.Status == ImportProcessStatus.Processing)
            .OrderBy(listItem => listItem.UploadedAt)
            .FirstOrDefaultAsync(cancellationToken);

        if (maybeImportFile is not { } importFile)
            return;

        var parsedFileContent = ImportFileParser.Parse(importFile.FileContent);
        await ValidateImportFile(importFile, logger, cancellationToken);
        // 2) verwerken (voorlopig random 1-60 sec delay, 1/10 kans op failure ?)
        await ProcessImportFile(dateTimeProvider, importFile, logger, cancellationToken);

        // 3) resultaat van verwerking terug saven naar db
        await context.SaveChangesAsync(cancellationToken);
    }

    private static IEnumerable<OutputRecord> ParseImportFile(string importFileFileContent)
    {
        throw new NotImplementedException();
    }

    private static async Task ValidateImportFile(
        ImportOrganisationsStatusListItem importFile,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        await UniqueReferenceValidator.Validate(importFile, logger, cancellationToken);
    }

    private static async Task ProcessImportFile(
        IDateTimeProvider dateTimeProvider,
        ImportOrganisationsStatusListItem importFile,
        ILogger logger,
        CancellationToken cancellationToken)
    {
        await DoDelay(cancellationToken);
        var success = GetSuccess();

        if (!success)
            logger.LogError("An error occured while processing the imported file {FileName}", importFile.FileName);

        importFile.Status = success ? ImportProcessStatus.Processed : ImportProcessStatus.Failed;
        importFile.LastProcessedAt = dateTimeProvider.UtcNow;
    }

    private static bool GetSuccess()
    {
        var rnd = new Random();

        var value = rnd.NextInt64(0, 10);

        return value != 5;
    }

    private static async Task DoDelay(CancellationToken cancellationToken)
    {
        var rnd = new Random();

        var delay = (int)rnd.NextInt64(1, 60);

        await Task.Delay(delay * 1000, cancellationToken);
    }
}

public static class ImportFileParser
{
    public static IEnumerable<OutputRecord> Parse(string importFileFileContent)
    {
        using var reader = new StringReader(importFileFileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        var importedRecords = new List<OutputRecord>();
        csv.Read();
        csv.ReadHeader();
        var csvHeaderRecord = GetHeaders(csv);

        while (csv.Read())
        {
            importedRecords.Add(GetRecord(csv, csvHeaderRecord));
        }

        return importedRecords;
    }

    private static Dictionary<string, int> GetHeaders(CsvReader csv)
    {
        return csv.HeaderRecord
            .Select(columnName => columnName.Trim().ToLower())
            .Select((item, index) => (item, index))
            .ToDictionary(tuple => tuple.item, tuple => tuple.index);
    }

    private static OutputRecord GetRecord(IReaderRow csv, IReadOnlyDictionary<string, int> csvHeaderRecord)
    {
        if (ValidateNumberOfFields(csv) is { } numberOfFieldsErrorMessage)
            return new OutputRecord(csv.Parser.RawRecord, ImmutableList.Create(numberOfFieldsErrorMessage));

        var reference = GetField(csv, csvHeaderRecord[ColumnNames.Reference]);
        var name = GetField(csv, csvHeaderRecord[ColumnNames.Name]);
        var parent = GetField(csv, csvHeaderRecord[ColumnNames.Parent]);

        if (ValidateRequiredFields(reference, name) is { } requiredFieldsErrorMessage)
            return new OutputRecord(csv.Parser.RawRecord, ImmutableList.Create(requiredFieldsErrorMessage));

        return new OutputRecord(reference, name, parent);
    }

    private static string? ValidateRequiredFields(string reference, string name)
    {
        var missingRequiredFields = new List<string>();

        if (string.IsNullOrEmpty(reference)) missingRequiredFields.Add(ColumnNames.Reference);
        if (string.IsNullOrEmpty(name)) missingRequiredFields.Add(ColumnNames.Name);

        return missingRequiredFields.Any()
            ? $"Rij ontbreekt waarde voor volgende kolommen: {string.Join(", ", missingRequiredFields.Select(f => $"'{f}'"))}."
            : null;
    }

    private static string? ValidateNumberOfFields(IReaderRow csv)
        => csv.Parser.Record.Length != csv.HeaderRecord.Length ? "Rij heeft incorrect aantal kolommen." : null;

    private static string GetField(IReaderRow csv, int index)
        => csv.GetField(index).Trim();
}

public static class ColumnNames
{
    public const string Reference = "reference";
    public const string Name = "name";
    public const string Parent = "parent";
    public const string Validity_Start = "validity_start";
    public const string ShortName = "shortname";
    public const string Article = "article";
    public const string OperationalValidity_Start = "operationalvalidity_start";

}
public class OutputRecord
{
    public OutputRecord(string reference, string name, string? parent = null)
    {
        Reference = reference;
        Name = name;
        Parent = parent;
    }

    public OutputRecord(string rawRecord, IImmutableList<string> errors) : this(string.Empty, string.Empty)
    {
        RawRecord = rawRecord;
        Errors = errors;
    }

    public string Reference { get; }
    public string? Parent { get; }
    public string Name { get; }
    public string? Validity_Start { get; init; }
    public string? ShortName { get; init; }
    public string? Article { get; init; }
    public string? OperationalValidity_Start { get; init; }
    public string? RawRecord { get; }
    public IImmutableList<string> Errors { get; init; } = ImmutableList<string>.Empty;
}

public static class UniqueReferenceValidator
{
    public static async Task Validate(
        ImportOrganisationsStatusListItem importFile,
        ILogger logger,
        CancellationToken cancellationToken)
    {
    }
}
