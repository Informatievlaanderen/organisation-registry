namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;
using Validators;

public static class FileValidator
{
    public static ValidationIssues Validate(ImportCache importCache, DateOnly today, IReadOnlyList<ParsedRecord> parsedRecords)
        => new ValidationIssues()
            .AddRange(RecordValidator.Validate(importCache, today, parsedRecords))
            .AddRange(HasDuplicateReferences.Validate(parsedRecords));


}
