namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System;
using System.Collections.Generic;
using OrganisationRegistry.Organisation.Import;

public class ValidationResult
{
    public ValidationIssues ValidationIssues { get; }
    private readonly List<CreateOrganisationsFromImportCommandItem>? _outputRecords;

    public List<CreateOrganisationsFromImportCommandItem> OutputRecords
        => _outputRecords ?? throw new NullReferenceException();

    public bool ValidationOk
        => ValidationIssues.Items.IsEmpty;

    private ValidationResult(List<CreateOrganisationsFromImportCommandItem> outputRecords)
    {
        ValidationIssues = new ValidationIssues();
        _outputRecords = outputRecords;
    }

    private ValidationResult(ValidationIssues validationIssues)
    {
        ValidationIssues = validationIssues;
        _outputRecords = null;
    }

    public static ValidationResult ForRecords(List<CreateOrganisationsFromImportCommandItem> records)
        => new(records);

    public static ValidationResult ForIssues(ValidationIssues validationIssues)
        => new(validationIssues);
}
