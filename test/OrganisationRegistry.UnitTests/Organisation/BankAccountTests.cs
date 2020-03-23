namespace OrganisationRegistry.UnitTests.Organisation
{
    using FluentAssertions;
    using OrganisationRegistry.Organisation;
    using Xunit;

    public class BankAccountTests
    {
        [Fact]
        public void TestWithBulgarianIban()
        {
            BankAccountNumber.CreateWithExpectedValidity("BG72UNCR70001522734456", true).IsValidIban.Should().BeTrue();
        }
    }
}
