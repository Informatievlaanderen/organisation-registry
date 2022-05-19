namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationKey;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using OrganisationRegistry.KeyTypes;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.KeyTypes.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationKey : Specification<AddOrganisationKeyCommandHandler, AddOrganisationKey>
{
    private Guid _organisationId;
    private Guid _keyId;
    private Guid _organisationKeyId;
    private const string Value = "12345ABC-@#$";
    private DateTime _validTo;
    private DateTime _validFrom;

    public WhenAddingAnOrganisationKey(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationKeyCommandHandler BuildHandler()
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
            Session,
            new OrganisationRegistryConfigurationStub(),
            securityServiceMock.Object);
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
        _organisationId = Guid.NewGuid();
        return new List<IEvent>
        {
            new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null),
            new KeyTypeCreated(_keyId, "Key A")
        };
    }

    protected override AddOrganisationKey When()
        => new(
            _organisationKeyId,
            new OrganisationId(_organisationId),
            new KeyTypeId(_keyId),
            Value,
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AnOrganisationKeyAddedEventIsPublished()
    {
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyAdded>>();
    }

    [Fact]
    public void TheEventContainsTheCorrectData()
    {
        var organisationKeyAdded = PublishedEvents.First().UnwrapBody<OrganisationKeyAdded>();
        organisationKeyAdded.OrganisationId.Should().Be(_organisationId);
        organisationKeyAdded.KeyTypeId.Should().Be(_keyId);
        organisationKeyAdded.Value.Should().Be(Value);
        organisationKeyAdded.ValidFrom.Should().Be(_validFrom);
        organisationKeyAdded.ValidTo.Should().Be(_validTo);
    }
}
