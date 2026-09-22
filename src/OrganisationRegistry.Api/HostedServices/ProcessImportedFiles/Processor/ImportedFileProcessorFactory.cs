namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Processor;

using System;
using Import.Organisations;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportedFileProcessorFactory
{
    private readonly OrganisationRegistryContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandSender _commandSender;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public ImportedFileProcessorFactory(
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider,
        ICommandSender commandSender,
        IOrganisationRegistryConfiguration configuration)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _commandSender = commandSender;
        _configuration = configuration;
    }

    public IImportedFileProcessor Create(string importFileType)
        => importFileType switch
        {
            ImportFileTypes.Create => new CreateOrganisations.ImportedFileProcessor(_context, _dateTimeProvider, _commandSender, _configuration),
            ImportFileTypes.Stop => new StopOrganisations.ImportedFileProcessor(_context, _commandSender, _configuration),
            _ => throw new ArgumentOutOfRangeException($"Unsupported import file type: {importFileType}"),
        };
}
