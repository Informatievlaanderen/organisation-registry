namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validation;

using System;
using System.Collections.Generic;

public class ValidationResult<TImportCommandItem>
{
    public ValidationIssues ValidationIssues { get; }
    private readonly List<TImportCommandItem>? _commandItems;

    public List<TImportCommandItem> CommandItems
        => _commandItems ?? throw new NullReferenceException();

    public bool ValidationOk
        => ValidationIssues.Items.IsEmpty;

    private ValidationResult(List<TImportCommandItem> commandItems)
    {
        ValidationIssues = new ValidationIssues();
        _commandItems = commandItems;
    }

    private ValidationResult(ValidationIssues validationIssues)
    {
        ValidationIssues = validationIssues;
        _commandItems = null;
    }

    public static ValidationResult<TImportCommandItem> ForRecords(List<TImportCommandItem> records)
        => new(records);

    public static ValidationResult<TImportCommandItem> ForIssues(ValidationIssues validationIssues)
        => new(validationIssues);
}
