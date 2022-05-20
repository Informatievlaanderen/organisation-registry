namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework;

using System;
using System.Collections.Generic;
using System.Linq;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependenciesButInAnotherFormalFramework
    : OldSpecification2<UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();
    private Guid _grandParentBecameDaughterOfGreatGrandParentOrganisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _grandParentOrganisationId;
    private Guid _childOrganisationId;

    public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependenciesButInAnotherFormalFramework(
        ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            Session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub());

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
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
        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationCreated.Build(),
            grandParentOrganisationCreated.Build(),
            greatGrandParentOrganisationCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkACreated.Build(),
            formalFrameworkBCreated.Build(),
            childBecameDaughterOfParent.Build(),
            parentBecameDaughterOfGrandParent.Build(),
            grandParentBecameDaughterOfGreatGrandParent.Build()
        };
    }

    protected override UpdateOrganisationFormalFramework When()
        => new(
            _grandParentBecameDaughterOfGreatGrandParentOrganisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkBId),
            new OrganisationId(_grandParentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(_dateTimeProviderStub.Today),
            new ValidTo(_dateTimeProviderStub.Today));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationFormalFramework()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_grandParentOrganisationId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_dateTimeProviderStub.Today);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_dateTimeProviderStub.Today);
    }

    [Fact]
    public void AssignsAParent()
    {
        var frameworkAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        frameworkAssignedToOrganisation.OrganisationId.Should().Be(_grandParentOrganisationId);
        frameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_childOrganisationId);
        frameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkBId);
    }
}
