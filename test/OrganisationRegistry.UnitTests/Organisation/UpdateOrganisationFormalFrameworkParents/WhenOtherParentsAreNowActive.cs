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

public class WhenOtherParentsAreNowActive : Specification<UpdateOrganisationFormalFrameworkParentsCommandHandler, UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new();
    private readonly DateTimeProviderStub _dateTimeProvider = new(DateTime.Now);
    private Guid _childOrganisationCreatedId;
    private Guid _parentOrganisationCreatedId;
    private Guid _anotherParentOrganisationCreatedId;
    private Guid _formalFrameworkCreatedId;

    public WhenOtherParentsAreNowActive(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder()
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _childOrganisationCreatedId = childOrganisationCreated.Id;

        var parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _parentOrganisationCreatedId = parentOrganisationCreated.Id;

        var anotherParentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _anotherParentOrganisationCreatedId = anotherParentOrganisationCreated.Id;

        var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
        _formalFrameworkCreatedId = formalFrameworkCreated.Id;

        var anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

        var organisationFormalFrameworkAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    childOrganisationCreated.Id,
                    formalFrameworkCreated.Id,
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

        var newOrganisationFormalFrameworkAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    childOrganisationCreated.Id,
                    formalFrameworkCreated.Id,
                    anotherParentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

        var organisationFormalFrameworkBAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    childOrganisationCreated.Id,
                    anotherFormalFrameworkCreated.Id,
                    parentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

        var anotherOrganisationFormalFrameworkBAdded =
            new OrganisationFormalFrameworkAddedBuilder(
                    childOrganisationCreated.Id,
                    anotherFormalFrameworkCreated.Id,
                    anotherParentOrganisationCreated.Id)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

        var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
            organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
            organisationFormalFrameworkAdded.FormalFrameworkId,
            organisationFormalFrameworkAdded.OrganisationId,
            organisationFormalFrameworkAdded.ParentOrganisationId);

        var anotherFormalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
            anotherOrganisationFormalFrameworkBAdded.OrganisationFormalFrameworkId,
            anotherOrganisationFormalFrameworkBAdded.FormalFrameworkId,
            anotherOrganisationFormalFrameworkBAdded.OrganisationId,
            anotherOrganisationFormalFrameworkBAdded.ParentOrganisationId);

        return new List<IEvent>
        {
            childOrganisationCreated.Build(),
            parentOrganisationCreated.Build(),
            anotherParentOrganisationCreated.Build(),
            formalFrameworkCategoryCreated.Build(),
            formalFrameworkCreated.Build(),
            anotherFormalFrameworkCreated.Build(),
            organisationFormalFrameworkBAdded.Build(),
            anotherOrganisationFormalFrameworkBAdded.Build(),
            organisationFormalFrameworkAdded.Build(),
            newOrganisationFormalFrameworkAdded.Build(),
            formalFrameworkAssignedToOrganisation.Build(),
            anotherFormalFrameworkAssignedToOrganisation.Build()
        };
    }

    protected override UpdateOrganisationFormalFrameworkParents When()
        => new(new OrganisationId(_childOrganisationCreatedId), new FormalFrameworkId(_formalFrameworkCreatedId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler()
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            Session,
            _dateTimeProvider);

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void PublishesFormalFrameworkClearedFromOrganisation()
    {
        var @event = PublishedEvents[0];
        @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
        var frameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        frameworkClearedFromOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        frameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationCreatedId);
        frameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
    }

    [Fact]
    public void PublishesAnotherFormalFrameworkAssignedToOrganisation()
    {
        var @event = PublishedEvents[1];
        @event.Should().BeOfType<Envelope<FormalFrameworkAssignedToOrganisation>>();
        var formalFrameworkAssignedToOrganisation = @event.UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        formalFrameworkAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        formalFrameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_anotherParentOrganisationCreatedId);
        formalFrameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
    }
}
