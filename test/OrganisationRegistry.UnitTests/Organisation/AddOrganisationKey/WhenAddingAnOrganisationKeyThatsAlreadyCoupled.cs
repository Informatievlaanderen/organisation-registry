namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationKey
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.KeyTypes;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.KeyTypes.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;

    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationKeyThatsAlreadyCoupled : OldExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationKey>
    {
        private Guid _organisationId;
        private Guid _keyId;
        private Guid _organisationKeyId;
        private string _value;
        private DateTime _validTo;
        private DateTime _validFrom;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(service =>
                    service.CanUseKeyType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
                .Returns(true);

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                securityServiceMock.Object);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();
            _keyId = Guid.NewGuid();
            _organisationKeyId = Guid.NewGuid();
            _value = "ABC-12-@#$%";
            _validFrom = DateTime.Now.AddDays(1);
            _validTo = DateTime.Now.AddDays(2);

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
                new KeyTypeCreated(_keyId, "Key A"),
                new OrganisationKeyAdded(_organisationId, _organisationKeyId, _keyId, "Sleutel A", _value, _validFrom, _validTo)
            };
        }

        protected override AddOrganisationKey When()
        {
            return new AddOrganisationKey(
                Guid.NewGuid(),
                new OrganisationId(_organisationId),
                new KeyTypeId(_keyId),
                _value,
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<KeyAlreadyCoupledToInThisPeriod>();
            Exception.Message.Should().Be("Deze sleutel is in deze periode reeds gekoppeld aan de organisatie.");
        }

        public WhenAddingAnOrganisationKeyThatsAlreadyCoupled(ITestOutputHelper helper) : base(helper) { }
    }
}
