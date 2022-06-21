namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Strategy;

using SqlServer.Import.Organisations;

public record ProcessImportedFileResult(ImportOrganisationsStatusListItem StatusItem, string OutputFileContent, bool Success);