namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework;

using System;
using System.Collections.Generic;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationFormalFramework : OldSpecification2<AddOrganisationFormalFrameworkCommandHandler,
    AddOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();
    private Guid _formalFramworkId;
    private Guid _childOrganisationId;
    private Guid _parentOrganisationId;

    public WhenAddingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub,
            Mock.Of<IOrganisationRegistryConfiguration>()
        );

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var formalFrameworkCategoryCreatedBuilder = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreatedBuilder.Id,
            formalFrameworkCategoryCreatedBuilder.Name);

        _formalFramworkId = formalFrameworkCreated.Id;
        _childOrganisationId = childOrganisationCreated.Id;
        _parentOrganisationId = parentOrganisationCreated.Id;

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkCreated.Build()
        };
    }

    protected override AddOrganisationFormalFramework When()
        => new AddOrganisationFormalFramework(
            Guid.NewGuid(),
            new FormalFrameworkId(_formalFramworkId),
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationId),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

        organisationParentAdded.OrganisationFormalFrameworkId.Should().NotBeEmpty();
        organisationParentAdded.FormalFrameworkId.Should().Be(_formalFramworkId);
        organisationParentAdded.OrganisationId.Should().Be(_childOrganisationId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_parentOrganisationId);
        organisationParentAdded.ValidFrom.Should().BeNull();
        organisationParentAdded.ValidTo.Should().BeNull();
    }

    [Fact]
    public void AssignsAParent()
    {
        var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        parentAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationId);
        parentAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFramworkId);
        parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationId);
    }
}
