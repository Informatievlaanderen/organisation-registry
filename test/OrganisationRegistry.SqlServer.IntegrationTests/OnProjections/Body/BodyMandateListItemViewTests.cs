namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Body;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Function.Events;
using TestBases;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class BodyMandateListItemViewTests : ListViewTestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task ReassignedFunctionType_FromRemovedToLiveFunctionType_ClearsFlag()
    {
        var bodyId = _fixture.Create<Guid>();
        var bodySeatId = _fixture.Create<Guid>();
        var bodyMandateId = _fixture.Create<Guid>();
        var organisationId = _fixture.Create<Guid>();
        var removedFunctionTypeId = _fixture.Create<Guid>();
        var liveFunctionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new FunctionCreated(removedFunctionTypeId, _fixture.Create<string>()),
            new FunctionCreated(liveFunctionTypeId, _fixture.Create<string>()),
            CreateBodySeatAdded(bodyId, bodySeatId),
            CreateAssignedFunctionType(bodyId, bodySeatId, bodyMandateId, organisationId, removedFunctionTypeId),
            new FunctionTypeRemoved(removedFunctionTypeId),
            CreateReassignedFunctionType(bodyId, bodySeatId, bodyMandateId, organisationId, removedFunctionTypeId, liveFunctionTypeId));

        var item = await Context.BodyMandateList.SingleAsync(x => x.BodyMandateId == bodyMandateId);
        item.ScheduledForRemoval.Should().BeFalse();
    }

    [Fact]
    public async Task ReassignedFunctionType_ToRemovedFunctionType_SetsFlag()
    {
        var bodyId = _fixture.Create<Guid>();
        var bodySeatId = _fixture.Create<Guid>();
        var bodyMandateId = _fixture.Create<Guid>();
        var organisationId = _fixture.Create<Guid>();
        var liveFunctionTypeId = _fixture.Create<Guid>();
        var removedFunctionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            new FunctionCreated(liveFunctionTypeId, _fixture.Create<string>()),
            new FunctionCreated(removedFunctionTypeId, _fixture.Create<string>()),
            CreateBodySeatAdded(bodyId, bodySeatId),
            CreateAssignedFunctionType(bodyId, bodySeatId, bodyMandateId, organisationId, liveFunctionTypeId),
            new FunctionTypeRemoved(removedFunctionTypeId),
            CreateReassignedFunctionType(bodyId, bodySeatId, bodyMandateId, organisationId, liveFunctionTypeId, removedFunctionTypeId));

        var item = await Context.BodyMandateList.SingleAsync(x => x.BodyMandateId == bodyMandateId);
        item.ScheduledForRemoval.Should().BeTrue();
    }

    private BodySeatAdded CreateBodySeatAdded(Guid bodyId, Guid bodySeatId)
        => new(
            bodyId,
            bodySeatId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            null,
            null,
            _fixture.Create<bool>(),
            _fixture.Create<bool>(),
            null,
            null);

    private AssignedFunctionTypeToBodySeat CreateAssignedFunctionType(Guid bodyId, Guid bodySeatId, Guid bodyMandateId, Guid organisationId, Guid functionTypeId)
        => new(
            bodyId,
            bodyMandateId,
            bodySeatId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            organisationId,
            _fixture.Create<string>(),
            functionTypeId,
            _fixture.Create<string>(),
            null,
            null);

    private ReassignedFunctionTypeToBodySeat CreateReassignedFunctionType(Guid bodyId, Guid bodySeatId, Guid bodyMandateId, Guid organisationId, Guid previousFunctionTypeId, Guid newFunctionTypeId)
        => new(
            bodyId,
            bodyMandateId,
            bodySeatId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            organisationId,
            _fixture.Create<string>(),
            newFunctionTypeId,
            _fixture.Create<string>(),
            null,
            null,
            bodySeatId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            organisationId,
            _fixture.Create<string>(),
            previousFunctionTypeId,
            _fixture.Create<string>(),
            null,
            null);
}
