namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System;
using System.Collections.Generic;

public static class ImportFileValidator
{
    public static ValidationIssues Validate(
        ImportCache importCache,
        DateOnly today,
        List<ParsedRecord> parsedRecords)
        => FileValidator.Validate(importCache, today, parsedRecords);
}
