namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenOtherParentsAreNowActive : Specification<UpdateOrganisationFormalFrameworkParentsCommandHandler,
    UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
    private readonly DateTimeProviderStub _dateTimeProvider;
    private Guid _childOrganisationCreatedId;
    private Guid _parentOrganisationCreatedId;
    private Guid _anotherParentOrganisationCreatedId;
    private Guid _formalFrameworkCreatedId;

    public WhenOtherParentsAreNowActive(ITestOutputHelper helper) : base(helper)
    {
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
    }

    private IEvent[] Events()
    {
        var childOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _childOrganisationCreatedId = childOrganisationCreated.Id;

        var parentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _parentOrganisationCreatedId = parentOrganisationCreated.Id;

        var anotherParentOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        _anotherParentOrganisationCreatedId = anotherParentOrganisationCreated.Id;

        var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
        var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreated.Id,
            formalFrameworkCategoryCreated.Name);
        _formalFrameworkCreatedId = formalFrameworkCreated.Id;

        var anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(
            formalFrameworkCategoryCreated.Id,
            formalFrameworkCategoryCreated.Name);

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

        return new IEvent[]
        {
            childOrganisationCreated.Build(), parentOrganisationCreated.Build(),
            anotherParentOrganisationCreated.Build(), formalFrameworkCategoryCreated.Build(),
            formalFrameworkCreated.Build(), anotherFormalFrameworkCreated.Build(),
            organisationFormalFrameworkBAdded.Build(), anotherOrganisationFormalFrameworkBAdded.Build(),
            organisationFormalFrameworkAdded.Build(), newOrganisationFormalFrameworkAdded.Build(),
            formalFrameworkAssignedToOrganisation.Build(), anotherFormalFrameworkAssignedToOrganisation.Build()
        };
    }

    private UpdateOrganisationFormalFrameworkParents UpdateOrganisationFormalFrameworkParentsCommand
        => new(new OrganisationId(_childOrganisationCreatedId), new FormalFrameworkId(_formalFrameworkCreatedId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            session,
            _dateTimeProvider);

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events()).When(
                UpdateOrganisationFormalFrameworkParentsCommand,
                new UserBuilder()
                    .Build())
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task PublishesFormalFrameworkClearedFromOrganisation()
    {
        await Given(Events()).When(
            UpdateOrganisationFormalFrameworkParentsCommand,
            new UserBuilder()
                .Build()).Then();
        var @event = PublishedEvents[0];
        @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
        var frameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        frameworkClearedFromOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        frameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationCreatedId);
        frameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
    }

    [Fact]
    public async Task PublishesAnotherFormalFrameworkAssignedToOrganisation()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkParentsCommand, new UserBuilder().Build()).Then();
        var @event = PublishedEvents[1];
        @event.Should().BeOfType<Envelope<FormalFrameworkAssignedToOrganisation>>();
        var formalFrameworkAssignedToOrganisation = @event.UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        formalFrameworkAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationCreatedId);
        formalFrameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_anotherParentOrganisationCreatedId);
        formalFrameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
    }
}
