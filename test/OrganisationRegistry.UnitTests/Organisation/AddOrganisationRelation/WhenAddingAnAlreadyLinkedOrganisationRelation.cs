namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationRelation;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationRelationType;
using OrganisationRelationType.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnAlreadyLinkedOrganisationRelation : Specification<AddOrganisationRelationCommandHandler, AddOrganisationRelation>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationRelationId;
    private readonly Guid _organisationRelationTypeId;
    private readonly Guid _organisationId;
    private readonly Guid _relatedOrganisationId;

    public WhenAddingAnAlreadyLinkedOrganisationRelation(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationRelationId = _fixture.Create<Guid>();
        _organisationRelationTypeId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _relatedOrganisationId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationRelationCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<AddOrganisationRelationCommandHandler>>(), session);

    private AddOrganisationRelation AddOrganisationRelationCommand
        => new(
            _organisationRelationId,
            new OrganisationRelationTypeId(_organisationRelationTypeId),
            new OrganisationId(_organisationId),
            new OrganisationId(_relatedOrganisationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated(),
                RelatedOrganisationCreated(),
                OrganisationRelationTypeCreated(),
                OrganisationRelationAdded())
            .When(AddOrganisationRelationCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationAlreadyLinkedToOrganisationInThisPeriod>();
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationCreated RelatedOrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_relatedOrganisationId);

    private OrganisationRelationTypeCreated OrganisationRelationTypeCreated()
        => new(_organisationRelationTypeId, _fixture.Create<string>(), null);

    private OrganisationRelationAdded OrganisationRelationAdded()
        => new(
            _organisationId,
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _relatedOrganisationId,
            _fixture.Create<string>(),
            _organisationRelationTypeId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            null);
}
