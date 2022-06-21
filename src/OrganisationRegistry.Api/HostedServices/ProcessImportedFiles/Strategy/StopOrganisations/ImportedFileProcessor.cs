namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Organisation;
using Organisation.Import;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportedFileProcessor : ImportedFileProcessor<DeserializedRecord, StopOrganisationsFromImportCommandItem>
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

    protected override List<ParsedRecord<DeserializedRecord>> Parse(ImportOrganisationsStatusListItem importFile)
        => ImportFileParser.Parse(importFile);

    protected override ValidationResult<StopOrganisationsFromImportCommandItem> Validate(List<ParsedRecord<DeserializedRecord>> parsedRecords)
        => ImportFileValidator.Validate(_context, parsedRecords);

    protected override async Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult<StopOrganisationsFromImportCommandItem> validationResult, CancellationToken cancellationToken)
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
