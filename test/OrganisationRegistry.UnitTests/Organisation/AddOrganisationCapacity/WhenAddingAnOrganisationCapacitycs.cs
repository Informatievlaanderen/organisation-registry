namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationCapacity;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Capacity;
using Capacity.Events;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationCapacity
    : Specification<AddOrganisationCapacityCommandHandler, AddOrganisationCapacity>
{
    private readonly Fixture _fixture;
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    private readonly Guid _capacityId;
    private readonly Guid _organisationId;
    private readonly Guid _organisationCapacityId;

    public WhenAddingAnOrganisationCapacity(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _capacityId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _organisationCapacityId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationCapacityCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<AddOrganisationCapacityCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub(),
            _dateTimeProviderStub);

    private AddOrganisationCapacity AddOrganisationCapacityCommand
        => new(
            _organisationCapacityId,
            new OrganisationId(_organisationId),
            new CapacityId(_capacityId),
            personId: null,
            functionId: null,
            locationId: null,
            contacts: null,
            new ValidFrom(_dateTimeProviderStub.Today.AddDays(value: 1)),
            new ValidTo(_dateTimeProviderStub.Today.AddDays(value: 2))
            );

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated,
                CapacityCreated)
            .When(AddOrganisationCapacityCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationCapacityAdded =  PublishedEvents[0].UnwrapBody<OrganisationCapacityAdded>();
        organisationCapacityAdded.OrganisationCapacityId.Should().Be(_organisationCapacityId);
        organisationCapacityAdded.CapacityId.Should().Be(_capacityId);
        organisationCapacityAdded.OrganisationId.Should().Be(_organisationId);
        organisationCapacityAdded.ValidTo.Should().Be(_dateTimeProviderStub.Today.AddDays(value: 1));
        organisationCapacityAdded.ValidFrom.Should().Be(_dateTimeProviderStub.Today.AddDays(value: 2));
    }

    private CapacityCreated CapacityCreated
        => new(_capacityId, _fixture.Create<string>());

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);
}
