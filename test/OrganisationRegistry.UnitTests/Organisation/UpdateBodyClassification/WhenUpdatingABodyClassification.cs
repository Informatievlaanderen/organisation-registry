namespace OrganisationRegistry.UnitTests.Organisation.UpdateBodyClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using BodyClassification;
using BodyClassification.Commands;
using BodyClassification.Events;
using BodyClassificationType;
using BodyClassificationType.Events;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.Stubs;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingABodyClassification : Specification<BodyClassificationCommandHandlers, UpdateBodyClassification>
{
    private readonly Guid _bodyClassificationTypeId;
    private readonly Guid _bodyClassificationId;
    private readonly string _name;
    private readonly int _order;
    private readonly bool _active;
    private readonly Fixture _fixture;

    public WhenUpdatingABodyClassification(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _bodyClassificationTypeId = _fixture.Create<Guid>();
        _bodyClassificationId = _fixture.Create<Guid>();
        _name = _fixture.Create<string>();
        _order = _fixture.Create<int>();
        _active = _fixture.Create<bool>();
    }

    protected override BodyClassificationCommandHandlers BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<BodyClassificationCommandHandlers>>(),
            session,
            new UniqueValidatorStub<BodyClassification>());

    private UpdateBodyClassification UpdateBodyClassificationCommand
        => new(
            new BodyClassificationId(_bodyClassificationId),
            _name,
            _order,
            _active,
            new BodyClassificationTypeId(_bodyClassificationTypeId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(BodyClassificationTypeCreated(),BodyClassificationCreated())
            .When(UpdateBodyClassificationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var bodyClassificationUpdated = PublishedEvents[0].UnwrapBody<BodyClassificationUpdated>();
        bodyClassificationUpdated.BodyClassificationId.Should().Be(_bodyClassificationId);
        bodyClassificationUpdated.BodyClassificationTypeId.Should().Be(_bodyClassificationTypeId);
        bodyClassificationUpdated.Active.Should().Be(_active);
        bodyClassificationUpdated.Name.Should().Be(_name);
        bodyClassificationUpdated.Order.Should().Be(_order);
    }

    private BodyClassificationTypeCreated BodyClassificationTypeCreated()
        => new(_bodyClassificationTypeId, _fixture.Create<string>());

    private BodyClassificationCreated BodyClassificationCreated()
        => new(
            _bodyClassificationId,
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            _fixture.Create<bool>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>());
}
