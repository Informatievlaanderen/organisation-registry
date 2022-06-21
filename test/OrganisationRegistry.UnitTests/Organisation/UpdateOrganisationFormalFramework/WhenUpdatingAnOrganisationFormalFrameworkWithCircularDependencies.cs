namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework;

using System;
using System.Threading.Tasks;
using FormalFramework;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies : Specification<
    UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private readonly Guid _grandParentBecameDaughterOfGrandParentId;
    private readonly Guid _formalFrameworkId;
    private readonly Guid _grandParentOrganisationId;
    private readonly Guid _childOrganisationId;
    private readonly Guid _categoryId;
    private readonly string _categoryName;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _greatGrandparentOrganisationId;

    public WhenUpdatingAnOrganisationFormalFrameworkWithCircularDependencies(ITestOutputHelper helper) : base(
        helper)
    {
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();

        _categoryId = Guid.NewGuid();
        _categoryName = "Formal Framework Category";
        _grandParentBecameDaughterOfGrandParentId =
            Guid.NewGuid();
        _formalFrameworkId = Guid.NewGuid();
        _greatGrandparentOrganisationId = Guid.NewGuid();
        _grandParentOrganisationId = Guid.NewGuid();
        _parentOrganisationId = Guid.NewGuid();
        _childOrganisationId = Guid.NewGuid();
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub());

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_childOrganisationId)).Build(),
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_parentOrganisationId)).Build(),
            new OrganisationCreatedBuilder(_ovoNumberGenerator)
                .WithId(new OrganisationId(_grandParentOrganisationId)).Build(),
            new OrganisationCreatedBuilder(_ovoNumberGenerator).Build(), new FormalFrameworkCategoryCreatedBuilder()
                .WithId(_categoryId)
                .WithName(_categoryName)
                .Build(),
            new FormalFrameworkCreatedBuilder(
                    _categoryId,
                    _categoryName)
                .WithId(new FormalFrameworkId(_formalFrameworkId))
                .Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                _childOrganisationId,
                _formalFrameworkId,
                _parentOrganisationId).Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                _parentOrganisationId,
                _formalFrameworkId,
                _grandParentOrganisationId).Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _grandParentOrganisationId,
                    _formalFrameworkId,
                    _greatGrandparentOrganisationId)
                .WithId(_grandParentBecameDaughterOfGrandParentId)
                .Build(),
        };

    private UpdateOrganisationFormalFramework UpdateOrganisationFormalFrameworkCommand
        => new(
            _grandParentBecameDaughterOfGrandParentId,
            new FormalFrameworkId(_formalFrameworkId),
            new OrganisationId(_grandParentOrganisationId),
            new OrganisationId(_childOrganisationId),
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsADomainException()
    {
        await Given(Events).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<CircularRelationInFormalFramework>();
    }
}
