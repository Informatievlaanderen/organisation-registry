namespace OrganisationRegistry.UnitTests
{
    using FluentAssertions;
    using OrganisationRegistry.Organisation;
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
    }
}
