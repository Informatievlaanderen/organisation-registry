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

public class UpdateSecondOrganisationClassificationWithTheSameValue
    : Specification<UpdateOrganisationOrganisationClassificationCommandHandler, UpdateOrganisationOrganisationClassification>


{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationClassificationTypeId;
    private readonly Guid _organisationClassification1Id;
    private readonly string _organisationClassificationTypeName;
    private readonly Guid _organisationOrganisationClassificationId;
    private readonly Guid _organisationClassification2Id;

    public UpdateSecondOrganisationClassificationWithTheSameValue(ITestOutputHelper helper) :
        base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationClassificationTypeId = _fixture.Create<Guid>();
        _organisationClassification1Id = _fixture.Create<Guid>();
        _organisationClassification2Id = _fixture.Create<Guid>();
        _organisationClassificationTypeName = _fixture.Create<string>();
        _organisationOrganisationClassificationId = _fixture.Create<Guid>();
    }

    protected override UpdateOrganisationOrganisationClassificationCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationOrganisationClassificationCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub());

    private UpdateOrganisationOrganisationClassification UpdateOrganisationOrganisationClassification1Command(DateTime? validFrom = null, DateTime? validTo = null)
        => new(
            _organisationOrganisationClassificationId,
            new OrganisationId(_organisationId),
            new OrganisationClassificationTypeId(_organisationClassificationTypeId),
            new OrganisationClassificationId(_organisationClassification2Id),
            validFrom.HasValue ? new ValidFrom(validFrom): new ValidFrom(),
            validTo.HasValue ? new ValidTo(validTo): new ValidTo());

    [Fact]
    public async Task ThenItPublishesNoEvents()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationOrganisationClassificationId, _organisationClassification1Id),
                OrganisationOrganisationClassificationAdded(_fixture.Create<Guid>(), _organisationClassification2Id))
            .When(UpdateOrganisationOrganisationClassification1Command(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThenItThrowsException()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassification1Id),
                OrganisationClassificationCreated(_organisationClassification2Id),
                OrganisationOrganisationClassificationAdded(_organisationOrganisationClassificationId, _organisationClassification1Id),
                OrganisationOrganisationClassificationAdded(_fixture.Create<Guid>(), _organisationClassification2Id))
            .When(UpdateOrganisationOrganisationClassification1Command(), TestUser.AlgemeenBeheerder)
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

    private OrganisationOrganisationClassificationAdded OrganisationOrganisationClassificationAdded(Guid organisationOrganisationClassificationId, Guid organisationClassificationId, DateTime? validFrom = null, DateTime? validTo = null)
        => new(
            _organisationId,
            organisationOrganisationClassificationId,
            _organisationClassificationTypeId,
            _organisationClassificationTypeName,
            organisationClassificationId,
            _fixture.Create<string>(),
            validFrom,
            validTo);
}
