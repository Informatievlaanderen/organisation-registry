namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Organisation;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;
using Processor;
using Validation;

public class ImportedFileProcessor : ImportedFileProcessor<DeserializedRecord, TerminateOrganisationsFromImportCommandItem>
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

    protected override ValidationResult<TerminateOrganisationsFromImportCommandItem> Validate(List<ParsedRecord<DeserializedRecord>> parsedRecords)
        => ImportFileValidator.Validate(_context, parsedRecords);

    protected override async Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult<TerminateOrganisationsFromImportCommandItem> validationResult, CancellationToken cancellationToken)
    {
        if (!validationResult.ValidationOk)
            return OutputSerializer.Serialize(validationResult.ValidationIssues);

        var user = new User(
            importFile.UserFirstName,
            importFile.UserName,
            importFile.UserId,
            null,
            importFile.UserRoles.Split("|").Select(x => (Role)Enum.Parse(typeof(Role), x)).ToArray(),
            new List<string>());

        await _commandSender.Send(
            new TerminateOrganisationsFromImport(importFile.Id, validationResult.CommandItems),
            user);

        return OutputSerializer.Serialize(importFile);
    }
}
