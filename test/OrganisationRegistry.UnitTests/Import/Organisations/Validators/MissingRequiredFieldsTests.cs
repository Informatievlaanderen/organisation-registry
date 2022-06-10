namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using Xunit;

public class MissingRequiredFieldsTests
{
    [Fact]
    public void ReturnsNull_WhenAllRequiredFieldsArePressent()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, "REF"),
            Name = Field.FromValue(ColumnNames.Name, "NAME"),
            Parent = Field.FromValue(ColumnNames.Parent, "PARENT")
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeNull();
    }

    [Fact]
    public void ReturnsIssue_WhenReferenceIsMissing()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.NoValue(ColumnNames.Reference),
            Name = Field.FromValue(ColumnNames.Name, "NAME"),
            Parent = Field.FromValue(ColumnNames.Parent, "PARENT")
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(1, MissingRequiredFields.FormatMessage($"'{ColumnNames.Reference}'")));
    }

    [Fact]
    public void ReturnsIssue_WhenNameIsMissing()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, "REF"),
            Name = Field.NoValue(ColumnNames.Name),
            Parent = Field.FromValue(ColumnNames.Parent, "PARENT")
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(1, MissingRequiredFields.FormatMessage($"'{ColumnNames.Name}'")));
    }

    [Fact]
    public void ReturnsIssue_WhenParentIsMissing()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.FromValue(ColumnNames.Reference, "REF"),
            Name = Field.FromValue(ColumnNames.Name, "NAME"),
            Parent = Field.NoValue(ColumnNames.Parent)
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(1,  MissingRequiredFields.FormatMessage($"'{ColumnNames.Parent}'")));
    }

    [Fact]
    public void ReturnsIssue_WhenAllFieldsAreMissing()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.NoValue(ColumnNames.Reference),
            Name = Field.NoValue(ColumnNames.Name),
            Parent = Field.NoValue(ColumnNames.Parent)
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(
                1,
                MissingRequiredFields.FormatMessage(
                    $"'{ColumnNames.Reference}', '{ColumnNames.Parent}', '{ColumnNames.Name}'")));
    }

    [Fact]
    public void ReturnsIssue_WhenSomeFieldsAreMissing()
    {
        var record = new DeserializedRecord
        {
            Reference = Field.NoValue(ColumnNames.Reference),
            Name = Field.FromValue(ColumnNames.Name, "NAME"),
            Parent = Field.NoValue(ColumnNames.Parent)
        };

        var issue = MissingRequiredFields.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(
                1,
                MissingRequiredFields.FormatMessage($"'{ColumnNames.Reference}', '{ColumnNames.Parent}'")));
    }
}
