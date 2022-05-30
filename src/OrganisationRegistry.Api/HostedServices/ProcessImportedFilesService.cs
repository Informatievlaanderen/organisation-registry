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

    private static IEnumerable<ImportRecord> ParseImportFile(string importFileFileContent)
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
    public static IEnumerable<ImportRecord> Parse(string importFileFileContent)
    {
        using var reader = new StringReader(importFileFileContent);
        using var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture) { Delimiter = ";" });

        var importedRecords = new List<ImportRecord>();
        csv.Read();
        csv.ReadHeader();
        var csvHeaderRecord = csv.HeaderRecord
            .Select(columnName => columnName.Trim().ToLower())
            .Select((item, index) => (item, index))
            .ToDictionary(tuple => tuple.item, tuple => tuple.index);

        while (csv.Read())
        {
            var r = csv.Parser.Record;
            if (r.Length != csvHeaderRecord.Count)
            {
                importedRecords.Add(
                    new ImportRecord(ImmutableList.Create<string>("Rij heeft incorrect aantal kollomen.")));
                continue;
            }

            var reference = GetField(csv, csvHeaderRecord["reference"]);
            var name = GetField(csv, csvHeaderRecord["name"]);
            var parent = GetField(csv, csvHeaderRecord["parent"]);
            var record = new ImportRecord(reference.value, name.value, parent.value);
            importedRecords.Add(record);
        }

        return importedRecords;
    }

    private static (bool found, string value) GetField(CsvReader csv, int index)
    {
        // try
        // {
        //     return (true, csv.GetField(index).Trim());
        // }
        // catch (MissingFieldException)
        // {
        //     return (false, string.Empty);
        // }
        return (true, csv.GetField(index).Trim());
    }
}

public class ImportRecord
{
    public ImportRecord(string reference, string name, string? parent = null)
    {
        Reference = reference;
        Name = name;
        Parent = parent;
    }

    public ImportRecord(IImmutableList<string> errors) : this(string.Empty, string.Empty)
    {
        Errors = errors;
    }

    public string Reference { get; }
    public string? Parent { get; }
    public string Name { get; }
    public string? Validity_Start { get; init; }
    public string? ShortName { get; init; }
    public string? Article { get; init; }
    public string? OperationalValidity_Start { get; init; }
    public IImmutableList<string> Errors { get; init; } = new ImmutableArray<string>();
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
