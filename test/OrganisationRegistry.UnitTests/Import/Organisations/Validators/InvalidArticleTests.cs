namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.Validators;
using FluentAssertions;
using OrganisationRegistry.Organisation.Import;
using Xunit;

public class InvalidArticleTests
{
    private static DeserializedRecord GetDeserializedRecordWithArticle(string article)
        => new()
        {
            Article = Field.FromValue(ColumnNames.Article, article)
        };

    [Theory]
    [InlineData("de")]
    [InlineData("het")]
    [InlineData("")]
    public void ReturnsEmpty_WhenAtricleIsValid(string article)
    {
        var record = GetDeserializedRecordWithArticle(article);

        var issues = InvalidArticle.Validate(1, record);

        issues.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenAtricleHasNoValue()
    {
        var record = new DeserializedRecord() { Article = Field.NoValue(ColumnNames.Article) };

        var issues = InvalidArticle.Validate(1, record);

        issues.Should().BeNull();
    }

    [Theory]
    [InlineData("notValid")]
    [InlineData("thisNeither")]
    [InlineData("NotEvenThisOne")]
    public void ReturnsOneIssue_WhenAtricleIsNotValid(string article)
    {
        var record = GetDeserializedRecordWithArticle(article);

        var issues = InvalidArticle.Validate(1, record);

        issues.Should().BeEquivalentTo(
            new ValidationIssue(
                1,
                $"De waarde '{article}' is ongeldig voor kolom 'article' (Geldige waarden: 'de', 'het')."));
    }
}
