namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.Infrastructure.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganisationKey : Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private Guid _organisationId;
    private Guid _organisationKeyId;

    protected override RemoveOrganisationKeyCommandHandler BuildHandler()
    {
        var securityServiceMock = new Mock<ISecurityService>();
        securityServiceMock
            .Setup(service => service.CanUseKeyType(It.IsAny<IUser>(), It.IsAny<Guid>()))
            .Returns(true);

        return new RemoveOrganisationKeyCommandHandler(
            new Mock<ILogger<RemoveOrganisationKeyCommandHandler>>().Object,
            Session);
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationId = Guid.NewGuid();

        var keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        var value = "13135/123lk.,m";
        var validFrom = DateTime.Now.AddDays(1);
        var validTo = DateTime.Now.AddDays(2);

        return new List<IEvent>
        {
            new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
            new KeyTypeCreated(keyId, "Sleutel A"),
            new OrganisationKeyAdded(_organisationId, _organisationKeyId, keyId, "Sleutel A", value, validFrom, validTo)
        };
    }

    protected override RemoveOrganisationKey When()
        => new(
            new OrganisationId(_organisationId),
            new OrganisationKeyId(_organisationKeyId)
        );

    protected override int ExpectedNumberOfEvents => 1;

    [Fact]
    public void AnOrganisationKeyRemovedEventIsPublished()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyRemoved>>();
    }

    [Fact]
    public void TheEventContainsTheCorrectData()
    {
        var organisationKeyRemoved = PublishedEvents.First().UnwrapBody<OrganisationKeyRemoved>();
        organisationKeyRemoved.OrganisationId.Should().Be(_organisationId);
        organisationKeyRemoved.OrganisationKeyId.Should().Be(_organisationKeyId);
    }

    public WhenRemovingAnOrganisationKey(ITestOutputHelper helper) : base(helper) { }
}
