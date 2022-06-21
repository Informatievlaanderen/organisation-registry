namespace OrganisationRegistry.UnitTests.KeyTypes.RemoveKeyType;

using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.KeyTypes.Commands;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit.Abstractions;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Xunit;

public class WhenRemovingAKeyTypeToAnAlreadyRemovedKeyType
    : Specification<KeyTypeCommandHandlers, RemoveKeyType>
{
    private readonly Guid _keyTypeId;
    private const string KeyTypeName = "Sleutel A";


    public WhenRemovingAKeyTypeToAnAlreadyRemovedKeyType(ITestOutputHelper helper) : base(helper)
    {
        _keyTypeId = Guid.NewGuid();
    }

    private static IUser AlgemeenBeheerderUser
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override KeyTypeCommandHandlers BuildHandler(ISession session)
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(service => service.CanUseKeyType(It.IsAny<IUser>(), It.IsAny<Guid>()))
            .Returns(true);

        return new KeyTypeCommandHandlers(
            new Mock<ILogger<KeyTypeCommandHandlers>>().Object,
            session,
            Mock.Of<IUniqueNameValidator<KeyType>>()
        );
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new KeyTypeCreated(
                _keyTypeId,
                KeyTypeName
            ),
            new KeyTypeRemoved(_keyTypeId),
        };


    private RemoveKeyType RemoveKeyTypeCommand
        => new(new KeyTypeId(_keyTypeId));

    [Fact]
    public async Task PublishesTheCorrectNumberOfEvents()
        => await Given(Events)
            .When(RemoveKeyTypeCommand, AlgemeenBeheerderUser)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
}
