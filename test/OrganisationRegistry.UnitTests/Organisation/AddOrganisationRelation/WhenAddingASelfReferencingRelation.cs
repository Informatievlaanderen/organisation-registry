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

public class WhenAddingASelfReferencingRelation : Specification<AddOrganisationRelationCommandHandler, AddOrganisationRelation>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationRelationId;
    private readonly Guid _organisationRelationTypeId;
    private readonly Guid _organisationId;

    public WhenAddingASelfReferencingRelation(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationRelationId = _fixture.Create<Guid>();
        _organisationRelationTypeId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationRelationCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<AddOrganisationRelationCommandHandler>>(), session);

    private AddOrganisationRelation AddOrganisationRelationCommand
        => new(
            _organisationRelationId,
            new OrganisationRelationTypeId(_organisationRelationTypeId),
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task ThrowsException()
    {
        await Given(
                OrganisationCreated(),
                OrganisationRelationTypeCreated())
            .When(AddOrganisationRelationCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationCannotBeLinkedToItself>();

         }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationRelationTypeCreated OrganisationRelationTypeCreated()
        => new(_organisationRelationTypeId, _fixture.Create<string>(), null);
}
