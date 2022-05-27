namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework;

using System;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFramework :
    Specification<UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private Guid _organisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _childOrganisationId;
    private Guid _parentOrganisationBId;
    private Guid _parentOrganisationAId;

    private readonly DateTime? _tomorrow;

    public WhenUpdatingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
    {
        DateTimeProviderStub dateTimeProviderStub = new(DateTime.Now);
        _tomorrow = dateTimeProviderStub.Today.AddDays(1);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub()
        );

    private IEvent[] Events()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationACreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationBCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var formalFrameworkCategoryCreatedBuilder = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkACreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreatedBuilder.Id,
            formalFrameworkCategoryCreatedBuilder.Name);
        var formalFrameworkBCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreatedBuilder.Id,
            formalFrameworkCategoryCreatedBuilder.Name);
        var childBecameDaughterOfParent =
            new OrganisationFormalFrameworkAddedBuilder(
                childOrganisationCreated.Id,
                formalFrameworkACreated.Id,
                parentOrganisationACreated.Id);

        _organisationFormalFrameworkId = childBecameDaughterOfParent.OrganisationFormalFrameworkId;
        _formalFrameworkBId = formalFrameworkBCreated.Id;
        _childOrganisationId = childOrganisationCreated.Id;
        _parentOrganisationAId = parentOrganisationACreated.Id;
        _parentOrganisationBId = parentOrganisationBCreated.Id;

        return new IEvent[]
        {
            childOrganisationCreated.Build(), parentOrganisationACreated.Build(), parentOrganisationBCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(), formalFrameworkACreated.Build(),
            formalFrameworkBCreated.Build(), childBecameDaughterOfParent.Build()
        };
    }

    private UpdateOrganisationFormalFramework UpdateOrganisationFormalFrameworkCommand
        => new(
            _organisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkBId),
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationBId),
            new ValidFrom(_tomorrow),
            new ValidTo(_tomorrow));

    [Fact]
    public async Task Publishes1Event()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task UpdatesTheOrganisationBuilding()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.PreviousParentOrganisationId.Should()
            .Be(_parentOrganisationAId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        organisationFormalFrameworkUpdated.FormalFrameworkId.Should().Be(_formalFrameworkBId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_tomorrow);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_tomorrow);
    }
}
