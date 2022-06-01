namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;

public record ParsedRecord(
    int RowNumber,
    DeserializedRecord? OutputRecord,
    IEnumerable<ValidationIssue> ValidationIssues);
