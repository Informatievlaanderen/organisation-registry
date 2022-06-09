namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using FluentAssertions;
using Xunit;

public class InvalidReferenceTests
{
    private static DeserializedRecord GetDeserializedRecordWithReference(string reference)
        => new()
        {
            Reference = Field.FromValue(ColumnNames.Reference, reference)
        };

    [Theory]
    [InlineData("REF1")]
    [InlineData("someText")]
    [InlineData("123abccba321")]
    public void ReturnNull_WhenReferenceIsValid(string reference)
    {
        var record = GetDeserializedRecordWithReference(reference);

        var issue = InvalidReference.Validate(1, record);

        issue.Should().BeNull();
    }

    [Theory]
    [InlineData("ovo123456")]
    [InlineData("ovoNotOk")]
    [InlineData("ovoid")]
    public void ReturnValidationissue_WhenReferenceStartsWithOVO(string ovoReference)
    {
        var record = GetDeserializedRecordWithReference(ovoReference);

        var issue = InvalidReference.Validate(1, record);

        issue.Should().BeEquivalentTo(
            new ValidationIssue(
                1,
                $"De waarde '{ovoReference}' is ongeldig voor kolom 'reference' (Waarde mag niet beginnen met 'OVO')."));
    }
}
