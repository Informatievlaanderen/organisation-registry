namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;

using System.Collections.Generic;

public record ParsedRecord(
    int RowNumber,
    DeserializedRecord? OutputRecord,
    IEnumerable<ValidationIssue> ValidationIssues);
