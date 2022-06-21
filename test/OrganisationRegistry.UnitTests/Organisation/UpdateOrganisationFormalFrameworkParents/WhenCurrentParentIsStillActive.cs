namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents;

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
using Xunit;
using Xunit.Abstractions;

public class WhenCurrentParentIsStillActive :
    Specification<UpdateOrganisationFormalFrameworkParentsCommandHandler, UpdateOrganisationFormalFrameworkParents>
{
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;
    private readonly DateTimeProviderStub _dateTimeProvider;

    private readonly Guid _organisationId;
    private readonly Guid _formalFrameworkCategoryId;
    private readonly Guid _parentOrganisationId;
    private readonly string _formalFrameworkCategoryName;
    private readonly Guid _formalFrameworkId;
    private readonly Guid _organisationFormalFrameworkId;

    public WhenCurrentParentIsStillActive(ITestOutputHelper helper) : base(helper)
    {
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        _dateTimeProvider = new DateTimeProviderStub(DateTime.Now);
        _organisationId = Guid.NewGuid();
        _parentOrganisationId = Guid.NewGuid();
        _formalFrameworkCategoryId = Guid.NewGuid();
        _formalFrameworkCategoryName = "Formal framework";
        _formalFrameworkId = Guid.NewGuid();
        _organisationFormalFrameworkId = Guid.NewGuid();
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
                .WithName(_formalFrameworkCategoryName)
                .Build(),
            new FormalFrameworkCreatedBuilder(
                    _formalFrameworkCategoryId,
                    _formalFrameworkCategoryName)
                .WithId(new FormalFrameworkId(_formalFrameworkId))
                .Build(),
            new OrganisationFormalFrameworkAddedBuilder(
                    _organisationId,
                    _formalFrameworkId,
                    _parentOrganisationId
                )
                .WithId(_organisationFormalFrameworkId)
                .WithValidity(_dateTimeProvider.Today, _dateTimeProvider.Today.AddDays(1))
                .Build(),
            new FormalFrameworkAssignedToOrganisationBuilder(
                _organisationFormalFrameworkId,
                _formalFrameworkId,
                _organisationId,
                _parentOrganisationId).Build(),
        };

    private UpdateOrganisationFormalFrameworkParents UpdateOrganisationFormalFrameworkParentsCommand
        => new(new OrganisationId(_organisationId), new FormalFrameworkId(_formalFrameworkId));

    protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler(ISession session)
        => new(
            Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
            session,
            _dateTimeProvider);

    [Fact]
    public async Task PublishesNotEvents()
    {
        await Given(Events).When(UpdateOrganisationFormalFrameworkParentsCommand, TestUser.User)
            .ThenItPublishesTheCorrectNumberOfEvents(0);
    }
}
