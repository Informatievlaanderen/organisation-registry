namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAVlimpersOrganisationParentAndUserIsVlimpers
    : OldSpecification2<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private Guid _organisationId;
    private Guid _organisationOrganisationParentId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private Guid _organisationParentId;

    public WhenAddingAVlimpersOrganisationParentAndUserIsVlimpers(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationOrganisationParentId = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();

        return new List<IEvent>
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                "OVO000012345",
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
            new OrganisationCreated(
                _organisationParentId,
                "Ouder en Gezin",
                "OVO000012346",
                "O&G",
                Article.None,
                "Moeder",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
            new OrganisationPlacedUnderVlimpersManagement(_organisationParentId),
        };
    }

    protected override AddOrganisationParent When()
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

        organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId);
        organisationParentAdded.OrganisationId.Should().Be(_organisationId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentAdded.ValidFrom.Should().Be(_validFrom);
        organisationParentAdded.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public void AssignsAParent()
    {
        var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<ParentAssignedToOrganisation>();
        parentAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
        parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_organisationParentId);
    }
}
