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
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub;

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator;

    private readonly string _ovoNumberA;
    private readonly Guid _organisationAId;
    private readonly Guid _organisationBId;
    private Guid _organisationOrganisationParentId;
    private string _parentOrganisationName;
    private DateTime _validFrom;
    private DateTime _validTo;

    public WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity(ITestOutputHelper helper) :
        base(helper)
    {
        _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2016, 6, 1));
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();


        _ovoNumberA = _sequentialOvoNumberGenerator.GenerateNumber();
        _organisationAId = Guid.NewGuid();
        _organisationBId = Guid.NewGuid();
        _organisationOrganisationParentId = Guid.NewGuid();
        _parentOrganisationName = "Parent organisation";
        _validFrom = new DateTime(2017, 1, 1);
        _validTo = new DateTime(2017, 12, 31);
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            _dateTimeProviderStub);

    private IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumberA)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationAId))
                .WithOvoNumber(_ovoNumberA)
                .Build(),
            new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator)
                .WithId(new OrganisationId(_organisationBId))
                .WithName(_parentOrganisationName)
                .Build(),
            new OrganisationParentAddedBuilder(_organisationAId, _organisationBId)
                .WithValidity(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31)).Build(),
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationAId),
            new OrganisationId(_organisationBId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));


    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task AddsAnOrganisationParent()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).Then();

        PublishedEvents[0]
            .UnwrapBody<OrganisationParentAdded>()
            .Should()
            .BeEquivalentTo(
                new OrganisationParentAdded(
                    _organisationAId,
                    _organisationOrganisationParentId,
                    _organisationBId,
                    _parentOrganisationName,
                    _validFrom,
                    _validTo
                ),
                opt => opt.ExcludeEventProperties());
    }
}
