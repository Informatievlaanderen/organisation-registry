namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy;

using System;
using Import.Organisations;
using OrganisationRegistry.Infrastructure.Commands;
using SqlServer.Infrastructure;

public class ImportedFileProcessorFactory
{
    private readonly OrganisationRegistryContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandSender _commandSender;

    public ImportedFileProcessorFactory(
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider,
        ICommandSender commandSender)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _commandSender = commandSender;
    }

    public IImportedFileProcessor Create(string importFileType)
        => importFileType switch
        {
            ImportFileTypes.Create => new CreateOrganisations.ImportedFileProcessor(_context, _dateTimeProvider, _commandSender),
            ImportFileTypes.Stop => new StopOrganisations.ImportedFileProcessor(_context, _commandSender),
            _ => throw new ArgumentOutOfRangeException($"Unsupported import file type: {importFileType}"),
        };
}
