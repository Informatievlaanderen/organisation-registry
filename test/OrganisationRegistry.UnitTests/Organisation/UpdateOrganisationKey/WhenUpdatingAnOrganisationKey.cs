namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationKey;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationKey : Specification<UpdateOrganisationKeyCommandHandler, UpdateOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _keyId;
    private readonly Guid _organisationKeyId;
    private readonly string _value;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    public WhenUpdatingAnOrganisationKey(ITestOutputHelper helper) : base(helper)
    {
        _value = "13135/123lk.,m";
        _organisationId = Guid.NewGuid();

        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();

        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override UpdateOrganisationKeyCommandHandler BuildHandler(ISession session)
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(service => service.CanUseKeyType(It.IsAny<IUser>(), It.IsAny<Guid>()))
            .Returns(true);

        return new UpdateOrganisationKeyCommandHandler(
            new Mock<ILogger<UpdateOrganisationKeyCommandHandler>>().Object,
            session,
            Mock.Of<IOrganisationRegistryConfiguration>());
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
            new KeyTypeCreated(_keyId, "Sleutel A"), new OrganisationKeyAdded(
                _organisationId,
                _organisationKeyId,
                _keyId,
                "Sleutel A",
                _value,
                _validFrom,
                _validTo),
        };

    private UpdateOrganisationKey UpdateOrganisationKeyCommand
        => new(
            _organisationKeyId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyId),
            _value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task Pubilshes1Event()
    {
        await Given(Events).When(UpdateOrganisationKeyCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AnOrganisationKeyUpdatedEventIsPublished()
    {
        await Given(Events).When(UpdateOrganisationKeyCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyUpdated>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(UpdateOrganisationKeyCommand, TestUser.AlgemeenBeheerder).Then();

        var organisationKeyAdded = PublishedEvents.First().UnwrapBody<OrganisationKeyUpdated>();
        organisationKeyAdded.OrganisationId.Should().Be(_organisationId);
        organisationKeyAdded.KeyTypeId.Should().Be(_keyId);
        organisationKeyAdded.Value.Should().Be(_value);
        organisationKeyAdded.ValidFrom.Should().Be(_validFrom);
        organisationKeyAdded.ValidTo.Should().Be(_validTo);
    }
}
