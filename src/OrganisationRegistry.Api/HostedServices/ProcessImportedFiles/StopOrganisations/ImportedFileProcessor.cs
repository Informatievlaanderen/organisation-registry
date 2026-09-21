namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.StopOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Organisation;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;
using Processor;
using Validation;

public class ImportedFileProcessor : ImportedFileProcessor<DeserializedRecord, TerminateOrganisationsFromImportCommandItem>
{
    private readonly OrganisationRegistryContext _context;
    private readonly ICommandSender _commandSender;
    private readonly IOrganisationRegistryConfiguration _configuration;

    public ImportedFileProcessor(
        OrganisationRegistryContext context,
        ICommandSender commandSender,
        IOrganisationRegistryConfiguration configuration)
    {
        _context = context;
        _commandSender = commandSender;
        _configuration = configuration;
    }

    protected override List<ParsedRecord<DeserializedRecord>> Parse(ImportOrganisationsStatusListItem importFile)
        => ImportFileParser.Parse(importFile);

    protected override ValidationResult<TerminateOrganisationsFromImportCommandItem> Validate(List<ParsedRecord<DeserializedRecord>> parsedRecords)
        => ImportFileValidator.Validate(_context, parsedRecords);

    protected override async Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult<TerminateOrganisationsFromImportCommandItem> validationResult, CancellationToken cancellationToken)
    {
        if (!validationResult.ValidationOk)
            return OutputSerializer.Serialize(validationResult.ValidationIssues);

        var roles = importFile.UserRoles.Split("|").Select(x => (Role)Enum.Parse(typeof(Role), x)).ToArray();
        var user = new User(
            importFile.UserFirstName,
            importFile.UserName,
            importFile.UserId,
            null,
            roles,
            new List<string>(),
            new List<Guid>(),
            new List<Guid>(),
            RolePermissionMap.For(roles, _configuration));

        await _commandSender.Send(
            new TerminateOrganisationsFromImport(importFile.Id, validationResult.CommandItems),
            user);

        return OutputSerializer.Serialize(importFile);
    }
}
