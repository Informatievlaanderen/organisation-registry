namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validation;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ValidationIssuesFactory
{
    public static ValidationIssue? Create(int rowNumber, IList<string> issues, Func<string, string> formatValidationMessage)
        => issues.Any()
            ? new ValidationIssue(rowNumber, formatValidationMessage(string.Join(", ", issues.Select(i => $"'{i}'"))))
            : null;
}
