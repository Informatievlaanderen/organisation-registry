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
using OrganisationRegistry.Organisation.Exceptions;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAParentWithOverlappingValidity
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTime _validTo;
    private readonly DateTime _validFrom;

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator;

    private readonly string _childOvoNumber;
    private readonly Guid _childId;
    private readonly Guid _parentId;

    public WhenAddingAParentWithOverlappingValidity(ITestOutputHelper helper) : base(helper)
    {
        var dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
        _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        _validFrom = dateTimeProviderStub.Today;
        _validTo = dateTimeProviderStub.Today.AddDays(2);

        _childOvoNumber = _sequentialOvoNumberGenerator.GenerateNumber();
        _childId = Guid.NewGuid();
        _parentId = Guid.NewGuid();
    }

    protected override AddOrganisationParentCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            session,
            new DateTimeProvider()
        );

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
                .Build(),
            new OrganisationParentAdded(
                _childId,
                Guid.NewGuid(),
                _parentId,
                "Ouder en Gezin",
                null,
                null)
        };

    private AddOrganisationParent AddOrganisationParentCommand
        => new(
            Guid.NewGuid(),
            new OrganisationId(_childId),
            new OrganisationId(_parentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    [Fact]
    public async Task PublishesNoEvents()
    {
        await Given(Events).When(AddOrganisationParentCommand, User).ThenItPublishesTheCorrectNumberOfEvents(0);
    }

    [Fact]
    public async Task ThrowsAnException()
    {
        await Given(Events).When(AddOrganisationParentCommand, User)
            .ThenThrows<OrganisationAlreadyCoupledToParentInThisPeriod>()
            .WithMessage("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.");
    }
}
