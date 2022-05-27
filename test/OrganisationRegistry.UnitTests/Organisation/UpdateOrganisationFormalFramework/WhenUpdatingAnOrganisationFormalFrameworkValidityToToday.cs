namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFramework;

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
using Tests.Shared.Stubs;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingAnOrganisationFormalFrameworkValidityToToday :
    Specification<UpdateOrganisationFormalFrameworkCommandHandler, UpdateOrganisationFormalFramework>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _ovoNumberGenerator;

    private Guid _organisationFormalFrameworkId;
    private Guid _formalFrameworkBId;
    private Guid _childOrganisationId;
    private Guid _parentOrganisationBId;

    public WhenUpdatingAnOrganisationFormalFrameworkValidityToToday(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _ovoNumberGenerator = new SequentialOvoNumberGenerator();
    }

    protected override UpdateOrganisationFormalFrameworkCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<UpdateOrganisationFormalFrameworkCommandHandler>>().Object,
            session,
            new DateTimeProvider(),
            new OrganisationRegistryConfigurationStub());

    private IEvent[] Events()
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

        return new IEvent[]
        {
            childOrganisationCreated.Build(), parentOrganisationACreated.Build(), parentOrganisationBCreated.Build(),
            formalFrameworkCategoryCreatedBuilder.Build(), formalFrameworkACreated.Build(),
            formalFrameworkBCreated.Build(), childBecameDaughterOfParent.Build()
        };
    }

    private UpdateOrganisationFormalFramework UpdateOrganisationFormalFrameworkCommand
        => new(
            _organisationFormalFrameworkId,
            new FormalFrameworkId(_formalFrameworkBId),
            new OrganisationId(_childOrganisationId),
            new OrganisationId(_parentOrganisationBId),
            new ValidFrom(_dateTimeProviderStub.Today),
            new ValidTo(_dateTimeProviderStub.Today));

    [Fact]
    public async Task Publishes2Events()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task UpdatesTheOrganisationFormalFramework()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationFormalFrameworkUpdated>>();

        var organisationFormalFrameworkUpdated =
            PublishedEvents[0].UnwrapBody<OrganisationFormalFrameworkUpdated>();
        organisationFormalFrameworkUpdated.OrganisationId.Should().Be(_childOrganisationId);
        organisationFormalFrameworkUpdated.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        organisationFormalFrameworkUpdated.ValidFrom.Should().Be(_dateTimeProviderStub.Today);
        organisationFormalFrameworkUpdated.ValidTo.Should().Be(_dateTimeProviderStub.Today);
    }

    [Fact]
    public async Task AssignsAParent()
    {
        await Given(Events()).When(UpdateOrganisationFormalFrameworkCommand, TestUser.AlgemeenBeheerder).Then();
        var frameworkAssignedToOrganisation =
            PublishedEvents[1].UnwrapBody<FormalFrameworkAssignedToOrganisation>();
        frameworkAssignedToOrganisation.OrganisationId.Should().Be(_childOrganisationId);
        frameworkAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationBId);
        frameworkAssignedToOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkBId);
    }
}
