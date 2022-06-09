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

        var importCache = ImportCache.Create(context, parsedRecords, dateTimeProvider.Today);

        var validationIssues = ImportFileValidator.Validate(
            importCache,
            DateOnly.FromDateTime(dateTimeProvider.Today),
            parsedRecords);

        return !validationIssues.Items.Any()
            ? ParseAndValidatorResult.ForRecords(ToOutputRecords(importCache, parsedRecords))
            : ParseAndValidatorResult.ForIssues(validationIssues);
    }

    private static List<OutputRecord> ToOutputRecords(ImportCache importCache, List<ParsedRecord> parsedRecords)
        => parsedRecords
            .Select((r, i) => ToOutputRecord(r, importCache, i))
            .ToList();

    private static OutputRecord ToOutputRecord(ParsedRecord record, ImportCache importCache, int index)
    {
        var parentOrganisation = importCache.GetOrganisationByOvoNumber(record.OutputRecord!.Parent.Value!)!;
        return OutputRecord.From(record.OutputRecord!, parentOrganisation.OrganisationId, index);
    }
}
