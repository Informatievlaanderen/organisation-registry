namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;

public class ValidationResult<TImportCommandItem>
{
    public ValidationIssues ValidationIssues { get; }
    private readonly List<TImportCommandItem>? _outputRecords;

    public List<TImportCommandItem> OutputRecords
        => _outputRecords ?? throw new NullReferenceException();

    public bool ValidationOk
        => ValidationIssues.Items.IsEmpty;

    private ValidationResult(List<TImportCommandItem> outputRecords)
    {
        ValidationIssues = new ValidationIssues();
        _outputRecords = outputRecords;
    }

    private ValidationResult(ValidationIssues validationIssues)
    {
        ValidationIssues = validationIssues;
        _outputRecords = null;
    }

    public static ValidationResult<TImportCommandItem> ForRecords(List<TImportCommandItem> records)
        => new(records);

    public static ValidationResult<TImportCommandItem> ForIssues(ValidationIssues validationIssues)
        => new(validationIssues);
}
