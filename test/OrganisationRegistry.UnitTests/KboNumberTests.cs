namespace OrganisationRegistry.UnitTests;

using FluentAssertions;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;

public class KboNumberTests
{
    [Fact]
    public void ToDigits()
    {
        new KboNumber("0248.211.419")
            .ToDigitsOnly()
            .Should()
            .Be("0248211419");
    }

    [Fact]
    public void ToDots()
    {
        new KboNumber("0248211419")
            .ToDotFormat()
            .Should()
            .Be("0248.211.419");
    }

    [Theory]
    [InlineData("1234")]
    [InlineData("123456789")]
    public void WithInvalidKBONumber_ThenThrowsInvalidKBONumberException(string kboNumber)
    {
        Assert.Throws<InvalidKBONumber>(() => KboNumber.Validate(kboNumber));
    }

    [Fact]
    public void WithValidKBONumber_ThenDoesNotThrow()
    {
        var kboNumber = "0123456789";

        KboNumber.Validate(kboNumber);
    }
}
