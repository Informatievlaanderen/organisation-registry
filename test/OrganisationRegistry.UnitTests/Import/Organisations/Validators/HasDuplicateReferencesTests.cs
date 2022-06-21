namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using System;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Strategy.CreateOrganisations.Validators;
using FluentAssertions;
using Xunit;

public class HasDuplicateReferencesTests
{
    private static ParsedRecord CreateParsedRecord(int rowNumber, string reference)
        => new(
            rowNumber,
            new DeserializedRecord() { Reference = Field.FromValue(ColumnNames.Reference, reference) },
            Array.Empty<ValidationIssue>());

    [Fact]
    public void ReturnsEmpty_WhenNoDuplicateReferencesAreFound()
    {
        var parsedRecords = new[]
        {
            CreateParsedRecord(1, "REF1"),
            CreateParsedRecord(2, "REF2"),
            CreateParsedRecord(3, "REF3"),
            CreateParsedRecord(4, "REF4"),
        };

        var issues = HasDuplicateReferences.Validate(parsedRecords);

        issues.Items.Should().BeEmpty();
    }

    [Fact]
    public void ReturnsIssues_WhenDuplicateReferencesAreFound()
    {
        var parsedRecords = new[]
        {
            CreateParsedRecord(1, "REF1"),
            CreateParsedRecord(2, "REF1"),
            CreateParsedRecord(3, "REF3"),
            CreateParsedRecord(4, "REF4"),
        };

        var issues = HasDuplicateReferences.Validate(parsedRecords);

        issues.Items.Should().HaveCount(2);
        issues.Items[0].Should().BeEquivalentTo(new ValidationIssue(1, HasDuplicateReferences.FormatMessage("REF1")));
        issues.Items[1].Should().BeEquivalentTo(new ValidationIssue(2, HasDuplicateReferences.FormatMessage("REF1")));
    }
}
