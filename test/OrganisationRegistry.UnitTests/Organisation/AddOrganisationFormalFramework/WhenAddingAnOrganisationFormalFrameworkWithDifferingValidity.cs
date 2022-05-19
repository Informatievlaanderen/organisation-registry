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

public class WhenAddingAnOrganisationFormalFrameworkWithDifferingValidity : Specification<
    AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

    private Guid _formalFrameworkId;
    private Guid _parentOrganisationId;
    private Guid _childOrganisationId;

    public WhenAddingAnOrganisationFormalFrameworkWithDifferingValidity(ITestOutputHelper helper) : base(helper)
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
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProviderStub.Today, _dateTimeProviderStub.Today);

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
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationId),
            new ValidFrom(_dateTimeProviderStub.Today.AddDays(1)),
            new ValidTo(_dateTimeProviderStub.Today.AddDays(1)));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkAdded>();

        organisationParentAdded.FormalFrameworkId.Should().Be(_formalFrameworkId);
        organisationParentAdded.OrganisationId.Should().Be(_childOrganisationId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_parentOrganisationId);
        organisationParentAdded.ValidFrom.Should().Be(_dateTimeProviderStub.Today.AddDays(1));
        organisationParentAdded.ValidTo.Should().Be(_dateTimeProviderStub.Today.AddDays(1));
    }
}
