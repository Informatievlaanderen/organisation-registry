namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationCapacity;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenRemovingAnOrganisationCapacity : Specification<RemoveOrganisationCapacityCommandHandler, RemoveOrganisationCapacity>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _organisationCapacityId;

    public WhenRemovingAnOrganisationCapacity(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationCapacityId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationCapacityCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<RemoveOrganisationCapacityCommandHandler>>(), session);

    private RemoveOrganisationCapacity RemoveOrganisationCapacityCommand
        => new(
            new OrganisationId(_organisationId),
            new OrganisationCapacityId(_organisationCapacityId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated(), OrganisationCapacityAdded())
            .When(RemoveOrganisationCapacityCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationCapacityRemoved = PublishedEvents[0].UnwrapBody<OrganisationCapacityRemoved>();
        organisationCapacityRemoved.OrganisationId.Should().Be(_organisationId);
        organisationCapacityRemoved.OrganisationCapacityId.Should().Be(_organisationCapacityId);
    }

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
}
