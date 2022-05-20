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
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationFormalFrameworkWithCircularDependencies : ExceptionOldSpecification2<
    AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();
    private Guid _formalFrameworkId;
    private Guid _parentOrganisationId;
    private Guid _childOrganisationId;

    public WhenAddingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub,
            Mock.Of<IOrganisationRegistryConfiguration>());

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var parentOrganisationCreated = new OrganisationCreatedBuilder(_ovoNumberGenerator);
        var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreated.Id,
            formalFrameworkCategoryCreated.Name);
        var childBecameDaughterOfParent =
            new OrganisationFormalFrameworkAddedBuilder(
                childOrganisationCreated.Id,
                formalFrameworkCreated.Id,
                parentOrganisationCreated.Id);

        _formalFrameworkId = formalFrameworkCreated.Id;
        _parentOrganisationId = parentOrganisationCreated.Id;
        _childOrganisationId = childOrganisationCreated.Id;

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationCreated.Build(),
            formalFrameworkCategoryCreated.Build(),
            formalFrameworkCreated.Build(),
            childBecameDaughterOfParent.Build()
        };
    }

    protected override AddOrganisationFormalFramework When()
        => new(
            Guid.NewGuid(),
            new FormalFrameworkId(_formalFrameworkId),
            new OrganisationId(_parentOrganisationId),
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
