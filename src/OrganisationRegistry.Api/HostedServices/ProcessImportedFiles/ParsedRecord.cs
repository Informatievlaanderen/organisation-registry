namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using Organisation.Import;

public record ParsedRecord(
    int RowNumber,
    DeserializedRecord? OutputRecord,
    IEnumerable<ValidationIssue> ValidationIssues);
