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

public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependenciesButInAnotherFormalFramework
    : Specification<UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private Guid _grandParentBecameDaughterOfGreatGrandParentOrganisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _grandParentOrganisationId;
    private Guid _childOrganisationId;

    public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependenciesButInAnotherFormalFramework(
        ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
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
        var parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var grandParentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var greatGrandParentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
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
                parentOrganisationCreated.Id);
        var parentBecameDaughterOfGrandParent =
            new OrganisationFormalFrameworkAddedBuilder(
                parentOrganisationCreated.Id,
                formalFrameworkACreated.Id,
                grandParentOrganisationCreated.Id);
        var grandParentBecameDaughterOfGreatGrandParent =
            new OrganisationFormalFrameworkAddedBuilder(
                grandParentOrganisationCreated.Id,
                formalFrameworkACreated.Id,
                greatGrandParentOrganisationCreated.Id);

        _grandParentBecameDaughterOfGreatGrandParentOrganisationFormalFrameworkId =
            grandParentBecameDaughterOfGreatGrandParent.OrganisationFormalFrameworkId;
        _formalFrameworkBId = formalFrameworkBCreated.Id;
        _grandParentOrganisationId = grandParentOrganisationCreated.Id;
        _childOrganisationId = childOrganisationCreated.Id;
        return new IEvent[]
        {
            childOrganisationCreated.Build(), parentOrganisationCreated.Build(), grandParentOrganisationCreated.Build(),
            greatGrandParentOrganisationCreated.Build(), formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkACreated.Build(), formalFrameworkBCreated.Build(), childBecameDaughterOfParent.Build(),
            parentBecameDaughterOfGrandParent.Build(), grandParentBecameDaughterOfGreatGrandParent.Build()
        };
    }

    private UpdateOrganisationFormalFramework UpdateOrganisationFormalFrameworkCommand
        => new(
            _grandParentBecameDaughterOfGreatGrandParentOrganisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkBId),
            new OrganisationId(_grandParentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(_dateTimeProviderStub.Today),
            new ValidTo(_dateTimeProviderStub.Today));

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
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_grandParentOrganisationId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_dateTimeProviderStub.Today);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_dateTimeProviderStub.Today);
    }

    [Fact]
    public async Task AssignsAParent()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder).Then();
        var frameworkAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        frameworkAssignedToOrganisation.OrganisationId.Should().Be(_grandParentOrganisationId);
        frameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_childOrganisationId);
        frameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkBId);
    }
}
