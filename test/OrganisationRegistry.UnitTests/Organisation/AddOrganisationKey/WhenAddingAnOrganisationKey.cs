namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationKey
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

    public class WhenAddingAnOrganisationKey : Specification<Organisation, OrganisationCommandHandlers, AddOrganisationKey>
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
            _keyId = Guid.NewGuid();
            _organisationKeyId = Guid.NewGuid();
            _value = "12345ABC-@#$";
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);
            _organisationId = Guid.NewGuid();
            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", "Kindjes en gezinnetjes", new List<Purpose>(),false, null, null),
                new KeyTypeCreated(_keyId, "Key A")
            };
        }

        protected override AddOrganisationKey When()
        {
            return new AddOrganisationKey(
                _organisationKeyId,
                new OrganisationId(_organisationId),
                new KeyTypeId(_keyId),
                _value,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void AnOrganisationKeyAddedEventIsPublished()
        {
            PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyAdded>>();
        }

        [Fact]
        public void TheEventContainsTheCorrectData()
        {
            var organisationKeyAdded = PublishedEvents.First().UnwrapBody<OrganisationKeyAdded>();
            organisationKeyAdded.OrganisationId.Should().Be(_organisationId);
            organisationKeyAdded.KeyTypeId.Should().Be(_keyId);
            organisationKeyAdded.Value.Should().Be(_value);
            organisationKeyAdded.ValidFrom.Should().Be(_validFrom);
            organisationKeyAdded.ValidTo.Should().Be(_validTo);
        }

        public WhenAddingAnOrganisationKey(ITestOutputHelper helper) : base(helper) { }
    }
}
