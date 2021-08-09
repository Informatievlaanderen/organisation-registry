namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLocation
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Location;
    using OrganisationRegistry.Infrastructure.Events;
    using Location.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenMakingAnOrganisationLocationAMainLocationWhenThereAlreadyIsOne : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationLocation>
    {
        private Guid _organisationId;
        private Guid _organisationLocationId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private Guid _locationAId;
        private Guid _locationBId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _organisationLocationId = Guid.NewGuid();
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);
            _locationAId = Guid.NewGuid();
            _locationBId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null),
                new LocationCreated(_locationAId, "12345", "Albert 1 laan 32, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new LocationCreated(_locationBId, "12346", "Albert 1 laan 34, 1000 Brussel", "Albert 1 laan 32", "1000", "Brussel", "Belgie"),
                new OrganisationLocationAdded(_organisationId, Guid.NewGuid(), _locationAId, "Gebouw A", true, null, null, _validFrom, _validTo),
                new OrganisationLocationAdded(_organisationId, _organisationLocationId, _locationBId, "Gebouw B", false, null, null, _validFrom, _validTo)
            };
        }

        protected override UpdateOrganisationLocation When()
        {
            return new UpdateOrganisationLocation(
                _organisationLocationId,
                new OrganisationId(_organisationId),
                new LocationId(_locationBId),
                true,
                null,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyHasAMainLocationInThisPeriodException>();
            Exception.Message.Should().Be("Deze organisatie heeft reeds een hoofdlocatie binnen deze periode.");
        }

        public WhenMakingAnOrganisationLocationAMainLocationWhenThereAlreadyIsOne(ITestOutputHelper helper) : base(helper) { }
    }
}
