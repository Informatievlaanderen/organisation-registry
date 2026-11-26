namespace OrganisationRegistry.UnitTests.Organisation;

using FluentAssertions;
using SqlServer.Organisation;
using Xunit;

/// <summary>
/// Normalisatie van codes voor het zoeken op de hoofdpagina (OR-3785). Zoeken is een deelmatch
/// op de genormaliseerde waarde; de normalisatie moet dus de schrijfwijzen opvangen
/// waarin gebruikers een OVO-nummer, KBO-nummer of Orafin-code aanleveren.
/// </summary>
public class OrganisationCodeNormalizerTests
{
    [Theory]
    [InlineData("OVO001234", "OVO001234")]
    [InlineData("ovo001234", "OVO001234")]
    [InlineData("  OVO001234  ", "OVO001234")]
    [InlineData("0123.456.789", "0123456789")]
    [InlineData("0123 456 789", "0123456789")]
    [InlineData("abc-123", "ABC123")]
    [InlineData("", "")]
    [InlineData("   ", "")]
    [InlineData(null, "")]
    public void NormalizesToUppercaseAlphanumerics(string? value, string expected)
        => OrganisationCodeNormalizer.Normalize(value).Should().Be(expected);

    [Theory]
    [InlineData("OVO001234", new[] { "OVO001234" })]
    [InlineData("ovo 001234", new[] { "OVO001234" })]
    [InlineData("0123.456.789", new[] { "0123456789" })]
    public void YieldsTheNormalizedValueAsCandidate(string value, string[] expected)
        => OrganisationCodeNormalizer.Candidates(value).Should().BeEquivalentTo(expected);

    [Theory]
    [InlineData("BE0123456789")]
    [InlineData("be 0123.456.789")]
    public void StripsTheBelgianCountryPrefixFromKboNumbers(string value)
        => OrganisationCodeNormalizer.Candidates(value)
            .Should().BeEquivalentTo("BE0123456789", "0123456789");

    [Fact]
    public void DoesNotPadNineDigitNumbers()
        => OrganisationCodeNormalizer.Candidates("123456789")
            .Should().BeEquivalentTo("123456789");

    [Theory]
    [InlineData("BEHEER")]
    [InlineData("BE12")]
    [InlineData("BE01234567890")]
    public void LeavesOtherValuesStartingWithBeAlone(string value)
        => OrganisationCodeNormalizer.Candidates(value)
            .Should().BeEquivalentTo(OrganisationCodeNormalizer.Normalize(value));

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData(null)]
    [InlineData("...")]
    public void YieldsNoCandidatesForAnEmptyValue(string? value)
        => OrganisationCodeNormalizer.Candidates(value).Should().BeEmpty();
}
