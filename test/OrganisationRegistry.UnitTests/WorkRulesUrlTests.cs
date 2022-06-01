namespace OrganisationRegistry.UnitTests;

using System;
using FluentAssertions;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;

public class WorkRulesUrlTests
{
    [Theory]
    [InlineData("test", "Arbeidsreglement moet een Pdf zijn.", "Arbeidsreglement moet een geldige Url zijn.")]
    [InlineData("test.pdf", "Arbeidsreglement moet een geldige Url zijn.")]
    [InlineData("http://test", "Arbeidsreglement moet een Pdf zijn.")]
    [InlineData("http:/test.pdf", "Arbeidsreglement moet een geldige Url zijn.")]
    public void InvalidWorkRulesUrl(string sample, params string[] validationErrors)
    {
        WorkRulesUrl.IsValid(sample).Should().BeEquivalentTo(validationErrors);
    }

    [Theory]
    [InlineData("http://test.pdf")]
    [InlineData("https://test.pdf")]
    public void ValidWorkRulesUrl(string sample)
    {
        WorkRulesUrl.IsValid(sample).Should().BeEmpty();
    }

    [Theory]
    [InlineData("test")]
    [InlineData("http://test")]
    [InlineData("http://test.html")]
    public void InvalidWorkRulesUrlInstancePdf(string sample)
    {
        Func<WorkRulesUrl> createInstance = () => new WorkRulesUrl(sample);

        createInstance.Should().Throw<WorkRuleUrlShouldBePdf>();
    }

    [Theory]
    [InlineData("test.pdf")]
    [InlineData("http:/test.pdf")]
    public void InvalidWorkRulesUrlInstanceUrl(string sample)
    {
        Func<WorkRulesUrl> createInstance = () => new WorkRulesUrl(sample);

        createInstance.Should().Throw<WorkRuleUrlShouldBeValidUrl>();
    }

    [Theory]
    [InlineData("http://test.pdf")]
    [InlineData("https://test.pdf")]
    public void ValidWorkRulesUrlInstance(string sample)
    {
        var workRulesUrl = new WorkRulesUrl(sample);

        workRulesUrl.Should().NotBeNull();
    }
}
