namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationKey;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationKey : OldSpecification2<UpdateOrganisationKeyCommandHandler, UpdateOrganisationKey>
{
    private Guid _organisationId;
    private Guid _keyId;
    private Guid _organisationKeyId;
    private const string Value = "13135/123lk.,m";
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenUpdatingAnOrganisationKey(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationKeyCommandHandler BuildHandler()
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(service => service.CanUseKeyType(It.IsAny<IUser>(), It.IsAny<Guid>()))
            .Returns(true);

        return new UpdateOrganisationKeyCommandHandler(
            new Mock<ILogger<UpdateOrganisationKeyCommandHandler>>().Object,
            Session,
            Mock.Of<IOrganisationRegistryConfiguration>(),
            securityServiceMock.Object);
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();

        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);

        return new List<IEvent>
        {
            new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
            new KeyTypeCreated(_keyId, "Sleutel A"),
            new OrganisationKeyAdded(_organisationId, _organisationKeyId, _keyId, "Sleutel A", Value, _validFrom, _validTo)
        };
    }

    protected override UpdateOrganisationKey When()
        => new (
            _organisationKeyId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AnOrganisationKeyUpdatedEventIsPublished()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyUpdated>>();
    }

    [Fact]
    public void TheEventContainsTheCorrectData()
    {
        var organisationKeyAdded = PublishedEvents.First().UnwrapBody<OrganisationKeyUpdated>();
        organisationKeyAdded.OrganisationId.Should().Be(_organisationId);
        organisationKeyAdded.KeyTypeId.Should().Be(_keyId);
        organisationKeyAdded.Value.Should().Be(Value);
        organisationKeyAdded.ValidFrom.Should().Be(_validFrom);
        organisationKeyAdded.ValidTo.Should().Be(_validTo);
    }
}
