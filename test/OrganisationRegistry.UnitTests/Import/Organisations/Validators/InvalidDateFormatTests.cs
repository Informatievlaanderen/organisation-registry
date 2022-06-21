namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using Xunit;

public class InvalidDateFormatTests
{
    private static DeserializedRecord GetDeserializedRecordWithOperationalValidityStart(string operationalValidityStart)
        => new()
        {
            Validity_Start = Field.NoValue(ColumnNames.Validity_Start),
            OperationalValidity_Start = Field.FromValue(ColumnNames.OperationalValidity_Start, operationalValidityStart),
        };

    private static DeserializedRecord GetDeserializedRecordWithValidityStart(string validityStart)
        => new()
        {
            Validity_Start = Field.FromValue(ColumnNames.Validity_Start, validityStart),
            OperationalValidity_Start = Field.NoValue(ColumnNames.OperationalValidity_Start),
        };

    private static DeserializedRecord GetDeserializedRecordWithValidityStartAndOperationalValidityStart(
        string validityStart,
        string operationalValidityStart)
        => new()
        {
            Validity_Start = Field.FromValue(ColumnNames.Validity_Start, validityStart),
            OperationalValidity_Start = Field.FromValue(ColumnNames.OperationalValidity_Start, operationalValidityStart),
        };


    [Theory]
    [InlineData("2022-06-08")]
    [InlineData("2001-09-11")]
    public void ReturnsEmpty_WhenValidityStartHasValidFormatAndOperationalValidityStartHasNoValue(string validityStart)
    {
        var record = GetDeserializedRecordWithValidityStart(validityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().BeEmpty();
    }


    [Theory]
    [InlineData("2022-06-07")]
    [InlineData("2001-09-10")]
    public void ReturnsEmpty_WhenValidityStartHasNoValueAndOperationalValidityStartHasValidFormat(
        string operationalValidityStart)
    {
        var record = GetDeserializedRecordWithOperationalValidityStart(operationalValidityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().BeEmpty();
    }

    [Theory]
    [InlineData("2022-06-06", "2022-06-05")]
    [InlineData("2001-09-09", "2001-09-08")]
    public void ReturnsEmpty_WhenValidityStartHasValidFormatAndOperationalValidityStartHasValidFormat(
        string validityStart,
        string operationalValidityStart)
    {
        var record =
            GetDeserializedRecordWithValidityStartAndOperationalValidityStart(validityStart, operationalValidityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().BeEmpty();
    }

    [Fact]
    public void ReturnsEmpty_WhenValidityStartHasNoValueAndOperationalValidityStartHasNoValue()
    {
        var record = new DeserializedRecord();
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().BeEmpty();
    }

    [Theory]
    [InlineData("01-02-2036")]
    [InlineData("notADate")]
    [InlineData("2000-31-12")]
    public void ReturnsOneValidationIssue_WhenValididyStartHasInvalidFormatAndOperationalValidityStartHasNoValue(
        string invalidValidityStart)
    {
        var record = GetDeserializedRecordWithValidityStart(invalidValidityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().HaveCount(1).And.ContainEquivalentOf(
            new ValidationIssue(
                1,
                InvalidDateFormat.FormatMessage($"{invalidValidityStart}", $"{ColumnNames.Validity_Start}")));
    }

    [Theory]
    [InlineData("01-02-2035")]
    [InlineData("notADateEither")]
    [InlineData("2000-31-11")]
    public void ReturnsOneValidationIssue_WhenValididyStartHasNoValueAndOperationalValidityStartHasInvalidFormat(
        string invalidOperationalValidityStart)
    {
        var record = GetDeserializedRecordWithOperationalValidityStart(invalidOperationalValidityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().HaveCount(1).And.ContainEquivalentOf(
            new ValidationIssue(
                1,
                InvalidDateFormat.FormatMessage($"{invalidOperationalValidityStart}", $"{ColumnNames.OperationalValidity_Start}")));
    }

    [Theory]
    [InlineData("01-02-2034", "01-02-2033")]
    [InlineData("alsoNotADate", "noIdeaButNotADate")]
    [InlineData("2000-31-10", "2000-31-09")]
    public void
        ReturnsTwoValidationIssue_WhenValididyStartHasHasInvalidFormatAndOperationalValidityStartHasInvalidFormat(
            string invalidValidityStart,
            string invalidOperationalValidityStart)
    {
        var record = GetDeserializedRecordWithValidityStartAndOperationalValidityStart(
            invalidValidityStart,
            invalidOperationalValidityStart);
        var issues = InvalidDateFormat.Validate(1, record);
        issues.Should().HaveCount(2)
            .And.ContainEquivalentOf(
                new ValidationIssue(
                    1,
                    InvalidDateFormat.FormatMessage($"{invalidValidityStart}", $"{ColumnNames.Validity_Start}")))
            .And.ContainEquivalentOf(
                new ValidationIssue(
                    1,
                    InvalidDateFormat.FormatMessage($"{invalidOperationalValidityStart}", $"{ColumnNames.OperationalValidity_Start}")));
    }
}
