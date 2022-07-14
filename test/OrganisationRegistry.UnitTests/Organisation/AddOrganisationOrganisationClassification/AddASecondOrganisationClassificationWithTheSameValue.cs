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

public class AddASecondOrganisationClassificationWithTheSameValue
    : Specification<AddOrganisationOrganisationClassificationCommandHandler, AddOrganisationOrganisationClassification>


{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationClassificationTypeId;
    private readonly Guid _organisationClassificationId;
    private readonly string _organisationClassificationTypeName;

    public AddASecondOrganisationClassificationWithTheSameValue(ITestOutputHelper helper) :
        base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationClassificationTypeId = _fixture.Create<Guid>();
        _organisationClassificationId = _fixture.Create<Guid>();
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
            new OrganisationClassificationId(_organisationClassificationId),
            validFrom.HasValue ? new ValidFrom(validFrom): new ValidFrom(),
            validTo.HasValue ? new ValidTo(validTo): new ValidTo());

    [Fact]
    public async Task ThenItPublishesNoEvents()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassificationId),
                OrganisationOrganisationClassificationAdded(_organisationClassificationId))
            .When(AddOrganisationOrganisationClassification2Command(), TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThenItThrowsException()
    {
        await Given(
                OrganisationCreated,
                OrganisationClassificationTypeCreated,
                OrganisationClassificationTypeAllowDifferentClassificationsToOverlapChanged(allow: true),
                OrganisationClassificationCreated(_organisationClassificationId),
                OrganisationOrganisationClassificationAdded(_organisationClassificationId))
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
