namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationCapacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Capacity;
using Capacity.Events;
using ContactType;
using ContactType.Events;
using FluentAssertions;
using Function;
using Function.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Location;
using Location.Events;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Person;
using Person.Events;
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
    private readonly Guid _personId;
    private readonly Guid _functionId;
    private readonly Guid _locationId;
    private readonly Guid _contactTypeId1;

    public WhenAddingAnOrganisationCapacity(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _capacityId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _organisationCapacityId = _fixture.Create<Guid>();
        _personId = _fixture.Create<Guid>();
        _functionId = _fixture.Create<Guid>();
        _locationId = _fixture.Create<Guid>();
        _contactTypeId1 = _fixture.Create<Guid>();
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
            new PersonId(_personId),
            new FunctionTypeId(_functionId),
            new LocationId(_locationId),
            new Dictionary<ContactTypeId, string>
            {
                { new ContactTypeId(_contactTypeId1), _fixture.Create<string>() },
            },
            new ValidFrom(_dateTimeProviderStub.Yesterday),
            new ValidTo(_dateTimeProviderStub.Today.AddDays(value: 1))
        );

    [Fact]
    public async Task PublishesTwoEvent()
    {
        await Given(
                OrganisationCreated,
                CapacityCreated,
                PersonCreated,
                FunctionCreated,
                LocationCreated,
                ContactTypeCreated)
            .When(AddOrganisationCapacityCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(2);

        var organisationCapacityAdded = PublishedEvents[0].UnwrapBody<OrganisationCapacityAdded>();
        organisationCapacityAdded.OrganisationCapacityId.Should().Be(_organisationCapacityId);
        organisationCapacityAdded.CapacityId.Should().Be(_capacityId);
        organisationCapacityAdded.OrganisationId.Should().Be(_organisationId);
        organisationCapacityAdded.FunctionId.Should().Be(_functionId);
        organisationCapacityAdded.LocationId.Should().Be(_locationId);
        organisationCapacityAdded.PersonId.Should().Be(_personId);
        organisationCapacityAdded.ValidFrom.Should().Be(_dateTimeProviderStub.Yesterday);
        organisationCapacityAdded.ValidTo.Should().Be(_dateTimeProviderStub.Today.AddDays(value: 1));
    }

    private ContactTypeCreated ContactTypeCreated
        => new(_contactTypeId1, _fixture.Create<string>());

    private LocationCreated LocationCreated
        => new(_locationId, null, _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>(), _fixture.Create<string>());

    private FunctionCreated FunctionCreated
        => new(_functionId, _fixture.Create<string>());

    private PersonCreated PersonCreated
        => new(_personId, _fixture.Create<string>(), _fixture.Create<string>(), null, null);

    private CapacityCreated CapacityCreated
        => new(_capacityId, _fixture.Create<string>());

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);
}
