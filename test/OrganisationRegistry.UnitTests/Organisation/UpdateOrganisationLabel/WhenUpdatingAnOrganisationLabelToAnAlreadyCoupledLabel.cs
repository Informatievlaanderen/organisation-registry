namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationLabel
{
    using System;
    using System.Collections.Generic;
    using Configuration;
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

    public class WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationLabel>
    {
        private OrganisationLabelAdded _organisationLabelAdded;
        private OrganisationLabelAdded _anotherOrganisationLabelAdded;
        private Guid _organisationId;
        private Guid _labelTypeAId;
        private Guid _labelTypeBId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(service =>
                    service.CanUseLabelType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
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
            _labelTypeAId = Guid.NewGuid();
            _organisationLabelAdded = new OrganisationLabelAdded(_organisationId, Guid.NewGuid(), _labelTypeAId, "Label A", "123123456", null, null) { Version = 2 };
            _labelTypeBId = Guid.NewGuid();
            _anotherOrganisationLabelAdded = new OrganisationLabelAdded(_organisationId, Guid.NewGuid(), _labelTypeBId, "Label B", "123123456", null, null) { Version = 3 };

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new LabelTypeCreated(_labelTypeAId, "Label A"),
                new LabelTypeCreated(_labelTypeBId, "Label B"),
                _organisationLabelAdded,
                _anotherOrganisationLabelAdded
            };
        }

        protected override UpdateOrganisationLabel When()
        {
            return new UpdateOrganisationLabel(
                _anotherOrganisationLabelAdded.OrganisationLabelId,
                new OrganisationId(_organisationId),
                new LabelTypeId(_organisationLabelAdded.LabelTypeId),
                "987987654",
                new ValidFrom(null),
                new ValidTo(null))
            {
                User = new UserBuilder()
                    .AddRoles(Role.OrganisationRegistryBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<LabelAlreadyCoupledToInThisPeriod>();
            Exception.Message.Should().Be("Dit label is in deze periode reeds gekoppeld aan de organisatie.");
        }

        public WhenUpdatingAnOrganisationLabelToAnAlreadyCoupledLabel(ITestOutputHelper helper) : base(helper) { }
    }
}
