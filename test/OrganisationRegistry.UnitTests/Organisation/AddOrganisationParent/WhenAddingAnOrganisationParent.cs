namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Domain;
using Tests.Shared;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationParent
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator;

    private readonly Guid _organisationOrganisationParentId;
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;
    private readonly string _childOvoNumber;
    private readonly Guid _childId;
    private readonly Guid _parentId;
    private readonly string _parentOrganisationName;


    public WhenAddingAnOrganisationParent(ITestOutputHelper helper) : base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        _organisationOrganisationParentId = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _childOvoNumber = _sequentialOvoNumberGenerator.GenerateNumber();
        _childId = Guid.NewGuid();
        _parentId = Guid.NewGuid();
        _parentOrganisationName = "Parent organisation";
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_childOvoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_childId))
                .WithOvoNumber(_childOvoNumber)
                .Build(),
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_parentId))
                .WithName(_parentOrganisationName)
                .Build()
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_childId),
            new OrganisationId(_parentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesTwoEvents()
        => await Given(Events)
            .When(AddOrganisationParentCommand, User)
            .ThenItPublishesTheCorrectNumberOfEvents(2);


    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationParentAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationParentAdded(
                    _childId,
                    _organisationOrganisationParentId,
                    _parentId,
                    _parentOrganisationName,
                    _validFrom,
                    _validTo
                ),
                opt => opt.ExcludeEventProperties());
    }

    [Fact]
    public async Task AssignsAParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).Then();

        PublishedEvents[1]
            .UnwrapBody<ParentAssignedToOrganisation>()
            .Should()
            .BeEquivalentTo(
                new ParentAssignedToOrganisation(
                    _childId,
                    _parentId,
                    _organisationOrganisationParentId),opt=>opt.ExcludeEventProperties());
    }
}
