namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser :
    Specification<UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
{
    private readonly Guid _organisationId;
    private readonly Fixture _fixture;

    public WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();

        _organisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Today));

    private IEvent[] Events
        => new IEvent[] {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(_organisationId), new OrganisationPlacedUnderVlimpersManagement(_organisationId), new OrganisationTerminatedV2(
                _organisationId,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<DateTime>(),
                new FieldsToTerminateV2(
                    null,
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>(),
                    new Dictionary<Guid, DateTime>()),
                new KboFieldsToTerminateV2(
                    new Dictionary<Guid, DateTime>(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?(),
                    new KeyValuePair<Guid, DateTime>?()
                ),
                _fixture.Create<bool>(),
                _fixture.Create<DateTime?>()
            ),
        };

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
        await Given(Events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(4);
    }

    [Fact]
    public async Task UpdatesOrganisationName()
    {
        await Given(Events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .Then();
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesShortName()
    {
        await Given(Events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .Then();
        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationValidity()
    {
        await Given(Events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .Then();
        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public async Task UpdatesOrganisationOperationalValidity()
    {
        await Given(Events).When(UpdateOrganisationInfoLimitedToVlimpersCommand, TestUser.VlimpersBeheerder)
            .Then();
        var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationOperationalValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }
}
