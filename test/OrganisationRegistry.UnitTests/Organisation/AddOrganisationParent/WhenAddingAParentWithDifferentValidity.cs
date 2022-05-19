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
using Xunit;
using Xunit.Abstractions;

public class
    WhenAddingAParentWithDifferentValidity
    : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private Guid _organisationId;
    private Guid _organisationOrganisationParentId1;
    private Guid _organisationOrganisationParentId2;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new (DateTime.Now);
    private Guid _organisationParentId;

    private const string OvoNumber = "OVO000012345";

    public WhenAddingAParentWithDifferentValidity(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(OvoNumber)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _organisationOrganisationParentId1 = Guid.NewGuid();
        _organisationOrganisationParentId2 = Guid.NewGuid();
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);
        _organisationId = Guid.NewGuid();
        _organisationParentId = Guid.NewGuid();

        return new List<IEvent>
        {
            new OrganisationCreated(
                _organisationId,
                "Kind en Gezin",
                OvoNumber,
                "K&G",
                Article.None,
                "Kindjes en gezinnetjes",
                new List<Purpose>(),
                false,
                null,
                null,
                null,
                null),
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
            new OrganisationParentAdded(
                _organisationId,
                _organisationOrganisationParentId1,
                _organisationParentId,
                "Ouder en Gezin",
                _validFrom,
                _validTo),
            new ParentAssignedToOrganisation(
                _organisationId,
                _organisationParentId,
                _organisationOrganisationParentId1)
        };
    }

    protected override AddOrganisationParent When()
        => new(
            _organisationOrganisationParentId2,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom.AddYears(1)),
            new ValidTo(_validTo.AddYears(1)));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

        organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId2);
        organisationParentAdded.OrganisationId.Should().Be(_organisationId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentAdded.ValidFrom.Should().Be(_validFrom.AddYears(1));
        organisationParentAdded.ValidTo.Should().Be(_validTo.AddYears(1));
    }
}
