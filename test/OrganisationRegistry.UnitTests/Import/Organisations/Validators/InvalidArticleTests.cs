namespace OrganisationRegistry.UnitTests.Import.Organisations.Validators;

using Api.HostedServices.ProcessImportedFiles;
using Api.HostedServices.ProcessImportedFiles.CreateOrganisations;
using Api.HostedServices.ProcessImportedFiles.Validation;
using FluentAssertions;
using Xunit;

public class InvalidArticleTests
{
    private static Field GetArticleField(string article)
        =>  Field.FromValue(ColumnNames.Article, article);

    [Theory]
    [InlineData("de")]
    [InlineData("het")]
    [InlineData("")]
    public void ReturnsEmpty_WhenAtricleIsValid(string article)
    {
        var issues = InvalidArticle.Validate(1, GetArticleField(article));

        issues.Should().BeNull();
    }

    [Fact]
    public void ReturnsEmpty_WhenAtricleHasNoValue()
    {
        var issues = InvalidArticle.Validate(1, Field.NoValue(ColumnNames.Article));

        issues.Should().BeNull();
    }

    [Theory]
    [InlineData("notValid")]
    [InlineData("thisNeither")]
    [InlineData("NotEvenThisOne")]
    public void ReturnsIssue_WhenAtricleIsNotValid(string article)
    {
        var issues = InvalidArticle.Validate(1, GetArticleField(article));

        issues.Should().BeEquivalentTo(
            new ValidationIssue(
                1,
                InvalidArticle.FormatMessage($"'{article}'")));
    }
}
