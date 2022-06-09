namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using OrganisationRegistry.Organisation.Import;

public class ParseAndValidatorResult
{
    public ValidationIssues ValidationIssues { get; }
    private readonly List<OutputRecord>? _outputRecords;

    public List<OutputRecord> OutputRecords
        => _outputRecords ?? throw new NullReferenceException();

    public bool ValidationOk
        => ValidationIssues.Items.IsEmpty;

    private ParseAndValidatorResult(List<OutputRecord> outputRecords)
    {
        ValidationIssues = new ValidationIssues();
        _outputRecords = outputRecords;
    }

    private ParseAndValidatorResult(ValidationIssues validationIssues)
    {
        ValidationIssues = validationIssues;
        _outputRecords = null;
    }

    public static ParseAndValidatorResult ForRecords(List<OutputRecord> records)
        => new(records);

    public static ParseAndValidatorResult ForIssues(ValidationIssues validationIssues)
        => new(validationIssues);
}
