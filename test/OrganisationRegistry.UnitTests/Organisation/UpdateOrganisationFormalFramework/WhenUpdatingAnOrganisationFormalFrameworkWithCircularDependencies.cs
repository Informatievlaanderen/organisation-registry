namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework;

using System;
using System.Collections.Generic;
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
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies : ExceptionSpecification<
    UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();
    private Guid _grandParentBecameDaughterOfGrandParentId;
    private Guid _formalFrameworkId;
    private Guid _grandParentOrganisationId;
    private Guid _childOrganisationId;

    public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(
        helper)
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
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreatedBuilder.Id,
            formalFrameworkCategoryCreatedBuilder.Name);
        var childBecameDaughterOfParent =
            new OrganisationFormalFrameworkAddedBuilder(
                childOrganisationCreated.Id,
                formalFrameworkCreated.Id,
                parentOrganisationCreated.Id);
        var parentBecameDaughterOfGrandParent =
            new OrganisationFormalFrameworkAddedBuilder(
                parentOrganisationCreated.Id,
                formalFrameworkCreated.Id,
                grandParentOrganisationCreated.Id);
        var grandParentBecameDaughterOfGrandParent =
            new OrganisationFormalFrameworkAddedBuilder(
                grandParentOrganisationCreated.Id,
                formalFrameworkCreated.Id,
                greatGrandParentOrganisationCreated.Id);

        _grandParentBecameDaughterOfGrandParentId = grandParentBecameDaughterOfGrandParent.OrganisationFormalFrameworkId;
        _formalFrameworkId = formalFrameworkCreated.Id;
        _grandParentOrganisationId = grandParentOrganisationCreated.Id;
        _childOrganisationId = childOrganisationCreated.Id;

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationCreated.Build(),
            grandParentOrganisationCreated.Build(),
            greatGrandParentOrganisationCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkCreated.Build(),
            childBecameDaughterOfParent.Build(),
            parentBecameDaughterOfGrandParent.Build(),
            grandParentBecameDaughterOfGrandParent.Build()
        };
    }

    protected override UpdateOrganisationFormalFramework When()
        => new(
            _grandParentBecameDaughterOfGrandParentId,
            new FormalFrameworkId(_formalFrameworkId),
            new OrganisationId(_grandParentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsADomainException()
    {
        Exception.Should().BeOfType<CircularRelationInFormalFramework>();
    }
}
