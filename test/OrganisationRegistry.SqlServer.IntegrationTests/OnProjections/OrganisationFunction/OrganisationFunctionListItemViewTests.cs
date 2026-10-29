namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFunction;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Function.Events;
using OrganisationRegistry.Organisation.Events;
using TestBases;
using Xunit;

[Collection(SqlServerTestsCollection.Name)]
public class OrganisationFunctionListItemViewTests : ListViewTestBase
{
    private readonly Fixture _fixture = new();

    [Fact]
    public async Task FunctionUpdated_FromRemovedToLiveFunctionType_ClearsFlag()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationFunctionId = _fixture.Create<Guid>();
        var removedFunctionTypeId = _fixture.Create<Guid>();
        var liveFunctionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            CreateOrganisationCreated(organisationId),
            new FunctionCreated(removedFunctionTypeId, _fixture.Create<string>()),
            new FunctionCreated(liveFunctionTypeId, _fixture.Create<string>()),
            new OrganisationFunctionAdded(
                organisationId,
                organisationFunctionId,
                removedFunctionTypeId,
                _fixture.Create<string>(),
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                new Dictionary<Guid, string>(),
                null,
                null),
            new FunctionTypeRemoved(removedFunctionTypeId),
            CreateFunctionUpdated(organisationId, organisationFunctionId, removedFunctionTypeId, liveFunctionTypeId));

        var item = await Context.OrganisationFunctionList.SingleAsync(x => x.OrganisationFunctionId == organisationFunctionId);
        item.ScheduledForRemoval.Should().BeFalse();
    }

    [Fact]
    public async Task FunctionUpdated_ToRemovedFunctionType_SetsFlag()
    {
        var organisationId = _fixture.Create<Guid>();
        var organisationFunctionId = _fixture.Create<Guid>();
        var liveFunctionTypeId = _fixture.Create<Guid>();
        var removedFunctionTypeId = _fixture.Create<Guid>();

        await HandleEvents(
            CreateOrganisationCreated(organisationId),
            new FunctionCreated(liveFunctionTypeId, _fixture.Create<string>()),
            new FunctionCreated(removedFunctionTypeId, _fixture.Create<string>()),
            new OrganisationFunctionAdded(
                organisationId,
                organisationFunctionId,
                liveFunctionTypeId,
                _fixture.Create<string>(),
                _fixture.Create<Guid>(),
                _fixture.Create<string>(),
                new Dictionary<Guid, string>(),
                null,
                null),
            new FunctionTypeRemoved(removedFunctionTypeId),
            CreateFunctionUpdated(organisationId, organisationFunctionId, liveFunctionTypeId, removedFunctionTypeId));

        var item = await Context.OrganisationFunctionList.SingleAsync(x => x.OrganisationFunctionId == organisationFunctionId);
        item.ScheduledForRemoval.Should().BeTrue();
    }

    private OrganisationCreated CreateOrganisationCreated(Guid organisationId)
        => new(
            organisationId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            null,
            null,
            null,
            new List<Purpose>(),
            _fixture.Create<bool>(),
            _fixture.Create<DateTime>(),
            null,
            null,
            null);

    private OrganisationFunctionUpdated CreateFunctionUpdated(Guid organisationId, Guid organisationFunctionId, Guid previousFunctionTypeId, Guid newFunctionTypeId)
        => new(
            organisationId,
            organisationFunctionId,
            newFunctionTypeId,
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            null,
            null,
            previousFunctionTypeId,
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            null,
            null);
}
