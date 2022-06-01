namespace OrganisationRegistry.UnitTests.KeyTypes.RemoveKeyType;

using System;
using System.Linq;
using System.Threading.Tasks;
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
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;

public class WhenRemovingAKeyType : Specification<KeyTypeCommandHandlers, RemoveKeyType>
{
    private readonly Guid _keyTypeId;
    private const string KeyTypeName = "Sleutel A";


    public WhenRemovingAKeyType(ITestOutputHelper helper) : base(helper)
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
        };


    private RemoveKeyType RemoveKeyTypeCommand
        => new(new KeyTypeId(_keyTypeId));

    [Fact]
    public async Task AKeyTypeRemovedEventIsPublished()
    {
        await Given(Events).When(RemoveKeyTypeCommand, AlgemeenBeheerderUser).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<KeyTypeRemoved>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(RemoveKeyTypeCommand, AlgemeenBeheerderUser).Then();
        var keyTypeRemoved = PublishedEvents.First().UnwrapBody<KeyTypeRemoved>();
        keyTypeRemoved.KeyTypeId.Should().Be(_keyTypeId);
    }

    [Fact]
    public async Task PublishesTheCorrectNumberOfEvents()
        => await Given(Events)
            .When(RemoveKeyTypeCommand, AlgemeenBeheerderUser)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
}
