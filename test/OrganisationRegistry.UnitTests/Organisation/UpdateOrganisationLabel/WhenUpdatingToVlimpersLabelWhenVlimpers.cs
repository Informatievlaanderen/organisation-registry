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
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingToVlimpersLabelWhenVlimpers : Specification<Organisation, OrganisationCommandHandlers, UpdateOrganisationLabel>
    {
        private Guid _organisationId;
        private Guid _vlimpersLabelTypeId;
        private Guid _organisationLabelId;
        private string _value;
        private DateTime _validTo;
        private DateTime _validFrom;
        private string _vlimpersLabelTypeName;
        private Guid _nonVlimpersLabelTypeId;
        private string _nonVlimpersLabelTypeName;
        private const string OvoNumber = "OVO000012345";

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(service => service.CanUseLabelType(It.IsAny<IUser>(), It.IsAny<Guid>()))
                .Returns(true);

            var configuration = new OrganisationRegistryConfigurationStub
            {
                Authorization = new AuthorizationConfigurationStub
                {
                    LabelIdsAllowedForVlimpers = new[] { _vlimpersLabelTypeId }
                }
            };

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                configuration,
                securityServiceMock.Object);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();

            _vlimpersLabelTypeId = Guid.NewGuid();
            _vlimpersLabelTypeName = "Vlimpers";
            _nonVlimpersLabelTypeId = Guid.NewGuid();
            _nonVlimpersLabelTypeName = "Niet vlimpers";

            _organisationLabelId = Guid.NewGuid();
            _value = "13135/123lk.,m";
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", OvoNumber, "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new LabelTypeCreated(_vlimpersLabelTypeId, _vlimpersLabelTypeName),
                new LabelTypeCreated(_nonVlimpersLabelTypeId, _nonVlimpersLabelTypeName),
                new OrganisationPlacedUnderVlimpersManagement(_organisationId),
                new OrganisationLabelAdded(_organisationId, _organisationLabelId, _nonVlimpersLabelTypeId, _nonVlimpersLabelTypeName, _value, _validFrom, _validTo)
            };
        }

        protected override UpdateOrganisationLabel When()
        {
            return new UpdateOrganisationLabel(
                _organisationLabelId,
                new OrganisationId(_organisationId),
                new LabelTypeId(_vlimpersLabelTypeId),
                _value,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder()
                    .AddRoles(Role.VlimpersBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void ThrowsAnException()
        {
            PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationLabelUpdated>>();
        }

        public WhenUpdatingToVlimpersLabelWhenVlimpers(ITestOutputHelper helper) : base(helper) { }
    }
}
