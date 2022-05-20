namespace OrganisationRegistry.UnitTests.Body.WhenUpdatingBodyOrganisation;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingBodyOrganisationWithSameOrganisation : OldSpecification2<UpdateBodyOrganisationCommandHandler, UpdateBodyOrganisation>
{
    private Guid _bodyId;
    private Guid _bodyOrganisationId;
    private Guid _previousOrganisationId;

    public WhenUpdatingBodyOrganisationWithSameOrganisation(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder().Build();

    protected override UpdateBodyOrganisationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateBodyOrganisationCommandHandler>>().Object,
            Session,
            Mock.Of<IDateTimeProvider>());

    protected override IEnumerable<IEvent> Given()
    {
        _bodyId = Guid.NewGuid();
        _previousOrganisationId = Guid.NewGuid();
        _bodyOrganisationId = Guid.NewGuid();
        return new List<IEvent>
        {
            new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
            new OrganisationCreated(
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
                null),
            new BodyOrganisationAdded(
                _bodyId,
                _bodyOrganisationId,
                "bodyName",
                _previousOrganisationId,
                "orgName",
                null,
                null),
            new BodyAssignedToOrganisation(_bodyId, "Body", _previousOrganisationId, "orgName", _bodyOrganisationId)
        };
    }

    protected override UpdateBodyOrganisation When()
        => new(
            new BodyId(_bodyId),
            new BodyOrganisationId(_bodyOrganisationId),
            new OrganisationId(_previousOrganisationId),
            new Period());

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void UpdatesTheBodyOrganisation()
    {
        var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.OrganisationId.Should().Be(_previousOrganisationId);
    }
}
