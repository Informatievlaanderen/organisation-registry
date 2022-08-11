namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationCapacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Capacity;
using Capacity.Events;
using ContactType;
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

public class WhenUpdatingAnOrganisationCapacity : Specification<UpdateOrganisationCapacityCommandHandler, UpdateOrganisationCapacity>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _capacityId;
    private readonly Guid _personId;
    private readonly Guid _funtionId;
    private readonly Guid _locationId;
    private readonly Guid _organisationCapacityId;

    public WhenUpdatingAnOrganisationCapacity(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _capacityId = _fixture.Create<Guid>();
        _personId = _fixture.Create<Guid>();
        _funtionId = _fixture.Create<Guid>();
        _locationId = _fixture.Create<Guid>();
        _organisationCapacityId = _fixture.Create<Guid>();
    }

    protected override UpdateOrganisationCapacityCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationCapacityCommandHandler>>(),
            session,
            new OrganisationRegistryConfigurationStub(),
            new DateTimeProviderStub(DateTime.Now));

    private UpdateOrganisationCapacity UpdateOrganisationCapacityCommand
        => new(
            _organisationCapacityId,
            new OrganisationId(_organisationId),
            new CapacityId(_capacityId),
            new PersonId(_personId),
            new FunctionTypeId(_funtionId),
            new LocationId(_locationId),
            new Dictionary<ContactTypeId, string>(),
            new ValidFrom(DateTime.Today.AddDays(-2)),
            new ValidTo(DateTime.Today.AddDays(-1)));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated(),
                CapacityCreated(),
                PersonCreated(),
                FunctionCreated(),
                LocationCreated(),
                OrganisationCapacityAdded())
            .When(UpdateOrganisationCapacityCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationCapacityUpdated = PublishedEvents[0].UnwrapBody<OrganisationCapacityUpdated>();
        organisationCapacityUpdated.OrganisationId.Should().Be(_organisationId);
        organisationCapacityUpdated.CapacityId.Should().Be(_capacityId);
        organisationCapacityUpdated.PersonId.Should().Be(_personId);
        organisationCapacityUpdated.FunctionId.Should().Be(_funtionId);
        organisationCapacityUpdated.LocationId.Should().Be(_locationId);
        organisationCapacityUpdated.OrganisationCapacityId.Should().Be(_organisationCapacityId);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private CapacityCreated CapacityCreated()
        => new(_capacityId, _fixture.Create<string>());

    private PersonCreated PersonCreated()
        => new(_personId, _fixture.Create<string>(), _fixture.Create<string>(), null, null);

    private FunctionCreated FunctionCreated()
        => new(_funtionId, _fixture.Create<string>());

    private LocationCreated LocationCreated()
        => new(
            _locationId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<string>());

    private OrganisationCapacityAdded OrganisationCapacityAdded()
        => new(
            _organisationId,
            _organisationCapacityId,
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            validFrom: null,
            validTo: null);
}
