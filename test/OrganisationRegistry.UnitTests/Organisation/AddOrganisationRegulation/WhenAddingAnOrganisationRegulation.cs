namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationRegulation;

using System;
using System.Threading.Tasks;
using AutoFixture;
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

public class WhenAddingAnOrganisationRegulation : Specification<AddOrganisationRegulationCommandHandler, AddOrganisationRegulation>
{
    private readonly Fixture _fixture;
    private readonly Guid _organisationRegulationId;
    private readonly Guid _organisationId;
    private readonly Guid _regulationThemeId;
    private readonly Guid _regulationSubThemeId;

    public WhenAddingAnOrganisationRegulation(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationRegulationId = _fixture.Create<Guid>();
        _organisationId = _fixture.Create<Guid>();
        _regulationThemeId = _fixture.Create<Guid>();
        _regulationSubThemeId = _fixture.Create<Guid>();
    }

    protected override AddOrganisationRegulationCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<AddOrganisationRegulationCommandHandler>>(), session);

    private AddOrganisationRegulation AddOrganisationRegulation
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
        await Given(
                OrganisationCreated(),
                RegulationThemeCreated(),
                RegulationSubThemeCreated())
            .When(AddOrganisationRegulation, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private RegulationThemeCreated RegulationThemeCreated()
        => new(_regulationThemeId, _fixture.Create<string>());

    private RegulationSubThemeCreated RegulationSubThemeCreated()
        => new(_regulationSubThemeId, _fixture.Create<string>(), _regulationThemeId, _fixture.Create<string>());
}
