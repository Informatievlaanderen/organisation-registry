namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationKey
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using KeyTypes;
    using OrganisationRegistry.Infrastructure.Events;
    using KeyTypes.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationKey : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationKey>
    {
        private Guid _organisationId;
        private Guid _keyId;
        private Guid _organisationKeyId;
        private string _value;
        private DateTime _validTo;
        private DateTime _validFrom;

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

            _keyId = Guid.NewGuid();
            _organisationKeyId = Guid.NewGuid();
            _value = "13135/123lk.,m";
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new KeyTypeCreated(_keyId, "Sleutel A"),
                new OrganisationKeyAdded(_organisationId, _organisationKeyId, _keyId, "Sleutel A", _value, _validFrom, _validTo)
            };
        }

        protected override UpdateOrganisationKey When()
        {
            return new UpdateOrganisationKey(
                _organisationKeyId,
                new OrganisationId(_organisationId),
                new KeyTypeId(_keyId),
                _value,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void AnOrganisationKeyUpdatedEventIsPublished()
        {
            PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyUpdated>>();
        }

        [Fact]
        public void TheEventContainsTheCorrectData()
        {
            var organisationKeyAdded = PublishedEvents.First().UnwrapBody<OrganisationKeyUpdated>();
            organisationKeyAdded.OrganisationId.Should().Be(_organisationId);
            organisationKeyAdded.KeyTypeId.Should().Be(_keyId);
            organisationKeyAdded.Value.Should().Be(_value);
            organisationKeyAdded.ValidFrom.Should().Be(_validFrom);
            organisationKeyAdded.ValidTo.Should().Be(_validTo);
        }

        public WhenUpdatingAnOrganisationKey(ITestOutputHelper helper) : base(helper) { }
    }
}
