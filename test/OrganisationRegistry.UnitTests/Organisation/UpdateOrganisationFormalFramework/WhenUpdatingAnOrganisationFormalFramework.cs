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

public class WhenUpdatingAnOrganisationFormalFramework : Specification<
    UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();
    private Guid _organisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _childOrganisationId;
    private Guid _parentOrganisationBId;
    private Guid _parentOrganisationAId;

    private DateTime? Tomorrow
        => _dateTimeProviderStub.Today.AddDays(1);

    public WhenUpdatingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            Session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub()
        );

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

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationACreated.Build(),
            parentOrganisationBCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(),
            formalFrameworkACreated.Build(),
            formalFrameworkBCreated.Build(),
            childBecameDaughterOfParent.Build()
        };
    }

    protected override UpdateOrganisationFormalFramework When()
        => new(
            _organisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkBId),
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationBId),
            new ValidFrom(Tomorrow),
            new ValidTo(Tomorrow));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void UpdatesTheOrganisationBuilding()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents.First().UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.PreviousParentOrganisationId.Should()
            .Be(_parentOrganisationAId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        organisationFormalFrameworkUpdated.FormalFrameworkId.Should().Be(_formalFrameworkBId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(Tomorrow);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(Tomorrow);
    }
}
