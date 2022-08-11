namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationFormalFramework;

using System;
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

public class WhenRemovingAnOrganisationFormalFramework : Specification<RemoveOrganisationFormalFrameworkCommandHandler, RemoveOrganisationFormalFramework>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationFormalFrameworkId;
    private readonly Guid _organisationId;

    public WhenRemovingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationFormalFrameworkId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
    }

    protected override RemoveOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<RemoveOrganisationFormalFrameworkCommandHandler>>(), session);

    private RemoveOrganisationFormalFramework RemoveOrganisationFormalFrameworkCommand
        => new(_organisationFormalFrameworkId, new OrganisationId(_organisationId));

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(
                OrganisationCreated(),
                OrganisationFormalFrameworkAdded())
            .When(RemoveOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationFormalFrameworkRemoved = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkRemoved>();
        organisationFormalFrameworkRemoved.OrganisationId.Should().Be(_organisationId);
        organisationFormalFrameworkRemoved.OrganisationFormalFrameworkId.Should().Be(_organisationFormalFrameworkId);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationFormalFrameworkAdded OrganisationFormalFrameworkAdded()
        => new(
            _organisationId,
            _organisationFormalFrameworkId,
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            _fixture.Create<Guid>(),
            _fixture.Create<string>(),
            null,
            null);
}
