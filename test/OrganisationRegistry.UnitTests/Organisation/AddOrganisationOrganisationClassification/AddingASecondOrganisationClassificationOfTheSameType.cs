namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationOrganisationClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationClassification;
using OrganisationClassification.Events;
using OrganisationClassificationType;
using OrganisationClassificationType.Events;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class AddingASecondOrganisationClassificationOfTheSameType
    : Specification<AddOrganisationOrganisationClassificationCommandHandler, AddOrganisationOrganisationClassification>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationClassificationTypeId;
    private readonly Guid _organisationClassification1Id;
    private readonly Guid _organisationClassification2Id;
    private readonly string _organisationClassificationTypeName;

    public AddingASecondOrganisationClassificationOfTheSameType(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationClassificationTypeId = _fixture.Create<Guid>();
        _organisationClassification1Id = _fixture.Create<Guid>();
        _organisationClassification2Id = _fixture.Create<Guid>();
        _organisationClassificationTypeName = _fixture.Create<string>();
    }

    protected override AddOrganisationOrganisationClassificationCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<AddOrganisationOrganisationClassificationCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub());

    private AddOrganisationOrganisationClassification AddOrganisationOrganisationClassification2Command(DateTime? validFrom = null, DateTime? validTo = null)
        => new(
            _fixture.Create<Guid>(),
            new OrganisationId(_organisationId),
            new OrganisationClassificationTypeId(_organisationClassificationTypeId),
            new OrganisationClassificationId(_organisationClassification2Id),
            validFrom.HasValue ? new ValidFrom(validFrom): new ValidFrom(),
            validTo.HasValue ? new ValidTo(validTo): new ValidTo());

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeAllowsDuplicates_ThenItPublishes1Event()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id))
            .When(AddOrganisationOrganisationClassification2Command(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeAllowsDuplicates_ThenItCreatesTheOrganisationOrganisationClassification()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id))
            .When(AddOrganisationOrganisationClassification2Command(), TestUser.AlgemeenBeheerder)
            .Then();

        var organisationOrganisationClassificationAddedEvent = PublishedEvents[0].UnwrapBody<OrganisationOrganisationClassificationAdded>();
        organisationOrganisationClassificationAddedEvent.OrganisationId.Should().Be(_organisationId);
        organisationOrganisationClassificationAddedEvent.OrganisationClassificationId.Should().Be(_organisationClassification2Id);
        organisationOrganisationClassificationAddedEvent.OrganisationClassificationTypeId.Should().Be(_organisationClassificationTypeId);
    }

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeDoesNotAllowDuplicatesAndNoOverlap_ThenItPublishes1Event()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: false),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id, validTo: DateTime.Today))
            .When(AddOrganisationOrganisationClassification2Command(validFrom: DateTime.Today.AddDays(1)), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeDoesNotAllowDuplicatesAndNoOverlap_ThenItCreatesTheOrganisationOrganisationClassification()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: false),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id, validTo: DateTime.Today))
            .When(AddOrganisationOrganisationClassification2Command(validFrom: DateTime.Today.AddDays(1)), TestUser.AlgemeenBeheerder)
            .Then();

        var organisationOrganisationClassificationAddedEvent = PublishedEvents[0].UnwrapBody<OrganisationOrganisationClassificationAdded>();
        organisationOrganisationClassificationAddedEvent.OrganisationId.Should().Be(_organisationId);
        organisationOrganisationClassificationAddedEvent.OrganisationClassificationId.Should().Be(_organisationClassification2Id);
        organisationOrganisationClassificationAddedEvent.OrganisationClassificationTypeId.Should().Be(_organisationClassificationTypeId);
    }

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeDoesNotAllowDuplicatesAndOverlap_ThenItPublishes0Events()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: false),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id))
            .When(AddOrganisationOrganisationClassification2Command(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task WhenTheOrganisationClassificationTypeDoesNotAllowDuplicatesAndOverlap_ThenItThrows()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: false),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationClassification1Id))
            .When(AddOrganisationOrganisationClassification2Command(), TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationClassificationTypeAlreadyCoupledToInThisPeriod>();
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
            .WithId(_organisationId);

    private OrganisationClassificationTypeCreated OrganisationClassificationTypeCreated
        => new(_organisationClassificationTypeId, _organisationClassificationTypeName);

    private OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(bool allow)
        => new(_organisationClassificationTypeId, allow);

    private OrganisationClassificationCreated OrganisationClassificationCreated(Guid id)
        => new(
            id,
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            null,
            true,
            _organisationClassificationTypeId,
            _organisationClassificationTypeName);

    private OrganisationOrganisationClassificationAdded OrganisationOrganisationClassificationAdded(Guid organisationClassificationId, DateTime? validFrom = null, DateTime? validTo = null)
        => new(
            _organisationId,
            _fixture.Create<Guid>(),
            _organisationClassificationTypeId,
            _organisationClassificationTypeName,
            organisationClassificationId,
            _fixture.Create<string>(),
            validFrom,
            validTo);
}
