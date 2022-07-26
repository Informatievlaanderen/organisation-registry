namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationFunction;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Function.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Organisation.Exceptions;
using Person;
using Person.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganistaionFunction
    : Specification<RemoveOrganisationFunctionCommandHandler, RemoveOrganisationFunction>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationFunctionId;
    private readonly Guid _functionId;
    private readonly Guid _personId;

    public WhenRemovingAnOrganistaionFunction(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationFunctionId = _fixture.Create<Guid>();
        _functionId = _fixture.Create<Guid>();
        _personId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationFunctionCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<RemoveOrganisationFunctionCommandHandler>>(), session);

    private RemoveOrganisationFunction RemoveOrganisationFunctionCommand
        => new(new OrganisationId(_organisationId), _organisationFunctionId);

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated,
                FunctionTypeCreated,
                PersonCreated,
                OrganisationFunctionAdded)
            .When(RemoveOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationFunctionRemoved = PublishedEvents.First().UnwrapBody<OrganisationFunctionRemoved>();
        organisationFunctionRemoved.OrganisationId.Should().Be(_organisationId);
        organisationFunctionRemoved.PersonId.Should().Be(_personId);
        organisationFunctionRemoved.OrganisationFunctionId.Should().Be(_organisationFunctionId);
    }

    [Fact]
    public async Task ThrowsException_WhenNotFound()
    {
        await Given(
                OrganisationCreated)
            .When(RemoveOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);

        await Given(
                OrganisationCreated)
            .When(RemoveOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationFunctionNotFound>();
    }

    [Fact]
    public async Task ThrowsException_WhenFunctionHasCapacities()
    {
        await Given(
                OrganisationCreated,
                FunctionTypeCreated,
                PersonCreated,
                OrganisationFunctionAdded,
                OrganisationCapacityAdded
            )
            .When(RemoveOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);

        await Given(
                OrganisationCreated,
                FunctionTypeCreated,
                PersonCreated,
                OrganisationFunctionAdded,
                OrganisationCapacityAdded
            )
            .When(RemoveOrganisationFunctionCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<FunctionStillUsedInCapacity>();
    }

    private OrganisationCapacityAdded OrganisationCapacityAdded
        => new(
            _organisationId,
            _fixture.Create<Guid>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _personId,
            _fixture.Create<string>(),
            _functionId,
            _fixture.Create<string>(),
            locationId: null,
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            validFrom: null,
            validTo: null);

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationFunctionAdded OrganisationFunctionAdded
        => new(
            _organisationId,
            _organisationFunctionId,
            _functionId,
            _fixture.Create<string>(),
            _personId,
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            validFrom: null,
            validTo: null
        );

    private FunctionCreated FunctionTypeCreated
        => new(_functionId, _fixture.Create<string>());

    private PersonCreated PersonCreated
        => new(
            _personId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<bool>() ? Sex.Male : Sex.Female,
            _fixture.Create<DateTime>());
}
