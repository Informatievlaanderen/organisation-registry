namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Api.HostedServices.ProcessImportedFiles;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using Xunit;

public class WhenParsingTheCsvImportFileContent
{
    private static List<ParsedRecord> Parse(string csvToParse)
        => ImportFileParser.ParseContent(csvToParse).ToList();

    [Fact]
    public void ItParsesRequiredFieldsLowerCaseHeaders()
    {
        // Arrange
        const string csvToParse = "reference;parent;name\n" +
                                  "REF1; Ovo00025; name1\n" +
                                  "REF2; Ovo00026; name2\n" +
                                  "REF3; ; name3";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(3);
        importedRecords[0].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF1"),
                Name = Field.FromValue(ColumnNames.Name, "name1"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00025")
            });
        importedRecords[1].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF2"),
                Name = Field.FromValue(ColumnNames.Name, "name2"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00026"),
            });
        importedRecords[2].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF3"),
                Name = Field.FromValue(ColumnNames.Name, "name3"),
                Parent = Field.FromValue(ColumnNames.Parent, ""),
            });
    }

    [Fact]
    public void ItParsesRequiredFieldsMixedCaseHeaders()
    {
        // Arrange
        const string csvToParse = "ReFeReNcE;PARENT;Name\n" +
                                  "REF1; Ovo00025; name1\n" +
                                  "REF2; Ovo00026; name2";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF1"),
                Name = Field.FromValue(ColumnNames.Name, "name1"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00025"),
            });
        importedRecords[1].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF2"),
                Name = Field.FromValue(ColumnNames.Name, "name2"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00026"),
            });
    }

    [Fact]
    public void ItParsesLabelFields()
    {
        // Arrange
        const string csvToParse = "reference;parent;name;label#some label;label#some other label\n" +
                                  "REF1; Ovo00025; name1;value whatever; other value\n" +
                                  "REF2; Ovo00026; name2; ; value";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF1"),
                Name = Field.FromValue(ColumnNames.Name, "name1"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00025"),
                Labels = ImmutableList.Create(Field.FromValue("label#some label", "value whatever"),Field.FromValue("label#some other label", "other value"))
            });
        importedRecords[1].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF2"),
                Name = Field.FromValue(ColumnNames.Name, "name2"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00026"),
                Labels = ImmutableList.Create(Field.FromValue("label#some other label", "value"))
            });
    }

    [Fact]
    public void ItAddsInvalidColumnCount()
    {
        const string csvToParse = "reference;parent;name\n" +
                                  "REF1; Ovo00025\n" +
                                  "REF2; Ovo00026; name2; extra";
        var importedRecords = Parse(csvToParse);
        importedRecords.Should().HaveCount(2);
        importedRecords[0].ValidationIssues.Should().HaveCount(1);
        importedRecords[0].ValidationIssues.First().Error.Should().Be("Rij heeft incorrect aantal kolommen.");
        importedRecords[1].ValidationIssues.Should().HaveCount(1);
        importedRecords[1].ValidationIssues.First().Error.Should().Be("Rij heeft incorrect aantal kolommen.");
    }

    [Fact]
    public void ItTrimsTheFieldValues()
    {
        const string csvToParse = "reference;parent;name\n" +
                                  "REF1   ;    Ovo00025;  name1  \n" +
                                  "  REF2  ; Ovo00026   ;   name2  ";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF1"),
                Name = Field.FromValue(ColumnNames.Name, "name1"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00025"),
            });
        importedRecords[1].OutputRecord.Should().BeEquivalentTo(
            new DeserializedRecord
            {
                Reference = Field.FromValue(ColumnNames.Reference, "REF2"),
                Name = Field.FromValue(ColumnNames.Name, "name2"),
                Parent = Field.FromValue(ColumnNames.Parent, "Ovo00026"),
            });
    }
}
