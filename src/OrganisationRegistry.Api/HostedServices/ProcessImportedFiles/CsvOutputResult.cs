namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;

public class CsvOutputResult
{
    private CsvOutputResult(IEnumerable<ValidationIssue> issues)
    {
        Issues = issues;
        Records = Array.Empty<OutputRecord>();
    }

    private CsvOutputResult(IEnumerable<OutputRecord> records)
    {
        Records = records;
        Issues = Array.Empty<ValidationIssue>();
    }

    public IEnumerable<ValidationIssue> Issues { get; }
    public IEnumerable<OutputRecord> Records { get; }

    public static CsvOutputResult WithIssues(ValidationIssues issues)
        => new(issues.Items);

    public static CsvOutputResult WithRecords(IEnumerable<OutputRecord> records)
        => new(records);
}
