namespace OrganisationRegistry.UnitTests.Organisation.UpdateBodyClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using BodyClassification;
using BodyClassification.Commands;
using BodyClassification.Events;
using BodyClassificationType;
using BodyClassificationType.Events;
using Exceptions;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.Stubs;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingABodyClassificationWhenNameIsNotUnique : Specification<BodyClassificationCommandHandlers, UpdateBodyClassification>
{
    private readonly Fixture _fixture;
    private readonly Guid _bodyClassificationTypeId;
    private readonly Guid _bodyClassificationId;

    public WhenUpdatingABodyClassificationWhenNameIsNotUnique(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _bodyClassificationTypeId = _fixture.Create<Guid>();
        _bodyClassificationId = _fixture.Create<Guid>();
    }

    protected override BodyClassificationCommandHandlers BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<BodyClassificationCommandHandlers>>(),
            session,
            new UniqueValidatorStub<BodyClassification>(isUnique: false));

    private UpdateBodyClassification UpdateBodyClassificationCommand
        => new(
            new BodyClassificationId(_bodyClassificationId),
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            _fixture.Create<bool>(),
            new BodyClassificationTypeId(_bodyClassificationTypeId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(BodyClassificationTypeCreated(),BodyClassificationCreated())
            .When(UpdateBodyClassificationCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<NameNotUniqueWithinType>();
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
