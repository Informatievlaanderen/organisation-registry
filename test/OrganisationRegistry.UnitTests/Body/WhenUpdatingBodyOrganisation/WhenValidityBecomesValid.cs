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

public class WhenValidityBecomesValid : Specification<UpdateBodyOrganisationCommandHandler, UpdateBodyOrganisation>
{
    private readonly Guid _bodyId;
    private readonly Guid _bodyOrganisationId;
    private readonly Guid _organisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly DateTime _yesterday;

    public WhenValidityBecomesValid(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
        _yesterday = _dateTimeProviderStub.Today.AddDays(-1);
        _bodyId = Guid.NewGuid();
        _organisationId = Guid.NewGuid();
        _bodyOrganisationId = Guid.NewGuid();
    }

    protected override UpdateBodyOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateBodyOrganisationCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IEvent[] Events
        => new IEvent[]
        {
            new BodyRegistered(_bodyId, "Body", "1", "bod", "some body", DateTime.Now, DateTime.Now),
            new OrganisationCreated(
                _organisationId,
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
                _organisationId,
                "orgName",
                _yesterday,
                _yesterday)
        };

    private UpdateBodyOrganisation UpdateBodyOrganisationCommand
        => new(
            new BodyId(_bodyId),
            new BodyOrganisationId(_bodyOrganisationId),
            new OrganisationId(_organisationId),
            new Period());

    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheBodyOrganisation()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).Then();
        var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.OrganisationId.Should().Be(_organisationId);
    }

    [Fact]
    public async Task AssignsTheBodyOrganisation()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).Then();
        var bodyBalancedParticipationChanged = PublishedEvents[1].UnwrapBody<BodyAssignedToOrganisation>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.OrganisationId.Should().Be(_organisationId);
    }
}
