namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Commands;
using Organisation;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;
using OrganisationRegistry.SqlServer.Organisation;

public class ImportedFileProcessor : IImportedFileProcessor
{
    private readonly OrganisationRegistryContext _context;
    private readonly IDateTimeProvider _dateTimeProvider;
    private readonly ICommandSender _commandSender;

    public ImportedFileProcessor(
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider,
        ICommandSender commandSender)
    {
        _context = context;
        _dateTimeProvider = dateTimeProvider;
        _commandSender = commandSender;
    }

    public async Task<ProcessImportedFileResult> Process(ImportOrganisationsStatusListItem importFile, CancellationToken cancellationToken)
    {
        var parsedRecords = ImportFileParser.Parse(importFile);
        var validationResult = ImportFileValidator.Validate(_context, _dateTimeProvider, parsedRecords);

        var processResult = await Process(importFile, validationResult, cancellationToken);

        return new ProcessImportedFileResult(importFile, processResult, validationResult.ValidationOk);
    }

    private async Task<string> Process(ImportOrganisationsStatusListItem importFile, ValidationResult validationResult, CancellationToken cancellationToken)
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
            new CreateOrganisationsFromImport(importFile.Id, validationResult.OutputRecords),
            user);

        var organisationDetails =
            await RetrieveOrganisationDetails(_context, importFile.Id, cancellationToken);

        var result = ToCsvResult(organisationDetails, validationResult.OutputRecords);

        return await OutputSerializer.Serialize(importFile, result);
    }

    private static IEnumerable<CreateOrganisationsFromImportCommandItem> ToCsvResult(
        IEnumerable<OrganisationDetailItem> organisationDetails,
        IEnumerable<CreateOrganisationsFromImportCommandItem> parsedRecords)
        => JoinRecordsWithProjection(organisationDetails, parsedRecords).OrderBy(or => or.SortOrder)
            .ToList();

    private static IEnumerable<CreateOrganisationsFromImportCommandItem> JoinRecordsWithProjection(
        IEnumerable<OrganisationDetailItem> organisationDetails,
        IEnumerable<CreateOrganisationsFromImportCommandItem> parsedRecords)
        => from od in organisationDetails
            join outputRecord in parsedRecords on od.SourceOrganisationIdentifier equals outputRecord.Reference
            select outputRecord.WithOvoNumber(od.OvoNumber);

    /// <summary>
    /// Wait for the projection to be populated with the created organisations.
    /// but timeout if the organisationDetails are not populated within a reasonable amount of time
    /// (30 min. for now)
    /// </summary>
    /// <returns></returns>
    private static async Task<IEnumerable<OrganisationDetailItem>> RetrieveOrganisationDetails(
        OrganisationRegistryContext context,
        Guid importFileId,
        CancellationToken cancellationToken)
        => await context.OrganisationDetail
            .Where(od => od.SourceType == OrganisationSource.CsvImport && od.SourceId == importFileId)
            .ToListAsync(cancellationToken);
}
