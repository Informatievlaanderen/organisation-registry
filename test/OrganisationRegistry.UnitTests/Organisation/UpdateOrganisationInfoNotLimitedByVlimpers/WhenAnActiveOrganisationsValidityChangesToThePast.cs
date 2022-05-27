namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfoNotLimitedByVlimpers;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Purpose;
using Purpose.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAnActiveOrganisationsValidityChangesToThePast :
    Specification<UpdateOrganisationNotLimitedToVlimpersCommandHandler, UpdateOrganisationInfoNotLimitedToVlimpers>
{
    private readonly Guid _purposeId;
    private readonly Guid _organisationId;

    public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _purposeId = Guid.NewGuid();
    }

    protected override UpdateOrganisationNotLimitedToVlimpersCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler>>().Object,
            session);

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(_organisationId), new PurposeCreated(_purposeId, "beleidsveld")
        };

    private UpdateOrganisationInfoNotLimitedToVlimpers UpdateOrganisationInfoNotLimitedToVlimpersCommand
        => new(
            new OrganisationId(_organisationId),
            "omschrijving",
            new List<PurposeId> { new(_purposeId) },
            true);

    [Fact]
    public async Task PublishesThreeEvents()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, TestUser.AlgemeenBeheerder).ThenItPublishesTheCorrectNumberOfEvents(3);
    }

    [Fact]
    public async Task UpdatesOrganisationDescription()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationDescriptionUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationValidity()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, TestUser.AlgemeenBeheerder).Then();

        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationPurposesUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationOperationalValidity()
    {
        await Given(Events).When(UpdateOrganisationInfoNotLimitedToVlimpersCommand, TestUser.AlgemeenBeheerder).Then();

        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationShowOnVlaamseOverheidSitesUpdated>();
        organisationCreated.Should().NotBeNull();
    }
}
