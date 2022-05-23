namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationFormalFramework;

using System;
using System.Threading.Tasks;
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
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationFormalFrameworkWithCircularDependencies : Specification<
    AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private readonly Guid _formalFrameworkId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _childOrganisationId;
    private readonly Guid _formalFrameworkCategoryId;
    private readonly string _formalFrameworkCategoryName;

    public WhenAddingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(helper)
    {
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
        _formalFrameworkId = Guid.NewGuid();
        _parentOrganisationId = Guid.NewGuid();
        _childOrganisationId = Guid.NewGuid();
        _formalFrameworkCategoryId = Guid.NewGuid();
        _formalFrameworkCategoryName = "Category1";
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProviderStub(DateTime.Now),
            Mock.Of<IOrganisationRegistryConfiguration>());

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_childOrganisationId))
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
                .Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _childOrganisationId,
                    _formalFrameworkId,
                    _parentOrganisationId)
                .Build()
        };

    private AddOrganisationFormalFramework AddOrganisationFormalFrameworkCommand
        => new(
            Guid.NewGuid(),
            new FormalFrameworkId(_formalFrameworkId),
            new OrganisationId(_parentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, UserBuilder.AlgemeenBeheerder())
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsADomainException()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, UserBuilder.AlgemeenBeheerder())
            .ThenThrows<CircularRelationInFormalFramework>();
    }
}
