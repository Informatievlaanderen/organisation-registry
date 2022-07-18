namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationOrganisationClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
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

public class AddAOrganisationClassificationFromAnotherClassificationType : Specification<AddOrganisationOrganisationClassificationCommandHandler, AddOrganisationOrganisationClassification>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationClassificationType1Id;
    private readonly Guid _organisationClassificationType2Id;
    private readonly Guid _organisationClassificationId;
    private readonly string _organisationClassificationTypeName;

    public AddAOrganisationClassificationFromAnotherClassificationType(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationClassificationType1Id = _fixture.Create<Guid>();
        _organisationClassificationType2Id = _fixture.Create<Guid>();
        _organisationClassificationId = _fixture.Create<Guid>();
        _organisationClassificationTypeName = _fixture.Create<string>();
    }

    protected override AddOrganisationOrganisationClassificationCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<AddOrganisationOrganisationClassificationCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub());

    private AddOrganisationOrganisationClassification AddOrganisationOrganisationClassificationCommand()
        => new(
            _fixture.Create<Guid>(),
            new OrganisationId(_organisationId),
            new OrganisationClassificationTypeId(_organisationClassificationType2Id),
            new OrganisationClassificationId(_organisationClassificationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task ThenItPublishesNoEvents()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated(_organisationClassificationType1Id),
                OrganisationClassificationTypeCreated(_organisationClassificationType2Id),
                OrganisationClassificationCreated( _organisationClassificationId,_organisationClassificationType1Id))
            .When(AddOrganisationOrganisationClassificationCommand(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThenItThrowsException()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated(_organisationClassificationType1Id),
                OrganisationClassificationTypeCreated(_organisationClassificationType2Id),
                OrganisationClassificationCreated( _organisationClassificationId,_organisationClassificationType1Id))
            .When(AddOrganisationOrganisationClassificationCommand(), TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType>();
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
            .WithId(_organisationId);

    private OrganisationClassificationTypeCreated OrganisationClassificationTypeCreated(Guid organisationClassificationTypeId)
        => new(organisationClassificationTypeId, _organisationClassificationTypeName);

    private OrganisationClassificationCreated OrganisationClassificationCreated(Guid id, Guid organisationClassificationTypeId)
        => new(
            id,
            _fixture.Create<string>(),
            _fixture.Create<int>(),
            null,
            true,
            organisationClassificationTypeId,
            _organisationClassificationTypeName);
}
