namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingBodyOrganisationWithDifferentOrganisation : Specification<UpdateBodyOrganisationCommandHandler, UpdateBodyOrganisation>
{
    private readonly Guid _bodyId;
    private readonly Guid _bodyOrganisationId;
    private readonly Guid _newOrganisationId;
    private readonly Guid _previousOrganisationId;

    public WhenUpdatingBodyOrganisationWithDifferentOrganisation(ITestOutputHelper helper) : base(helper)
    {
        _bodyId = Guid.NewGuid();
        _previousOrganisationId = Guid.NewGuid();
        _newOrganisationId = Guid.NewGuid();
        _bodyOrganisationId = Guid.NewGuid();
    }

    protected override UpdateBodyOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateBodyOrganisationCommandHandler>>().Object,
            session,
            Mock.Of<IDateTimeProvider>());

    private IEvent[] Events
        => new IEvent[] { new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now), new OrganisationCreated(
                _previousOrganisationId,
                "orgName",
                "ovoNumber",
                "shortName",
                string.Empty,
                "description",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null), new OrganisationCreated(
                _newOrganisationId,
                "orgName",
                "ovoNumber",
                "shortName",
                string.Empty,
                "description",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null), new BodyOrganisationAdded(
                _bodyId,
                _bodyOrganisationId,
                "bodyName",
                _previousOrganisationId,
                "orgName",
                null,
                null), new BodyAssignedToOrganisation(_bodyId, "Body", _previousOrganisationId, "orgName", _bodyOrganisationId),
        };

    private UpdateBodyOrganisation UpdateBodyOrganisationCommand
        => new(
            new BodyId(_bodyId),
            new BodyOrganisationId(_bodyOrganisationId),
            new OrganisationId(_newOrganisationId),
            new Period());

    [Fact]
    public async Task Publishes1Event()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesTheBodyOrganisation()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).Then();
        var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.OrganisationId.Should().Be(_newOrganisationId);
    }
}
