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
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFrameworkValidityToToday : Specification<
    UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator = new();

    private Guid _organisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _childOrganisationId;
    private Guid _parentOrganisationBId;

    private DateTime Tomorrow
        => _dateTimeProviderStub.Today.AddDays(1);

    public WhenUpdatingAnOrganisationFormalFrameworkValidityToToday(ITestOutputHelper helper) : base(helper)
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
            new ValidFrom(_dateTimeProviderStub.Today),
            new ValidTo(_dateTimeProviderStub.Today));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationFormalFramework()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_dateTimeProviderStub.Today);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_dateTimeProviderStub.Today);
    }

    [Fact]
    public void AssignsAParent()
    {
        var frameworkAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        frameworkAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationId);
        frameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        frameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkBId);
    }
}
