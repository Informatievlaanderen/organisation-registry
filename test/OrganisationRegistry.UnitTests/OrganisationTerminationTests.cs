namespace OrganisationRegistry.UnitTests
{
    using System;
    using System.Linq;
    using AutoFixture;
    using OrganisationRegistry.Organisation;
    using Xunit;

    public class OrganisationTerminationTests
    {
        [Fact]
        public void OrganisationCannotBeInFutureOfTerminationDate()
        {
            var fixture = new Fixture();

            var dateOfTermination = fixture.Create<DateTime>();

            var organisationState = new OrganisationState
            {
                Validity = new Period(
                    new ValidFrom(dateOfTermination.AddDays(1)),
                    new ValidTo(dateOfTermination.AddYears(1)))
            };

            Assert.Throws<OrganisationCannotBeTerminatedWithFieldsInTheFuture>(
                () => OrganisationTermination.Calculate(dateOfTermination,
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    Enumerable.Empty<Guid>(),
                    new KboState(),
                    organisationState));
        }
    }
}
