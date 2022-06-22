namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using System;
using System.Linq;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Validation;
using FluentAssertions;
using Xunit;

public class RecordValidatorTests
{
    [Fact]
    public void ReturnsIssues_WhenParsedRecordAlreadyHasIssues()
    {
        var parsedRecord = new ParsedRecord<DeserializedRecord>(
            RowNumber: 1,
            OutputRecord: null,
            new[] { new ValidationIssue(RowNumber: 1, InvalidColumnCount.FormatMessage()) });

        var issues = ImportRecordValidator.Validate(
            FakeImportCache.Create(),
            DateOnly.FromDateTime(DateTime.Today),
            new[] { parsedRecord });

        issues.Items.Should().HaveCount(1);
        issues.Items[0].Should().Be(parsedRecord.ValidationIssues.ElementAt(0));
    }

    [Fact]
    public void ThrowsNullReferenceException_WhenParsedRecordHasNoIssuesAndNoRecord()
    {
        var parsedRecord = new ParsedRecord<DeserializedRecord>(
            RowNumber: 1,
            OutputRecord: null,
            Array.Empty<ValidationIssue>());

        var action = () => ImportRecordValidator.Validate(
            FakeImportCache.Create(),
            DateOnly.FromDateTime(DateTime.Today),
            new[] { parsedRecord });

        action.Should().Throw<NullReferenceException>("parsedRecord.OutputRecord should never be null");
    }
}
