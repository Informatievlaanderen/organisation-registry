namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationRegulation;

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
using RegulationSubTheme;
using RegulationSubTheme.Events;
using RegulationTheme;
using RegulationTheme.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationRegulation : Specification<UpdateOrganisationRegulationCommandHandler, UpdateOrganisationRegulation>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationId;
    private readonly Guid _regulationThemeId;
    private readonly Guid _regulationSubThemeId;
    private readonly Guid _organisationRegulationId;

    public WhenUpdatingAnOrganisationRegulation(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _regulationThemeId = _fixture.Create<Guid>();
        _regulationSubThemeId = _fixture.Create<Guid>();
        _organisationRegulationId = _fixture.Create<Guid>();
    }

    protected override UpdateOrganisationRegulationCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<UpdateOrganisationRegulationCommandHandler>>(), session);

    private UpdateOrganisationRegulation UpdateOrganisationRegulationCommand
        => new(
            _organisationRegulationId,
            new OrganisationId(_organisationId),
            new RegulationThemeId(_regulationThemeId),
            new RegulationSubThemeId(_regulationSubThemeId),
            _fixture.Create<string>(),
            null,
            null,
            null,
            null,
            null,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated, RegulationThemeCreated, RegulationSubThemeCreated, OrganisationRegulationAdded)
            .When(UpdateOrganisationRegulationCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

       var organisationRegulationUpdate =  PublishedEvents[0].UnwrapBody<OrganisationRegulationUpdated>();
       organisationRegulationUpdate.OrganisationId.Should().Be(_organisationId);
       organisationRegulationUpdate.OrganisationRegulationId.Should().Be(_organisationRegulationId);
       organisationRegulationUpdate.RegulationThemeId.Should().Be(_regulationThemeId);
       organisationRegulationUpdate.RegulationSubThemeId.Should().Be(_regulationSubThemeId);
    }

    private OrganisationCreated OrganisationCreated
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private RegulationThemeCreated RegulationThemeCreated
        => new(_regulationThemeId, _fixture.Create<string>());

    private RegulationSubThemeCreated RegulationSubThemeCreated
        => new(_regulationSubThemeId, _fixture.Create<string>(), _regulationThemeId, _fixture.Create<string>());

    private OrganisationRegulationAdded OrganisationRegulationAdded
        => new(
            _organisationId,
            _organisationRegulationId,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null,
            null);
}

