namespace OrganisationRegistry.UnitTests.KeyTypes.RemoveKeyType
{
    using System;
    using System.Collections.Generic;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.KeyTypes;
    using OrganisationRegistry.KeyTypes.Commands;
    using OrganisationRegistry.KeyTypes.Events;
    using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
    using Xunit.Abstractions;

    public class WhenRemovingAKeyTypeToAnAlreadyRemovedKeyType
        : OldSpecification<KeyType, KeyTypeCommandHandlers, RemoveKeyType>
    {
        private Guid _keyTypeId;
        private const string KeyTypeName = "Sleutel A";


        public WhenRemovingAKeyTypeToAnAlreadyRemovedKeyType(ITestOutputHelper helper) : base(helper)
        {
        }

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

            return new List<IEvent>
            {
                new KeyTypeCreated(
                    _keyTypeId,
                    KeyTypeName
                ),
                new KeyTypeRemoved(_keyTypeId)
            };
        }

        protected override RemoveKeyType When()
            => new(
                new KeyTypeId(_keyTypeId))
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };

        protected override int ExpectedNumberOfEvents
            => 0;
    }
}
