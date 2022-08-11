namespace OrganisationRegistry.UnitTests.Organisation.CreateBodyClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
using BodyClassification;
using BodyClassification.Commands;
using BodyClassification.Events;
using BodyClassificationType;
using BodyClassificationType.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenCreatingABodyClassification : Specification<BodyClassificationCommandHandlers, CreateBodyClassification>
{
    private readonly Fixture _fixture;
    private readonly Guid _bodyClassificationTypeId;
    private readonly Guid _bodyClassificationId;

    public WhenCreatingABodyClassification(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _bodyClassificationTypeId = _fixture.Create<Guid>();
        _bodyClassificationId = _fixture.Create<Guid>();
    }

    protected override BodyClassificationCommandHandlers BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<BodyClassificationCommandHandlers>>(),
            session,
            new UniqueValidatorStub<BodyClassification>());

    private CreateBodyClassification CreateBodyClassificationCommand
        => new(
            new BodyClassificationId(_bodyClassificationId),
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            _fixture.Create<bool>(),
            new BodyClassificationTypeId(_bodyClassificationTypeId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(BodyClassificationTypeCreated())
            .When(CreateBodyClassificationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var bodyClassificationCreated = PublishedEvents[0].UnwrapBody<BodyClassificationCreated>();
        bodyClassificationCreated.BodyClassificationId.Should().Be(_bodyClassificationId);
        bodyClassificationCreated.BodyClassificationTypeId.Should().Be(_bodyClassificationTypeId);
    }

    private BodyClassificationTypeCreated BodyClassificationTypeCreated()
        => new(_bodyClassificationTypeId, _fixture.Create<string>());
}
