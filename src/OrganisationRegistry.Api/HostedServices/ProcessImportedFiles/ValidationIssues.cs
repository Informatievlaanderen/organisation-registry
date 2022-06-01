namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Collections.Immutable;

public class ValidationIssues
{
    public ImmutableList<ValidationIssue> Items { get; }

    public ValidationIssues()
    {
        Items = ImmutableList<ValidationIssue>.Empty;
    }

    private ValidationIssues(ImmutableList<ValidationIssue> items)
    {
        Items = items;
    }

    public ValidationIssues Add(ValidationIssue? maybeIssue)
        => maybeIssue is { } issue ? new(Items.Add(issue)) : this;

    public ValidationIssues AddRange(IEnumerable<ValidationIssue> issue)
        => new(Items.AddRange(issue));
}
