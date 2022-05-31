namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System.Collections.Generic;
using System.Linq;
using Api.HostedServices;
using FluentAssertions;
using Xunit;

public class WhenParsingTheCsvImportFileContent
{
    private static List<OutputRecord> Parse(string csvToParse)
        => ImportFileParser.Parse(csvToParse).ToList();

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
        importedRecords[0].Should().BeEquivalentTo(new OutputRecord("REF1", "name1", "Ovo00025"));
        importedRecords[1].Should().BeEquivalentTo(new OutputRecord("REF2", "name2", "Ovo00026"));
        importedRecords[2].Should().BeEquivalentTo(new OutputRecord("REF3", "name3", ""));
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
        importedRecords[0].Should().BeEquivalentTo(new OutputRecord("REF1", "name1", "Ovo00025"));
        importedRecords[1].Should().BeEquivalentTo(new OutputRecord("REF2", "name2", "Ovo00026"));
    }

    [Fact]
    public void ItAddsInvalidColumnCount()
    {
        const string csvToParse = "reference;parent;name\n" +
                                  "REF1; Ovo00025\n" +
                                  "REF2; Ovo00026; name2; extra";
        var importedRecords = Parse(csvToParse);
        importedRecords.Should().HaveCount(2);
        importedRecords[0].Errors.Should().HaveCount(1);
        importedRecords[0].Errors[0].Should().Be("Rij heeft incorrect aantal kollomen.");
        importedRecords[1].Errors.Should().HaveCount(1);
        importedRecords[1].Errors[0].Should().Be("Rij heeft incorrect aantal kollomen.");
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
        importedRecords[0].Should().BeEquivalentTo(new OutputRecord("REF1", "name1", "Ovo00025"));
        importedRecords[1].Should().BeEquivalentTo(new OutputRecord("REF2", "name2", "Ovo00026"));
    }

    [Fact]
    public void ItAddsFieldValueRequired()
    {
        // Arrange
        const string csvToParse = "reference;parent;name\n" +
                                  "; Ovo00025; name1\n" +
                                  "REF2; Ovo00026;\n" +
                                  ";Ovo00027;";
        // Act
        var importedRecords = Parse(csvToParse);
        // Assert
        importedRecords.Should().HaveCount(3);
        importedRecords[0].Errors.Should().HaveCount(1);
        importedRecords[0].Errors[0].Should().Be("Rij ontbreekt waarde voor volgende kolommen: 'reference'.");
        importedRecords[1].Errors.Should().HaveCount(1);
        importedRecords[1].Errors[0].Should().Be("Rij ontbreekt waarde voor volgende kolommen: 'name'.");
        importedRecords[2].Errors.Should().HaveCount(1);
        importedRecords[2].Errors[0].Should().Be("Rij ontbreekt waarde voor volgende kolommen: 'reference', 'name'.");
    }


}
