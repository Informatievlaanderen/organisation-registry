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

public class WhenAddingAnOrganisationFormalFrameworkWithOverlappingValidity : Specification<
    AddOrganisationFormalFrameworkCommandHandler, AddOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;
    private readonly Guid _formalFrameworkId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _childOrganisationId;
    private readonly Guid _formalFrameworkCategoryId;
    private readonly string _formalFrameworkCategoryName;
    private readonly Guid _organisationFormalFrameworkId;
    private readonly string _formalFramworkName;
    private readonly string _parentOrganisationName;

    public WhenAddingAnOrganisationFormalFrameworkWithOverlappingValidity(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
        _formalFrameworkId = Guid.NewGuid();
        _formalFramworkName = "Formal Framework";
        _parentOrganisationId = Guid.NewGuid();
        _parentOrganisationName = "Parent Organisation";
        _childOrganisationId = Guid.NewGuid();
        _formalFrameworkCategoryId = Guid.NewGuid();
        _formalFrameworkCategoryName = "Category1";
        _organisationFormalFrameworkId = Guid.NewGuid();
    }

    protected override AddOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            _dateTimeProviderStub,
            Mock.Of<IOrganisationRegistryConfiguration>());

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
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationId),
            new ValidFrom(),
            new ValidTo());


    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsADomainException()
    {
        await Given(Events)
            .When(AddOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenThrows<OrganisationAlreadyCoupledToFormalFrameworkParentInThisPeriod>();
    }
}
