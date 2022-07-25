namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationCapacity;

using System;
using System.Threading.Tasks;
using AutoFixture;
using Capacity.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationCapacity
    : Specification<AddOrganisationCapacityCommandHandler, AddOrganisationCapacity>
{
    private readonly Fixture _fixture;
    private readonly Guid _capacityId;

    public WhenAddingAnOrganisationCapacity(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _capacityId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationCapacityCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<AddOrganisationCapacityCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub(),
            new DateTimeProviderStub(DateTime.Now));

    [Fact]
    public async Task WhenInFuture_PublishesOneEvent()
    {
        Given(OrganisationCreated,
                CapacityCreated)
            .When(FutureOrganisationCapacityCreated)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    private CapacityCreated CapacityCreated
        => new(_capacityId, _fixture.Create<string>());

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder();
}
