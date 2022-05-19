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
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationParent
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private Guid _organisationOrganisationParentId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator = new();

    private string _childOvoNumber = null!;
    private Guid _childId;
    private Guid _parentId;


    public WhenAddingAnOrganisationParent(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(_childOvoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationOrganisationParentId = Guid.NewGuid();
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
        };
    }

    protected override AddOrganisationParent When()
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_childId),
            new OrganisationId(_parentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

        organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId);
        organisationParentAdded.OrganisationId.Should().Be(_childId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_parentId);
        organisationParentAdded.ValidFrom.Should().Be(_validFrom);
        organisationParentAdded.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public void AssignsAParent()
    {
        var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<ParentAssignedToOrganisation>();
        parentAssignedToOrganisation.OrganisationId.Should().Be(_childId);
        parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentId);
    }
}
