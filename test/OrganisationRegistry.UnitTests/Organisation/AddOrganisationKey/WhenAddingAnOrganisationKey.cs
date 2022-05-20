namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationKey;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.KeyTypes.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationKey : Specification<AddOrganisationKeyCommandHandler, AddOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _keyId;
    private readonly Guid _organisationKeyId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenAddingAnOrganisationKey(ITestOutputHelper helper) : base(helper, BuildHandler)
    {
        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _organisationId = Guid.NewGuid();
        _value = "12345ABC-@#$";
    }

    private static AddOrganisationKeyCommandHandler BuildHandler(ISession session)
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(
                service =>
                    service.CanUseKeyType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
            .Returns(true);

        return new AddOrganisationKeyCommandHandler(
            new Mock<ILogger<AddOrganisationKeyCommandHandler>>().Object,
            session,
            new OrganisationRegistryConfigurationStub(),
            securityServiceMock.Object);
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new KeyTypeCreated(_keyId, "Key A")
        };

    private AddOrganisationKey AddOrganisationKeyCommand
        => new(
            _organisationKeyId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    private static IUser AlgemeenBeheerderUser
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    [Fact]
    public async Task AnOrganisationKeyAddedEventIsPublished()
    {
        await Given(Events).When(AddOrganisationKeyCommand, AlgemeenBeheerderUser).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyAdded>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(AddOrganisationKeyCommand, AlgemeenBeheerderUser).Then();
        PublishedEvents.First().UnwrapBody<OrganisationKeyAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationKeyAdded(
                    _organisationId,
                    _organisationKeyId,
                    _keyId,
                    "Key A",
                    _value,
                    _validFrom,
                    _validTo
                ),
                opt =>
                    opt.Excluding(e => e.Timestamp)
                        .Excluding(e =>e .Version)
            );
    }

    [Fact]
    public async Task PublishesTheCorrectNumberOfEvents()
    {
        await Given(Events)
            .When(AddOrganisationKeyCommand, AlgemeenBeheerderUser)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
}
