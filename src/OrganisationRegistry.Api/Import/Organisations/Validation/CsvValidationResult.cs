namespace OrganisationRegistry.Api.Import.Organisations.Validation;

using System.Collections.Immutable;

public record CsvValidationResult(bool IsValid, ImmutableList<ValidationIssue> ValidationIssues);
