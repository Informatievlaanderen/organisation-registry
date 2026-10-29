namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationFunction;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Function.Events;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.TestDataBuilders;
using WellknownUsers = OrganisationRegistry.Infrastructure.WellknownUsers;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// The only caller of <see cref="RemoveOrganisationFunction"/> is the
/// SyncRemovedItemsService background job, acting as
/// <see cref="WellknownUsers.SyncRemovedItemsService"/>. That well-known user
/// must hold an unrestricted <see cref="Permission.CanManageFunctions"/>
/// grant, otherwise scheduled function removals would always fail
/// authorization.
/// </summary>
public class WhenRemovingAnOrganisationFunctionAsSyncRemovedItemsService :
    Specification<RemoveOrganisationFunctionCommandHandler, RemoveOrganisationFunction>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationFunctionId;
    private readonly Guid _functionId;
    private readonly Guid _personId;

    public WhenRemovingAnOrganisationFunctionAsSyncRemovedItemsService(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationFunctionId = _fixture.Create<Guid>();
        _functionId = _fixture.Create<Guid>();
        _personId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationFunctionCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<RemoveOrganisationFunctionCommandHandler>>(),
            session);

    private RemoveOrganisationFunction RemoveOrganisationFunctionCommand
        => new(
            new OrganisationId(_organisationId),
            _organisationFunctionId);

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationFunctionAdded OrganisationFunctionAdded()
        => new(
            _organisationId,
            _organisationFunctionId,
            _functionId,
            _fixture.Create<string>(),
            _personId,
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            validFrom: null,
            validTo: null);

    private FunctionCreated FunctionTypeCreated
        => new(_functionId, _fixture.Create<string>());

    private Person.Events.PersonCreated PersonCreated
        => new(
            _personId,
            _fixture.Create<string>(),
            _fixture.Create<string>(),
            _fixture.Create<bool>() ? Person.Sex.Male : Person.Sex.Female,
            _fixture.Create<DateTime>());

    [Fact]
    public async Task Succeeds()
    {
        await Given(OrganisationCreated(), FunctionTypeCreated, PersonCreated, OrganisationFunctionAdded())
            .When(RemoveOrganisationFunctionCommand, WellknownUsers.SyncRemovedItemsService)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
}
