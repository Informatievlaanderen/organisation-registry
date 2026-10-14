namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationCapacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

/// <summary>
/// The only caller of <see cref="RemoveOrganisationCapacity"/> is the
/// SyncRemovedItemsService background job, acting as
/// <see cref="WellknownUsers.SyncRemovedItemsService"/>. That well-known user
/// must hold an unrestricted <see cref="Permission.CanManageCapacities"/>
/// grant, otherwise scheduled capacity removals would always fail
/// authorization.
/// </summary>
public class WhenRemovingAnOrganisationCapacityAsSyncRemovedItemsService :
    Specification<RemoveOrganisationCapacityCommandHandler, RemoveOrganisationCapacity>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationCapacityId;

    public WhenRemovingAnOrganisationCapacityAsSyncRemovedItemsService(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationCapacityId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationCapacityCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<RemoveOrganisationCapacityCommandHandler>>(),
            session);

    private RemoveOrganisationCapacity RemoveOrganisationCapacityCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationCapacityId(_organisationCapacityId));

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationCapacityAdded OrganisationCapacityAdded()
        => new(
            _organisationId,
            _organisationCapacityId,
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid?>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid?>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid?>(),
            _fixture.Create<string>(),
            new Dictionary<Guid, string>(),
            DateTime.Now,
            DateTime.Now.AddDays(1));

    [Fact]
    public async Task Succeeds()
    {
        await Given(OrganisationCreated(), OrganisationCapacityAdded())
            .When(RemoveOrganisationCapacityCommand, WellknownUsers.SyncRemovedItemsService)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }
}
