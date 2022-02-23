namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using LabelType;
    using LabelType.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationLabel : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationLabel>
    {
        private Guid _organisationId;
        private Guid _labelTypeId;
        private Guid _organisationLabelId;
        private string _value;
        private DateTime _validTo;
        private DateTime _validFrom;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(service => service.CanUseLabelType(It.IsAny<IUser>(), It.IsAny<Guid>()))
                .Returns(true);

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                new OrganisationRegistryConfigurationStub(),
                securityServiceMock.Object);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _labelTypeId = Guid.NewGuid();
            _organisationLabelId = Guid.NewGuid();
            _value = "13135/123lk.,m";
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new LabelTypeCreated(_labelTypeId, "Label A"),
                new OrganisationLabelAdded(_organisationId, _organisationLabelId, _labelTypeId, "Label A", _value, _validFrom, _validTo)
            };
        }

        protected override UpdateOrganisationLabel When()
        {
            return new UpdateOrganisationLabel(
                _organisationLabelId,
                new OrganisationId(_organisationId),
                new LabelTypeId(_labelTypeId),
                _value,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder()
                    .AddRoles(Role.OrganisationRegistryBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void AnOrganisationLabelUpdatedEventIsPublished()
        {
            PublishedEvents.First().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
        }

        [Fact]
        public void TheEventContainsTheCorrectData()
        {
            var organisationLabelAdded = PublishedEvents.First().UnwrapBody<OrganisationLabelUpdated>();
            organisationLabelAdded.OrganisationId.Should().Be(_organisationId);
            organisationLabelAdded.LabelTypeId.Should().Be(_labelTypeId);
            organisationLabelAdded.Value.Should().Be(_value);
            organisationLabelAdded.ValidFrom.Should().Be(_validFrom);
            organisationLabelAdded.ValidTo.Should().Be(_validTo);
        }

        public WhenUpdatingAnOrganisationLabel(ITestOutputHelper helper) : base(helper) { }
    }
}
