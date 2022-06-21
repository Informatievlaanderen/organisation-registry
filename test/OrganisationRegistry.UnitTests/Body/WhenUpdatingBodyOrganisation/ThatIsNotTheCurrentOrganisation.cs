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

public class
    ThatIsNotTheCurrentOrganisation : Specification<UpdateBodyOrganisationCommandHandler, UpdateBodyOrganisation>
{
    private readonly Guid _bodyId;
    private readonly Guid _bodyOrganisationId;
    private readonly Guid _otherOrganisationId;
    private readonly Guid _previousOrganisationId;
    private readonly Guid _otherBodyOrganisationId;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    public ThatIsNotTheCurrentOrganisation(ITestOutputHelper helper) : base(helper)
    {
        _bodyId = Guid.NewGuid();
        _previousOrganisationId = Guid.NewGuid();
        _otherOrganisationId = Guid.NewGuid();
        _bodyOrganisationId = Guid.NewGuid();
        _otherBodyOrganisationId = Guid.NewGuid();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
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
            new OrganisationCreated(
                _otherOrganisationId,
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
                _dateTimeProviderStub.Today,
                _dateTimeProviderStub.Today),
            new BodyOrganisationAdded(
                _bodyId,
                _otherBodyOrganisationId,
                "other body name",
                _otherOrganisationId,
                "other orgName",
                DateTime.MinValue,
                DateTime.MinValue),
            new BodyAssignedToOrganisation(_bodyId, "Body", _previousOrganisationId, "orgName", _bodyOrganisationId),
        };

    private UpdateBodyOrganisation UpdateBodyOrganisationCommand
        => new(
            new BodyId(_bodyId),
            new BodyOrganisationId(_otherBodyOrganisationId),
            new OrganisationId(_otherOrganisationId),
            new Period(new ValidFrom(DateTime.MaxValue), new ValidTo(DateTime.MaxValue)));


    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesTheBodyOrganisation()
    {
        await Given(Events).When(UpdateBodyOrganisationCommand, TestUser.User).Then();
        var bodyBalancedParticipationChanged = PublishedEvents[0].UnwrapBody<BodyOrganisationUpdated>();
        bodyBalancedParticipationChanged.BodyId.Should().Be(_bodyId);

        bodyBalancedParticipationChanged.OrganisationId.Should().Be(_otherOrganisationId);
    }
}
