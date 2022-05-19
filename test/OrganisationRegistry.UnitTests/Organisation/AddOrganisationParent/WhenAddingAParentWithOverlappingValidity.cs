namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
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
    : ExceptionSpecification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator = new();

    private string _childOvoNumber = null!;
    private Guid _childId;
    private Guid _parentId;

    public WhenAddingAParentWithOverlappingValidity(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            new DateTimeProvider()
        );

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(_childOvoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);

        var childCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();
        var parentCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();

        _childOvoNumber = childCreated.OvoNumber;
        _childId = childCreated.OrganisationId;
        _parentId = parentCreated.OrganisationId;

        return new List<IEvent>
        {
            childCreated,
            parentCreated,
            new OrganisationParentAdded(
                childCreated.OrganisationId,
                Guid.NewGuid(),
                parentCreated.OrganisationId,
                "Ouder en Gezin",
                null,
                null)
        };
    }

    protected override AddOrganisationParent When()
        => new(
            Guid.NewGuid(),
            new OrganisationId(_childId),
            new OrganisationId(_parentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsAnException()
    {
        Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
        Exception?.Message.Should()
            .Be("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.");
    }
}
