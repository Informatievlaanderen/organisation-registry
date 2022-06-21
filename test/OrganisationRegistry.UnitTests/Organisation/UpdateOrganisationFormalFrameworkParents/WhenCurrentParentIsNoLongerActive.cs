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

public class WhenCurrentParentIsNoLongerActive : Specification<UpdateOrganisationFormalFrameworkParentsCommandHandler,
    UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
    private readonly DateTimeProviderStub _dateTimeProvider;

    private readonly Guid _organisationId;
    private readonly Guid _parentOrganisationId;
    private readonly Guid _formalFrameworkCategoryId;
    private readonly string _formalFrameworkCategoryName;
    private readonly Guid _formalFrameworAId;
    private readonly Guid _formalFrameworkBId;
    private readonly Guid _organisationFormalFrameworkAId;
    private readonly Guid _organisationFormalFrameworkBId;

    public WhenCurrentParentIsNoLongerActive(ITestOutputHelper helper) : base(helper)
    {
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _parentOrganisationId = Guid.NewGuid();
        _formalFrameworkCategoryId = Guid.NewGuid();
        _formalFrameworkCategoryName = "Formal Framework Category";
        _formalFrameworAId = Guid.NewGuid();
        _formalFrameworkBId = Guid.NewGuid();
        _organisationFormalFrameworkAId = Guid.NewGuid();
        _organisationFormalFrameworkBId = Guid.NewGuid();
    }

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationId))
                .Build(),
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_parentOrganisationId))
                .Build(),
            new FormalFrameworkCategoryCreatedBuilder()
                .WithId(_formalFrameworkCategoryId)
                .WithName(_formalFrameworkCategoryName).Build(),
            new FormalFrameworkCreatedBuilder(
                    _formalFrameworkCategoryId,
                    _formalFrameworkCategoryName)
                .WithId(new FormalFrameworkId(_formalFrameworAId))
                .Build(),
            new FormalFrameworkCreatedBuilder(
                    _formalFrameworkCategoryId,
                    _formalFrameworkCategoryName)
                .WithId(new FormalFrameworkId(_formalFrameworkBId)).Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _organisationId,
                    _formalFrameworAId,
                    _parentOrganisationId)
                .WithId(_organisationFormalFrameworkAId)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1))
                .Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _organisationId,
                    _formalFrameworkBId,
                    _parentOrganisationId)
                .WithId(_organisationFormalFrameworkBId)
                .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1)).Build(),
            new FormalFrameworkAssignedToOrganisationBuilder(
                _organisationFormalFrameworkAId,
                _formalFrameworAId,
                _organisationId,
                _parentOrganisationId).Build(),
            new FormalFrameworkAssignedToOrganisationBuilder(
                _organisationFormalFrameworkBId,
                _formalFrameworkBId,
                _organisationId,
                _parentOrganisationId).Build(),
        };

    private UpdateOrganisationFormalFrameworkParents UpdateOrganisationFormalFrameworkParentsCommand
        => new(new OrganisationId(_organisationId), new FormalFrameworkId(_formalFrameworAId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            session,
            _dateTimeProvider);

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(UpdateOrganisationFormalFrameworkParentsCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task PublishesFormalFrameworkClearedFromOrganisation()
    {
        await Given(Events).When(UpdateOrganisationFormalFrameworkParentsCommand, TestUser.User).Then();
        PublishedEvents[0].Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
        var formalFrameworkClearedFromOrganisation =
            PublishedEvents[0].UnwrapBody<FormalFrameworkClearedFromOrganisation>();
        formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworAId);
        formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationId);
    }
}
