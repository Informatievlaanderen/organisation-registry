namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Collections.Immutable;

public class ValidationIssues
{
    public ValidationIssues()
    {
        Items = ImmutableList<ValidationIssue>.Empty;
    }

    private ValidationIssues(ImmutableList<ValidationIssue> items)
    {
        Items = items;
    }

    public ImmutableList<ValidationIssue> Items { get; }

    public ValidationIssues Add(ValidationIssue? maybeIssue)
        => maybeIssue is { } issue ? new ValidationIssues(Items.Add(issue)) : this;

    public ValidationIssues AddRange(IEnumerable<ValidationIssue> issues)
        => new(Items.AddRange(issues));

    public ValidationIssues AddRange(ValidationIssues issues)
        => new(Items.AddRange(issues.Items));
}
