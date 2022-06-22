namespace OrganisationRegistry.UnitTests.Import.Organisations;

using System.Collections.Immutable;
using System.Linq;
using Api.Import.Organisations.Validation;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

public class WhenValidatingTheCsvImportFileHeaders
{
    private const string LabelFormeleNaam = "formele naam";
    private const string LabelKorteNaam = "korte naam";

    private static CsvValidationResult Validate(string csvContent)
        => Validate("testfile.csv", csvContent);

    private static CsvValidationResult Validate(string csvFilename, string csvContent)
        => OrganisationCreationsCsvHeaderValidator.Validate(Mock.Of<ILogger>(), ImmutableList.Create<string>(LabelFormeleNaam, LabelKorteNaam), csvFilename, csvContent);

    [Fact]
    public void GivenAnEmptyCsvFile_ThenItReturnsInValid()
    {
        var validationResult = Validate("");

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues.Single().Description.Should().NotBeEmpty();
    }

    [Theory]
    [InlineData("parent", "'reference', 'name'")]
    [InlineData("reference; parent", "'name'")]
    [InlineData("parent; name", "'reference'")]
    [InlineData("reference; name", "'parent'")]
    public void GivenACsvFileWithMissingRequiredColumns_ThenItReturnsInValid(
        string csvFile,
        string expectedMissingColumns)
    {
        var validationResult = Validate(csvFile);

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues.Single().Description.Should()
            .Be(MissingRequiredColumns.FormatMessage(expectedMissingColumns));
    }

    [Theory]
    [InlineData("reference; parent; parent; name", "'parent'")]
    [InlineData("reference; reference; parent; parent; name", "'reference', 'parent'")]
    [InlineData("reference; reference; parent; name; name; name", "'reference', 'name'")]
    [InlineData("reference; parent; name; reference; parent; name", "'reference', 'parent', 'name'")]
    [InlineData("name; parent; reference; name", "'name'")]
    public void GivenACsvFileWithDuplicateColumnNames_ThenItReturnsInValid(
        string csvFile,
        string expectedDuplicateColumns)
    {
        var validationResult = Validate(csvFile);

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues.Single().Description.Should()
            .Be(DuplicateColumns.FormatMessage(expectedDuplicateColumns));
    }

    [Theory]
    [InlineData("reference; parent; name")]
    [InlineData("parent; name; reference")]
    [InlineData("reference; name; parent")]
    [InlineData("Reference; Name; parent")]
    [InlineData("ReFeReNcE; NAmE;   parent")]
    [InlineData("   reference;   name;   parent  ")]
    [InlineData("reference; parent; name; validity_start")]
    [InlineData("reference; parent; name; shortName")]
    [InlineData("reference; parent; name; article")]
    [InlineData("reference; parent; name; operationalValidity_Start")]
    [InlineData("reference; parent; name; validity_start; shortName; article; operationalValidity_Start")]
    [InlineData("reference; parent; name; validity_start; shortname; Article; operationalValidity_Start")]
    [InlineData("validity_start; reference; operationalValidity_Start; parent; name; shortName; article")]
    [InlineData("reference; parent; name; validity_start; shortName; article")]
    [InlineData("reference; parent; name; validity_start; shortName")]
    [InlineData("reference; parent; name; shortName; article; operationalValidity_Start")]
    [InlineData("reference; parent; name; article; operationalValidity_Start")]
    [InlineData("reference; parent; name; label#"+LabelFormeleNaam+";label#"+LabelKorteNaam)]
    [InlineData("reference; parent; name; LABEL#"+LabelFormeleNaam+";LABEL#"+LabelKorteNaam)]
    public void GivenAValidCsvFile_ThenItReturnsValid(string csvFile)
    {
        var validationResult = Validate(csvFile);

        validationResult.IsValid.Should().BeTrue();
        validationResult.ValidationIssues.Should().BeEmpty();
    }

    [Theory]
    [InlineData("reference; parent; name; blah", "'blah'")]
    [InlineData("reference; parent; name; blah; operationalvalidity_start; allena", "'blah', 'allena'")]
    [InlineData("reference; parent; name; lable#"+LabelFormeleNaam, "'lable#"+LabelFormeleNaam+"'")]
    [InlineData("reference; parent; name; label#blah", "'label#blah'")]
    public void GivenACsvFileWithInvalidColumns_ThenItReturnsInvalid(string csvFile, string expectedInvalidColumns)
    {
        var validationResult = Validate(csvFile);

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues.Single().Description.Should()
            .Be(InvalidColumns.FormatMessage(expectedInvalidColumns));
    }

    [Fact]
    public void GivenACsvFileWithNotAllRequiredColumnsAndInvalidColumns_ThenItReturnsInvalid()
    {
        var validationResult = Validate("reference; blah");

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues[0].Description.Should()
            .Be(MissingRequiredColumns.FormatMessage("'parent', 'name'"));
        validationResult.ValidationIssues[1].Description.Should().Be(InvalidColumns.FormatMessage("'blah'"));
    }

    [Fact]
    public void GivenACsvFileWithCommasInsteadOfSemiColons_ThenItReturnsInvalid()
    {
        var validationResult = Validate("reference, parent, name");

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues[0].Description.Should()
            .Be(MissingRequiredColumns.FormatMessage("'reference', 'parent', 'name'"));
        validationResult.ValidationIssues[1].Description.Should()
            .Be(InvalidColumns.FormatMessage("'reference, parent, name'"));
    }

    [Theory]
    [InlineData("test.vsc")]
    [InlineData("test.txt")]
    [InlineData("csv.txt")]
    [InlineData("test.csv.bin")]
    public void GivenACsvFilenameThatDoesntEndWithCsv_ThenItReturnsInvalid(string csvFilename)
    {
        var validationResult = Validate(csvFilename, "reference; parent; name");

        validationResult.IsValid.Should().BeFalse();
        validationResult.ValidationIssues.Single().Description.Should().Be(InvalidFilename.FormatMessage(csvFilename));
    }
}
