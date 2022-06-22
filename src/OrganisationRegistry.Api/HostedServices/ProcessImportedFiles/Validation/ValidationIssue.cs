namespace OrganisationRegistry.Api.HostedServices.ProcessImportedFiles.Validation;

public record ValidationIssue(int RowNumber, string Error);
