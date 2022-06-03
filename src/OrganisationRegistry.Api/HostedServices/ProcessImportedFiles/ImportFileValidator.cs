namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Linq;

public static class ImportFileValidator
{
    public static (bool validationOk, string serializedOutput, CsvOutputResult? csvOutput) Validate(ImportCache importCache, DateOnly today, ParseResult parseResult)
    {
        var (parsedRecords, importFileContent) = parseResult;
        var validationIssues = FileValidator.Validate(importCache, today, parsedRecords);
        if (validationIssues.Items.Any())
        {
            var csvIssueOutput = CsvOutputResult.WithIssues(validationIssues);
            return (false, OutputSerializer.Serialize(csvIssueOutput), csvIssueOutput);
        }

        var csvRecordOutput = CsvOutputResult.WithRecords(parsedRecords.Select(r => OutputRecord.From(r.OutputRecord!)));
        return (true, importFileContent, csvRecordOutput);
    }
}
