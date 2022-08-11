namespace OrganisationRegistry.UnitTests.Organisation.UpdateCurrentOrganisationParent;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationParent
    : Specification<UpdateCurrentOrganisationParentCommandHandler, UpdateCurrentOrganisationParent>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _parentOrganisationId;

    public WhenUpdatingAnOrganisationParent(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _parentOrganisationId = _fixture.Create<Guid>();
    }

    protected override UpdateCurrentOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateCurrentOrganisationParentCommandHandler>>().Object,
            session,
            new DateTimeProvider()
        );

    private UpdateCurrentOrganisationParent UpdateCurrentOrganisationParentCommand
        => new(new OrganisationId(_organisationId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated(), OrganisationParentAdded())
            .When(UpdateCurrentOrganisationParentCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var parentAssignedToOrganisation = PublishedEvents[0].UnwrapBody<ParentAssignedToOrganisation>();
        parentAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
        parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationId);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationParentAdded OrganisationParentAdded()
        => new(_organisationId, _fixture.Create<Guid>(), _parentOrganisationId, _fixture.Create<string>(), null, null);
}
