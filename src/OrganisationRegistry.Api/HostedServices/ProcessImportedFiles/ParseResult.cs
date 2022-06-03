namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles;

using System.Collections.Generic;

public record ParseResult(List<ParsedRecord> ParsedRecords, string ImportFileContent);
