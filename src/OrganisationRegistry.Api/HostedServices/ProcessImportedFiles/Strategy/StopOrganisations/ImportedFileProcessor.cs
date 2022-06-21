namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Organisation;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportedFileProcessor : IImportedFileProcessor
{
    private readonly OrganisationRegistryContext _context;
    private readonly ICommandSender _commandSender;

    public ImportedFileProcessor(
        OrganisationRegistryContext context,
        ICommandSender commandSender)
    {
        _context = context;
        _commandSender = commandSender;
    }

    public async Task<ProcessImportedFileResult> Process(ImportOrganisationsStatusListItem importFile, CancellationToken cancellationToken)
    {
        var parsedRecords = ImportFileParser.Parse(importFile);
        var validationResult = ImportFileValidator.Validate(_context, parsedRecords);

        var processResult = await Process(importFile, validationResult);

        return new ProcessImportedFileResult(importFile, processResult, validationResult.ValidationOk);
    }

    private async Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult validationResult)
    {
        if (!validationResult.ValidationOk)
            return OutputSerializer.Serialize(validationResult.ValidationIssues);

        var user = new User(
            importFile.UserFirstName,
            importFile.UserName,
            importFile.UserId,
            null,
            new[] { Role.AlgemeenBeheerder },
            new List<string>());

        await _commandSender.Send(
            new TerminateOrganisationsFromImport(importFile.Id, validationResult.OutputRecords),
            user);

        return OutputSerializer.Serialize(importFile);
    }
}
