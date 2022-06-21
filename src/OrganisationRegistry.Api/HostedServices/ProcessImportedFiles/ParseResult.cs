namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;
using Strategy.CreateOrganisations;

public record ParseResult(List<ParsedRecord> ParsedRecords, string ImportFileContent);
