namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System;
using System.Collections.Immutable;
using System.Linq;

public static class ValidationIssuesFactory
{
    public static ValidationIssue? Create(ImmutableList<string> issues, Func<string, string> formatValidationMessage)
        => issues.Any()
            ? new ValidationIssue(formatValidationMessage(string.Join(", ", issues)))
            : null;

    public static ValidationIssue? Create(string? maybeIssue, Func<string, string> formatValidationMessage)
        => maybeIssue is { } issue
            ? new ValidationIssue(formatValidationMessage(issue))
            : null;
}
