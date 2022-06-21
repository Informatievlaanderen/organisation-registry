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

public class WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture :
    Specification<UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;

    private readonly DateTime? _tomorrow;
    private Guid _childOrganisationCreatedId;
    private Guid _parentOrganisationBCreatedId;
    private Guid _formalFrameworkACreatedId;
    private Guid _childBecameDaughterOfParentOrganisationFormalFrameworkId;

    public WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
        _tomorrow = dateTimeProviderStub.Today.AddDays(1);
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub());

    private IEvent[] Events()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationACreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationBCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var formalFrameworkCategoryCreatedBuilder = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkACreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreatedBuilder.Id,
            formalFrameworkCategoryCreatedBuilder.Name);
        var childBecameDaughterOfParent = new OrganisationFormalFrameworkAddedBuilder(
            childOrganisationCreated.Id,
            formalFrameworkACreated.Id,
            parentOrganisationACreated.Id);
        var formalFrameworkAssignedToOrg =
            new FormalFrameworkAssignedToOrganisationBuilder(
                childBecameDaughterOfParent.OrganisationFormalFrameworkId,
                childBecameDaughterOfParent.FormalFrameworkId,
                childOrganisationCreated.Id,
                parentOrganisationACreated.Id);

        _childOrganisationCreatedId = childOrganisationCreated.Id;
        _parentOrganisationBCreatedId = parentOrganisationBCreated.Id;
        _formalFrameworkACreatedId = formalFrameworkACreated.Id;
        _childBecameDaughterOfParentOrganisationFormalFrameworkId =
            childBecameDaughterOfParent.OrganisationFormalFrameworkId;

        return new IEvent[]
        {
            childOrganisationCreated.Build(), parentOrganisationACreated.Build(), parentOrganisationBCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(), formalFrameworkACreated.Build(),
            childBecameDaughterOfParent.Build(), formalFrameworkAssignedToOrg.Build(),
        };
    }

    private UpdateOrganisationFormalFramework UpdateOrganisationFormalFrameworkCommand
        => new(
            _childBecameDaughterOfParentOrganisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkACreatedId),
            new OrganisationId(_childOrganisationCreatedId),
            new OrganisationId(_parentOrganisationBCreatedId),
            new ValidFrom(_tomorrow),
            new ValidTo(_tomorrow));

    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationFormalFramework()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationCreatedId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBCreatedId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_tomorrow);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_tomorrow);
    }

    [Fact]
    public async Task ClearsAParent()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder).Then();
        var formalFrameworkClearedFromOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationBCreatedId);
        formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkACreatedId);
    }
}
