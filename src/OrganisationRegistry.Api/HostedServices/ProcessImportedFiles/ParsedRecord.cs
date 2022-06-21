namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;

public record ParsedRecord<TDeserializedRecord>(
    int RowNumber,
    TDeserializedRecord? OutputRecord,
    IEnumerable<ValidationIssue> ValidationIssues);
