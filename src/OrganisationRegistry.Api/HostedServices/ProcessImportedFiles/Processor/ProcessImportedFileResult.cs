namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Processor;

using OrganisationRegistry.SqlServer.Import.Organisations;

public record ProcessImportedFileResult(ImportOrganisationsStatusListItem StatusItem, string OutputFileContent, bool Success);
