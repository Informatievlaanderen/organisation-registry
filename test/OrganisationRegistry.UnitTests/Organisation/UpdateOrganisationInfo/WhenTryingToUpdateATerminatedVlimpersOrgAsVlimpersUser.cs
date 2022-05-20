namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo;

using System;
using System.Collections.Generic;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser : OldSpecification2<
    UpdateOrganisationInfoLimitedToVlimpersCommandHandler, UpdateOrganisationInfoLimitedToVlimpers>
{
    private Guid _organisationId;

    public WhenTryingToUpdateATerminatedVlimpersOrgAsVlimpersUser(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationInfoLimitedToVlimpersCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationInfoLimitedToVlimpersCommandHandler>>().Object,
            Session,
            new DateTimeProviderStub(DateTime.Today));

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var fixture = new Fixture();
        var organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

        _organisationId = organisationCreatedBuilder.Id;

        return new List<IEvent>
        {
            organisationCreatedBuilder
                .WithValidity(null, null)
                .Build(),
            new OrganisationBecameActive(organisationCreatedBuilder.Id),
            new OrganisationPlacedUnderVlimpersManagement(organisationCreatedBuilder.Id),
            new OrganisationTerminatedV2(
                organisationCreatedBuilder.Id,
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<DateTime>(),
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
                fixture.Create<bool>(),
                fixture.Create<DateTime?>()
            ),
        };
    }

    protected override UpdateOrganisationInfoLimitedToVlimpers When()
        => new(
            new OrganisationId(_organisationId),
            "Test",
            Article.None,
            "testing",
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 4;

    [Fact]
    public void UpdatesOrganisationName()
    {
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void UpdatesShortName()
    {
        var organisationCreated = PublishedEvents[1].UnwrapBody<OrganisationShortNameUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void UpdatesOrganisationValidity()
    {
        var organisationCreated = PublishedEvents[2].UnwrapBody<OrganisationValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void UpdatesOrganisationOperationalValidity()
    {
        var organisationCreated = PublishedEvents[3].UnwrapBody<OrganisationOperationalValidityUpdated>();
        organisationCreated.Should().NotBeNull();
    }
}
