namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Configuration;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity :
    Specification<AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private readonly Guid _formalFrameworkId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _childOrganisationId;
    private readonly Guid _organisationFormalFrameworkId;
    private readonly Guid _formalFrameworkCategoryId;
    private readonly string _formalFrameworkCategoryName;
    private readonly DateTime _validFrom;
    private readonly DateTime _validTo;
    private readonly string _childOrganisationName;
    private readonly string _formalFramworkName;

    public WhenAddingAnOrganisationFormalFrameworkWithCircularDependenciesButNotInTheSameValidity(
        ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
        _formalFrameworkId = Guid.NewGuid();
        _formalFramworkName = "Formal Framework";
        _parentOrganisationId = Guid.NewGuid();
        _childOrganisationName = "Child Organisation";
        _childOrganisationId = Guid.NewGuid();
        _formalFrameworkCategoryId = Guid.NewGuid();
        _formalFrameworkCategoryName = "Category1";
        _validFrom = _dateTimeProviderStub.Today.AddDays(1);
        _organisationFormalFrameworkId = Guid.NewGuid();
        _validTo = _dateTimeProviderStub.Today.AddDays(1);
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            Mock.Of<IOrganisationRegistryConfiguration>());

    private static IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_childOrganisationId))
                .WithName(_childOrganisationName)
                .Build(),
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_parentOrganisationId))
                .Build(),
            new FormalFrameworkCategoryCreatedBuilder()
                .WithId(_formalFrameworkCategoryId)
                .WithName(_formalFrameworkCategoryName)
                .Build(),
            new FormalFrameworkCreatedBuilder(
                    _formalFrameworkCategoryId,
                    _formalFrameworkCategoryName)
                .WithId(new FormalFrameworkId(_formalFrameworkId))
                .WithName(_formalFramworkName)
                .Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _childOrganisationId,
                    _formalFrameworkId,
                    _parentOrganisationId)
                .WithValidity(_dateTimeProviderStub.Today, _dateTimeProviderStub.Today)
                .Build()
        };

    private AddOrganisationFormalFramework AddOrganisationFormalFrameworkCommand
        => new(
            _organisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkId),
            new OrganisationId(_parentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo)
        );

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AnOrganisationParentWasAdded()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, User)
            .Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationFormalFrameworkAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationFormalFrameworkAdded(
                    _parentOrganisationId,
                    _organisationFormalFrameworkId,
                    _formalFrameworkId,
                    _formalFramworkName,
                    _childOrganisationId,
                    _childOrganisationName,
                    _dateTimeProviderStub.Today.AddDays(1),
                    _dateTimeProviderStub.Today.AddDays(1)
                ),
                opt => opt.ExcludeEventProperties()
            );
    }
}
