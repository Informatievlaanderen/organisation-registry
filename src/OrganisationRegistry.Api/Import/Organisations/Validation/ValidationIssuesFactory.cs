namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;

public static class ValidationIssuesFactory
{
    public static IEnumerable<ValidationIssue> Create(ImmutableList<string> issues, Func<string, string> formatValidationMessage)
        => issues.Any()
            ? ImmutableList.Create(new ValidationIssue(formatValidationMessage(string.Join(", ", issues))))
            : ImmutableList<ValidationIssue>.Empty;
}
