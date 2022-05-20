namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfoNotLimitedByVlimpers;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Purpose;
using Purpose.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAnActiveOrganisationsValidityChangesToThePast : OldSpecification2<
    UpdateOrganisationNotLimitedToVlimpersCommandHandler, UpdateOrganisationInfoNotLimitedToVlimpers>
{
    private Guid _purposeId;
    private Guid _organisationId;

    public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationNotLimitedToVlimpersCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationNotLimitedToVlimpersCommandHandler>>().Object,
            Session);

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());
        _organisationId = organisationCreatedBuilder.Id;
        _purposeId = Guid.NewGuid();
        return new List<IEvent>
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(organisationCreatedBuilder.Id),
            new PurposeCreated(_purposeId, "beleidsveld")
        };
    }

    protected override UpdateOrganisationInfoNotLimitedToVlimpers When()
        => new(
            new OrganisationId(_organisationId),
            "omschrijving",
            new List<PurposeId> { new(_purposeId) },
            true);

    protected override int ExpectedNumberOfEvents
        => 3;


    [Fact]
    public void UpdatesOrganisationDescription()
    {
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationDescriptionUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void UpdatesOrganisationValidity()
    {
        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationPurposesUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void UpdatesOrganisationOperationalValidity()
    {
        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationShowOnVlaamseOverheidSitesUpdated>();
        organisationCreated.Should().NotBeNull();
    }
}
