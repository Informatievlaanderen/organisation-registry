namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.StopOrganisations;

using System;
using System.Collections.Generic;
using Organisation.Import;

public class ValidationResult
{
    public ValidationIssues ValidationIssues { get; }
    private readonly List<StopOrganisationsFromImportCommandItem>? _outputRecords;

    public List<StopOrganisationsFromImportCommandItem> OutputRecords
        => _outputRecords ?? throw new NullReferenceException();

    public bool ValidationOk
        => ValidationIssues.Items.IsEmpty;

    private ValidationResult(List<StopOrganisationsFromImportCommandItem> outputRecords)
    {
        ValidationIssues = new ValidationIssues();
        _outputRecords = outputRecords;
    }

    private ValidationResult(ValidationIssues validationIssues)
    {
        ValidationIssues = validationIssues;
        _outputRecords = null;
    }

    public static ValidationResult ForRecords(List<StopOrganisationsFromImportCommandItem> records)
        => new(records);

    public static ValidationResult ForIssues(ValidationIssues validationIssues)
        => new(validationIssues);
}
