namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenTryingToUpdateAVlimpersOrgAsVlimpersUser :
    Specification<UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
{
    private Guid _organisationId;

    public WhenTryingToUpdateAVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
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
            new OrganisationBecameActive(organisationCreatedBuilder.Id),
            new OrganisationPlacedUnderVlimpersManagement(organisationCreatedBuilder.Id)
        };
    }

    private UpdateOrganisationInfoLimitedToVlimpers UpdateOrganisationInfoLimitedToVlimpersCommand
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesFourEvents()
    {
        await Given(Events()).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(4);
    }

    [Fact]
    public async Task UpdatesOrganisationName()
    {
        await Given(Events()).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder).Then();
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesShortName()
    {
        await Given(Events()).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder).Then();
        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationValidity()
    {
        await Given(Events()).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder).Then();
        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationOperationalValidity()
    {
        await Given(Events()).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder).Then();
        var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationOperationalValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }
}
