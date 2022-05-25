namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey;

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
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.KeyTypes.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Commands;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganisationKey : Specification<RemoveOrganisationKeyCommandHandler, RemoveOrganisationKey>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationKeyId;
    private readonly Guid _keyId;
    private readonly string _value;
    private readonly DateTime _validFrom;
    private readonly DateTime _validTo;

    public WhenRemovingAnOrganisationKey(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();

        _keyId = Guid.NewGuid();
        _organisationKeyId = Guid.NewGuid();
        _value = "13135/123lk.,m";
        _validFrom = DateTime.Now.AddDays(1);
        _validTo = DateTime.Now.AddDays(2);
    }

    protected override RemoveOrganisationKeyCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<RemoveOrganisationKeyCommandHandler>>().Object,
            session);

    protected IEvent[] Events
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
            new KeyTypeCreated(_keyId, "Sleutel A"),
            new OrganisationKeyAdded(
                _organisationId,
                _organisationKeyId,
                _keyId,
                "Sleutel A",
                _value,
                _validFrom,
                _validTo)
        };

    protected RemoveOrganisationKey RemoveOrganisationKeyCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationKeyId(_organisationKeyId)
        );

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(RemoveOrganisationKeyCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
    [Fact]
    public async Task AnOrganisationKeyRemovedEventIsPublished()
    {
        await Given(Events).When(RemoveOrganisationKeyCommand, UserBuilder.AlgemeenBeheerder()).Then();
        PublishedEvents.First().Should().BeOfType<Envelope<OrganisationKeyRemoved>>();
    }

    [Fact]
    public async Task TheEventContainsTheCorrectData()
    {
        await Given(Events).When(RemoveOrganisationKeyCommand, UserBuilder.AlgemeenBeheerder()).Then();
        var organisationKeyRemoved = PublishedEvents.First().UnwrapBody<OrganisationKeyRemoved>();
        organisationKeyRemoved.OrganisationId.Should().Be(_organisationId);
        organisationKeyRemoved.OrganisationKeyId.Should().Be(_organisationKeyId);
    }
}
