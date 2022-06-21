namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationFormalFramework : Specification<AddOrganisationFormalFrameworkCommandHandler,
    AddOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private readonly Guid _formalFramworkId;
    private readonly Guid _childOrganisationId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _formalFramworkCategoryId;
    private readonly Guid _organisationFormalFramworkId;
    private readonly ValidFrom _validFrom;
    private readonly ValidTo _validTo;
    private readonly string _parentOrganisationName;
    private readonly string _formalFramworkName;

    public WhenAddingAnOrganisationFormalFramework(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
        _formalFramworkId = Guid.NewGuid();
        _childOrganisationId = Guid.NewGuid();
        _parentOrganisationId = Guid.NewGuid();
        _formalFramworkCategoryId = Guid.NewGuid();
        _organisationFormalFramworkId = Guid.NewGuid();
        _validFrom = new ValidFrom();
        _validTo = new ValidTo();
        _parentOrganisationName = "Parent Organisation";
        _formalFramworkName = "Formal Framework";
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Now),
            Mock.Of<IOrganisationRegistryConfiguration>()
        );

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_childOrganisationId))
                .Build(),
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_parentOrganisationId))
                .WithName(_parentOrganisationName)
                .Build(),
            new FormalFrameworkCategoryCreatedBuilder()
                .WithId(_formalFramworkCategoryId)
                .WithName("Category1")
                .Build(),
            new FormalFrameworkCreatedBuilder(
                    _formalFramworkCategoryId,
                    "Category1")
                .WithId(new FormalFrameworkId(_formalFramworkId))
                .WithName(_formalFramworkName)
                .Build(),
        };


    private AddOrganisationFormalFramework AddOrganisationFormalFrameworkCommand
        => new(
            _organisationFormalFramworkId,
            new FormalFrameworkId(_formalFramworkId),
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationId),
            _validFrom,
            _validTo
        );

    [Fact]
    public async Task PublishesTwoEvents()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationFormalFrameworkAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationFormalFrameworkAdded(
                    _childOrganisationId,
                    _organisationFormalFramworkId,
                    _formalFramworkId,
                    _formalFramworkName,
                    _parentOrganisationId,
                    _parentOrganisationName,
                    _validFrom,
                    _validTo
                ),
                opt => opt.ExcludeEventProperties()
            );
    }

    [Fact]
    public async Task AssignsAParent()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .Then();

        PublishedEvents[1]
            .UnwrapBody<FormalFrameworkAssignedToOrganisation>()
            .Should()
            .BeEquivalentTo(
                new FormalFrameworkAssignedToOrganisation(
                    _childOrganisationId,
                    _formalFramworkId,
                    _parentOrganisationId,
                    _organisationFormalFramworkId),
                opt => opt.ExcludeEventProperties()
            );
    }
}
