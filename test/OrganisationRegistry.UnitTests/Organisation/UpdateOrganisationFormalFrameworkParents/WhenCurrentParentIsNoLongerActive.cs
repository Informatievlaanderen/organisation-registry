namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents;

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
using Xunit;
using Xunit.Abstractions;

public class WhenCurrentParentIsNoLongerActive : OldSpecification2<UpdateOrganisationFormalFrameworkParentsCommandHandler, UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new();
    private readonly DateTimeProviderStub _dateTimeProvider = new(DateTime.Now);

    private Guid _organisationCreatedId;
    private Guid _formalFrameworkCreatedId;
    private Guid _parentOrganisationCreatedId;

    public WhenCurrentParentIsNoLongerActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder()
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _organisationCreatedId = organisationCreated.Id;

        var parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _parentOrganisationCreatedId = parentOrganisationCreated.Id;

        var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
        _formalFrameworkCreatedId = formalFrameworkCreated.Id;

        var anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

        var organisationFormalFrameworkAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    organisationCreated.Id,
                    formalFrameworkCreated.Id,
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

        var anotherOrganisationFormalFrameworkAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    organisationCreated.Id,
                    anotherFormalFrameworkCreated.Id,
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

        var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
            organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
            organisationFormalFrameworkAdded.FormalFrameworkId,
            organisationFormalFrameworkAdded.OrganisationId,
            organisationFormalFrameworkAdded.ParentOrganisationId);

        var anotherFormalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
            anotherOrganisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
            anotherOrganisationFormalFrameworkAdded.FormalFrameworkId,
            anotherOrganisationFormalFrameworkAdded.OrganisationId,
            anotherOrganisationFormalFrameworkAdded.ParentOrganisationId);

        return new List<IEvent>
        {
            organisationCreated.Build(),
            parentOrganisationCreated.Build(),
            formalFrameworkCategoryCreated.Build(),
            formalFrameworkCreated.Build(),
            anotherFormalFrameworkCreated.Build(),
            organisationFormalFrameworkAdded.Build(),
            anotherOrganisationFormalFrameworkAdded.Build(),
            formalFrameworkAssignedToOrganisation.Build(),
            anotherFormalFrameworkAssignedToOrganisation.Build()
        };
    }

    protected override UpdateOrganisationFormalFrameworkParents When()
        => new(new OrganisationId(_organisationCreatedId), new FormalFrameworkId(_formalFrameworkCreatedId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler()
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            Session,
            _dateTimeProvider);

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void PublishesFormalFrameworkClearedFromOrganisation()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
        var formalFrameworkClearedFromOrganisation = PublishedEvents[0].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be(_organisationCreatedId);
        formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
        formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationCreatedId);
    }
}
