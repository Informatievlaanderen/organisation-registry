namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationRelation;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRelationType;
using OrganisationRelationType.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationRelation : Specification<UpdateOrganisationRelationCommandHandler, UpdateOrganisationRelation>
{
    private readonly Fixture _fixture;
    private readonly Guid _firstOrganisationId;
    private readonly Guid _secondOrganisationId;
    private readonly Guid _organisationRelationTypeId;
    private readonly Guid _organisationRelationId;

    public WhenUpdatingAnOrganisationRelation(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _firstOrganisationId = _fixture.Create<Guid>();
        _secondOrganisationId = _fixture.Create<Guid>();
        _organisationRelationTypeId = _fixture.Create<Guid>();
        _organisationRelationId = _fixture.Create<Guid>();
    }

    protected override UpdateOrganisationRelationCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<UpdateOrganisationRelationCommandHandler>>(), session);

    private UpdateOrganisationRelation UpdateOrganisationRelationCommand
        => new(
            _organisationRelationId,
            new OrganisationRelationTypeId(_organisationRelationTypeId),
            new OrganisationId(_firstOrganisationId),
            new OrganisationId(_secondOrganisationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(FirstOrganisationCreated, SecondOrganisationCreated, OrganisationRelationTypeCreated, OrganisationRelationAdded)
            .When(UpdateOrganisationRelationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationRegulationUpdate = PublishedEvents[0].UnwrapBody<OrganisationRelationUpdated>();
        organisationRegulationUpdate.OrganisationId.Should().Be(_firstOrganisationId);
        organisationRegulationUpdate.RelationId.Should().Be(_organisationRelationTypeId);
        organisationRegulationUpdate.RelatedOrganisationId.Should().Be(_secondOrganisationId);
        organisationRegulationUpdate.OrganisationRelationId.Should().Be(_organisationRelationId);
    }

    private OrganisationCreated FirstOrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_firstOrganisationId);

    private OrganisationCreated SecondOrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_secondOrganisationId);

    private OrganisationRelationTypeCreated OrganisationRelationTypeCreated
        => new(_organisationRelationTypeId, _fixture.Create<string>(), null);

    private OrganisationRelationAdded OrganisationRelationAdded
        => new(
            _firstOrganisationId,
            _fixture.Create<string>(),
            _organisationRelationId,
            _secondOrganisationId,
            _fixture.Create<string>(),
            _organisationRelationTypeId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            null);
}
