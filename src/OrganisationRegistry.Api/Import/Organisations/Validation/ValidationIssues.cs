namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Immutable;
using System.Linq;

public class ValidationIssues
{
    private readonly ImmutableList<ValidationIssue> _validationIssues = ImmutableList<ValidationIssue>.Empty;

    public ValidationIssues()
    { }

    private ValidationIssues(ImmutableList<ValidationIssue> validationIssues)
    {
        _validationIssues = validationIssues;
    }

    public ValidationIssues Add(ValidationIssue? maybeIssue)
        => maybeIssue is { } issue
            ? new ValidationIssues(_validationIssues.Add(issue))
            : this;

    public CsvValidationResult ToResult()
        => new(!_validationIssues.Any(), _validationIssues);
}
