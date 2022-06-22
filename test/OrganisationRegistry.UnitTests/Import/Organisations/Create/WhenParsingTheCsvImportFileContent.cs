namespace OrganisationRegistry.UnitTests.Import.Organisations.Create;

using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Tests.Shared.TestDataBuilders;
using Xunit;

public class WhenParsingTheCsvImportFileContent
{
    private static List<ParsedRecord<DeserializedRecord>> Parse(string csvToParse)
        => ImportFileParser.ParseContent(csvToParse).ToList();

    [Fact]
    public void ItParsesRequiredFieldsLowerCaseHeaders()
    {
        // Arrange
        const string csvToParse = "reference;parent;name\n" +
                                  "REF1; Ovo000025; name1\n" +
                                  "REF2; Ovo000026; name2\n" +
                                  "REF3; ; name3";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(3);
        importedRecords[0].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF1", "name1").WithParent("Ovo000025").Build());
        importedRecords[1].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF2", "name2").WithParent("Ovo000026").Build());
        importedRecords[2].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF3", "name3").WithParent("").Build());
    }

    [Fact]
    public void ItParsesRequiredFieldsMixedCaseHeaders()
    {
        // Arrange
        const string csvToParse = "ReFeReNcE;PARENT;Name\n" +
                                  "REF1; Ovo000025; name1\n" +
                                  "REF2; Ovo000026; name2";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF1", "name1").WithParent("Ovo000025").Build());
        importedRecords[1].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF2", "name2").WithParent("Ovo000026").Build());
    }

    [Fact]
    public void ItParsesLabelFields()
    {
        // Arrange
        const string csvToParse = "reference;parent;name;label#some label;label#some other label\n" +
                                  "REF1; Ovo000025; name1;value whatever; other value\n" +
                                  "REF2; Ovo000026; name2; ; value";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF1", "name1")
                .WithParent("Ovo000025")
                .AddLabel("some label", "value whatever")
                .AddLabel("some other label", "other value")
                .Build());
        importedRecords[1].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF2", "name2")
                .WithParent("Ovo000026")
                .AddLabel("some other label", "value")
                .Build());
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
                                  "REF1   ;    Ovo000025;  name1  \n" +
                                  "  REF2  ; Ovo000026   ;   name2  ";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(2);
        importedRecords[0].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF1", "name1").WithParent("Ovo000025").Build());
        importedRecords[1].DeserializedRecord.Should().BeEquivalentTo(
            new CreateOrganisationsDeserializedRecordBuilder("REF2", "name2").WithParent("Ovo000026").Build());
    }
}
