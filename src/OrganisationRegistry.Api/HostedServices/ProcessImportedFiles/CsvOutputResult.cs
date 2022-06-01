namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;

public class CsvOutputResult
{
    public IEnumerable<ValidationIssue> Issues { get; }
    public IEnumerable<OutputRecord> Records { get;  }

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

    public static CsvOutputResult WithIssues(IEnumerable<ValidationIssue> issues)
        => new(issues);
    public static CsvOutputResult WithRecords(IEnumerable<OutputRecord> records)
        => new(records);
}
