namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationCapacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Capacity.Events;
using OrganisationRegistry.Function.Events;
using OrganisationRegistry.Organisation.Events;
using TestBases;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class OrganisationCapacityListItemViewTests : ListViewTestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task CapacityAdded_WithRemovedFunctionType_IsFlaggedForRemoval()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationCapacityId = _fixture.Create<Guid>();
        var capacityId = _fixture.Create<Guid>();
        var functionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new CapacityCreated(capacityId, _fixture.Create<string>()),
            new FunctionCreated(functionTypeId, _fixture.Create<string>()),
            new FunctionTypeRemoved(functionTypeId),
            CreateCapacityAdded(organisationId, organisationCapacityId, capacityId, functionTypeId));

        var item = await Context.OrganisationCapacityList.SingleAsync(x => x.OrganisationCapacityId == organisationCapacityId);
        item.ScheduledForRemoval.Should().BeTrue();
    }

    [Fact]
    public async Task CapacityAdded_WithLiveFunctionType_IsNotFlaggedForRemoval()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationCapacityId = _fixture.Create<Guid>();
        var capacityId = _fixture.Create<Guid>();
        var functionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new CapacityCreated(capacityId, _fixture.Create<string>()),
            new FunctionCreated(functionTypeId, _fixture.Create<string>()),
            CreateCapacityAdded(organisationId, organisationCapacityId, capacityId, functionTypeId));

        var item = await Context.OrganisationCapacityList.SingleAsync(x => x.OrganisationCapacityId == organisationCapacityId);
        item.ScheduledForRemoval.Should().BeFalse();
    }

    [Fact]
    public async Task CapacityUpdated_FromRemovedToLiveFunctionType_ClearsFlag()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationCapacityId = _fixture.Create<Guid>();
        var capacityId = _fixture.Create<Guid>();
        var removedFunctionTypeId = _fixture.Create<Guid>();
        var liveFunctionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new CapacityCreated(capacityId, _fixture.Create<string>()),
            new FunctionCreated(removedFunctionTypeId, _fixture.Create<string>()),
            new FunctionCreated(liveFunctionTypeId, _fixture.Create<string>()),
            new FunctionTypeRemoved(removedFunctionTypeId),
            CreateCapacityAdded(organisationId, organisationCapacityId, capacityId, removedFunctionTypeId),
            CreateCapacityUpdated(organisationId, organisationCapacityId, capacityId, removedFunctionTypeId, liveFunctionTypeId));

        var item = await Context.OrganisationCapacityList.SingleAsync(x => x.OrganisationCapacityId == organisationCapacityId);
        item.ScheduledForRemoval.Should().BeFalse();
    }

    [Fact]
    public async Task CapacityAdded_WithRemovedCapacity_IsFlaggedForRemoval()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationCapacityId = _fixture.Create<Guid>();
        var capacityId = _fixture.Create<Guid>();
        var functionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new CapacityCreated(capacityId, _fixture.Create<string>()),
            new FunctionCreated(functionTypeId, _fixture.Create<string>()),
            new CapacityRemoved(capacityId),
            CreateCapacityAdded(organisationId, organisationCapacityId, capacityId, functionTypeId));

        var item = await Context.OrganisationCapacityList.SingleAsync(x => x.OrganisationCapacityId == organisationCapacityId);
        item.ScheduledForRemoval.Should().BeTrue();
    }

    private OrganisationCapacityAdded CreateCapacityAdded(Guid organisationId, Guid organisationCapacityId, Guid capacityId, Guid functionTypeId)
        => new(
            organisationId,
            organisationCapacityId,
            capacityId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            functionTypeId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            new Dictionary<Guid, string>(),
            null,
            null);

    private OrganisationCapacityUpdated CreateCapacityUpdated(Guid organisationId, Guid organisationCapacityId, Guid capacityId, Guid previousFunctionTypeId, Guid newFunctionTypeId)
        => new(
            organisationId,
            organisationCapacityId,
            capacityId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            newFunctionTypeId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            new Dictionary<Guid, string>(),
            null,
            null,
            capacityId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            previousFunctionTypeId,
            _fixture.Create<string>(),
            null,
            string.Empty,
            new Dictionary<Guid, string>(),
            null,
            null,
            false);
}
