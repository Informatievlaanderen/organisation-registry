namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using Validation;

public record ParsedRecord<TDeserializedRecord>(
    int RowNumber,
    TDeserializedRecord? OutputRecord,
    IEnumerable<ValidationIssue> ValidationIssues);
