namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAnActiveOrganisationsValidityChangesToThePast :
    Specification<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
{
    private readonly DateTime _yesterday;
    private Guid _organisationId;

    public WhenAnActiveOrganisationsValidityChangesToThePast(ITestOutputHelper helper) : base(helper)
    {
        _yesterday = DateTime.Today.AddDays(-1);
    }

    protected override UpdateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IEvent[] Events()
    {
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

        _organisationId = organisationCreatedBuilder.Id;

        return new IEvent[]
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(organisationCreatedBuilder.Id)
        };
    }

    private UpdateOrganisationInfo UpdateOrganisationInfoCommand
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            "",
            new List<PurposeId>(),
            false,
            new ValidFrom(_yesterday),
            new ValidTo(_yesterday),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task Publishes6Events()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(6);
    }

    [Fact]
    public async Task UpdatesOrganisationName()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationShortName()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationDescription()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationDescriptionUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationValidity()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationOperationalValidity()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationCreated = PublishedEvents[4].UnwrapBody<OrganisationOperationalValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task TheOrganisationBecomesActive()
    {
        await Given(Events()).When(UpdateOrganisationInfoCommand, TestUser.AlgemeenBeheerder).Then();
        var organisationBecameInactive = PublishedEvents[5].UnwrapBody<OrganisationBecameInactive>();
        organisationBecameInactive.Should().NotBeNull();
    }
}
