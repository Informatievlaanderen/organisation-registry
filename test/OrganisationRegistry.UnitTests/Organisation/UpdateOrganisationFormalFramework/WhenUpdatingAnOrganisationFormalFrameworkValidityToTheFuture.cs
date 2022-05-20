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

public class WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture : OldSpecification2<
    UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

    private DateTime? Tomorrow
        => _dateTimeProviderStub.Today.AddDays(1);

    private Guid _childOrganisationCreatedId;
    private Guid _parentOrganisationBCreatedId;
    private Guid _formalFrameworkACreatedId;
    private Guid _childBecameDaughterOfParentOrganisationFormalFrameworkId;

    public WhenUpdatingAnOrganisationFormalFrameworkValidityToTheFuture(ITestOutputHelper helper) : base(helper)
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

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationACreated.Build(),
            parentOrganisationBCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkACreated.Build(),
            childBecameDaughterOfParent.Build(),
            formalFrameworkAssignedToOrg.Build()
        };
    }

    protected override UpdateOrganisationFormalFramework When()
        => new(
            _childBecameDaughterOfParentOrganisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkACreatedId),
            new OrganisationId(_childOrganisationCreatedId),
            new OrganisationId(_parentOrganisationBCreatedId),
            new ValidFrom(Tomorrow),
            new ValidTo(Tomorrow));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationFormalFramework()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationCreatedId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBCreatedId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(Tomorrow);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(Tomorrow);
    }

    [Fact]
    public void ClearsAParent()
    {
        var formalFrameworkClearedFromOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationBCreatedId);
        formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkACreatedId);
    }
}
