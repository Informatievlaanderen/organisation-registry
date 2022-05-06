namespace OrganisationRegistry.UnitTests.KeyTypes.RemoveKeyType
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.KeyTypes;
    using OrganisationRegistry.KeyTypes.Commands;
    using OrganisationRegistry.KeyTypes.Events;
    using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenRemovingAKeyType : OldSpecification<KeyType, KeyTypeCommandHandlers, RemoveKeyType>
    {
        private Guid _keyTypeId;
        private string _keyTypeName;

        protected override KeyTypeCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock
                .Setup(service => service.CanUseKeyType(It.IsAny<IUser>(), It.IsAny<Guid>()))
                .Returns(true);

            return new KeyTypeCommandHandlers(
                new Mock<ILogger<KeyTypeCommandHandlers>>().Object,
                Session,
                Mock.Of<IUniqueNameValidator<KeyType>>()
            );
        }

        protected override IEnumerable<IEvent> Given()
        {
            _keyTypeId = Guid.NewGuid();

            _keyTypeName = "Sleutel A";

            return new List<IEvent>
            {
                new KeyTypeCreated(
                    _keyTypeId,
                    _keyTypeName
                ),
            };
        }

        protected override RemoveKeyType When()
            => new (
                new KeyTypeId(_keyTypeId))
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };

        protected override int ExpectedNumberOfEvents
            => 1;

        [Fact]
        public void AKeyTypeRemovedEventIsPublished()
        {
            PublishedEvents.First().Should().BeOfType<Envelope<KeyTypeRemoved>>();
        }

        [Fact]
        public void TheEventContainsTheCorrectData()
        {
            var keyTypeRemoved = PublishedEvents.First().UnwrapBody<KeyTypeRemoved>();
            keyTypeRemoved.KeyTypeId.Should().Be(_keyTypeId);
        }

        public WhenRemovingAKeyType(ITestOutputHelper helper) : base(helper)
        {
        }
    }
}
