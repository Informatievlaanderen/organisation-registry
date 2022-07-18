namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationOrganisationClassification;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using OrganisationClassification;
using OrganisationClassification.Events;
using OrganisationClassificationType;
using OrganisationClassificationType.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class UpdateOrganisationClassificationFromAnotherClassificationType
    : Specification<UpdateOrganisationOrganisationClassificationCommandHandler, UpdateOrganisationOrganisationClassification>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationClassificationType1Id;
    private readonly Guid _organisationClassificationType2Id;
    private readonly Guid _organisationClassificationId;
    private readonly string _organisationClassificationTypeName;
    private readonly Guid _organisationOrganisationClassificationId;

    public UpdateOrganisationClassificationFromAnotherClassificationType(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationClassificationType1Id = _fixture.Create<Guid>();
        _organisationClassificationType2Id = _fixture.Create<Guid>();
        _organisationClassificationId = _fixture.Create<Guid>();
        _organisationClassificationTypeName = _fixture.Create<string>();
        _organisationOrganisationClassificationId = _fixture.Create<Guid>();
    }

    protected override UpdateOrganisationOrganisationClassificationCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationOrganisationClassificationCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub());

    private UpdateOrganisationOrganisationClassification UpdateOrganisationOrganisationClassificationCommand()
        => new(
            _organisationOrganisationClassificationId,
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
                OrganisationClassificationCreated(_organisationClassificationId, _organisationClassificationType1Id),
                OrganisationOrganisationClassificationAdded(_organisationOrganisationClassificationId, _organisationClassificationType1Id, _organisationClassificationId))
            .When(UpdateOrganisationOrganisationClassificationCommand(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThenItThrowsException()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated(_organisationClassificationType1Id),
                OrganisationClassificationTypeCreated(_organisationClassificationType2Id),
                OrganisationClassificationCreated(_organisationClassificationId, _organisationClassificationType1Id),
                OrganisationOrganisationClassificationAdded(_organisationOrganisationClassificationId, _organisationClassificationType1Id, _organisationClassificationId))
            .When(UpdateOrganisationOrganisationClassificationCommand(), TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationClassificationIsNotPartOfSpecifiedOrganisationClassificationType>();
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder()
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

    private OrganisationOrganisationClassificationAdded OrganisationOrganisationClassificationAdded(Guid organisationOrganisationClassificationId, Guid organisationClassificationTypeId, Guid organisationClassificationId)
        => new(
            _organisationId,
            organisationOrganisationClassificationId,
            organisationClassificationTypeId,
            _organisationClassificationTypeName,
            organisationClassificationId,
            _fixture.Create<string>(),
            new ValidFrom(),
            new ValidTo());
}
