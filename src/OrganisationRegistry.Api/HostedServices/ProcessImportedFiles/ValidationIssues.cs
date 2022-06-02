namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using System.Collections.Immutable;

public class ValidationIssues
{
    private readonly ImmutableList<ValidationIssue> _items;

    public ValidationIssues()
        => _items = ImmutableList<ValidationIssue>.Empty;

    private ValidationIssues(ImmutableList<ValidationIssue> items)
        => _items = items;

    public ImmutableList<ValidationIssue> Items
        => _items.Sort((item1, item2) => item1.RowNumber - item2.RowNumber);

    public ValidationIssues Add(ValidationIssue? maybeIssue)
        => maybeIssue is { } issue ? new ValidationIssues(_items.Add(issue)) : this;

    public ValidationIssues AddRange(IEnumerable<ValidationIssue> issues)
        => new(_items.AddRange(issues));

    public ValidationIssues AddRange(ValidationIssues issues)
        => new(_items.AddRange(issues.Items));
}
