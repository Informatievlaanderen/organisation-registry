namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using System.Linq;
using OrganisationRegistry.Organisation.Import;
using OrganisationRegistry.SqlServer.Import.Organisations;
using OrganisationRegistry.SqlServer.Infrastructure;

public class ImportFileParserAndValidator : IImportFileParserAndValidator
{
    public ParseAndValidatorResult ParseAndValidate(
        ImportOrganisationsStatusListItem importFile,
        OrganisationRegistryContext context,
        IDateTimeProvider dateTimeProvider)
    {
        var parsedRecords = ImportFileParser.Parse(importFile);

        var importCache = ImportCache.Create(context, parsedRecords);

        var validationIssues = ImportFileValidator.Validate(
            importCache,
            DateOnly.FromDateTime(dateTimeProvider.Today),
            parsedRecords);

        return !validationIssues.Items.Any()
            ? ParseAndValidatorResult.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ParseAndValidatorResult.ForIssues(validationIssues);
    }

    private static List<OutputRecord> ToOutputRecords(ImportCache importCache, IEnumerable<ParsedRecord> parsedRecords)
        => parsedRecords
            .Select(r => ToOutputRecord(r, importCache))
            .ToList();

    private static OutputRecord ToOutputRecord(ParsedRecord record, ImportCache importCache)
        => OutputRecord.From(
            importCache.LabelTypes,
            record.OutputRecord!,
            GetOrganisationParentidentifier(importCache, record.OutputRecord!.Parent.Value!),
            record.RowNumber);

    private static OrganisationParentIdentifier GetOrganisationParentidentifier(
        ImportCache importCache,
        string parentIdentifierValue)
        => (OrganisationParentIdentifier?)importCache.GetOrganisationByOvoNumber(parentIdentifierValue)?.OrganisationId
           ?? (OrganisationParentIdentifier)parentIdentifierValue;
}
