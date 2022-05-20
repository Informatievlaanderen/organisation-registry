namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent;

using System;
using System.Collections.Generic;
using System.Linq;
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

public class WhenUpdatingAnOrganisationParentToValid
    : OldSpecification2<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private Guid _organisationId;
    private Guid _organisationParentId;
    private Guid _organisationOrganisationParentId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
    private const string OvoNumber = "OVO000012345";

    public WhenUpdatingAnOrganisationParentToValid(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
            Session,
            new DateTimeProvider());

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(OvoNumber)
            .AddRoles(Role.DecentraalBeheerder)
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
                _organisationOrganisationParentId,
                _organisationParentId,
                "Ouder en Gezin",
                _dateTimeProviderStub.Today.AddYears(-1),
                _dateTimeProviderStub.Today.AddYears(-1))
        };
    }

    protected override UpdateOrganisationParent When()
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom),
            new ValidTo(_validTo));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationBuilding()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

        var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
        organisationParentUpdated.OrganisationId.Should().Be(_organisationId);
        organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentUpdated.ValidFrom.Should().Be(_validFrom);
        organisationParentUpdated.ValidTo.Should().Be(_validTo);
    }

    [Fact]
    public void AssignsAParent()
    {
        var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<ParentAssignedToOrganisation>();
        parentAssignedToOrganisation.OrganisationId.Should().Be(_organisationId);
        parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_organisationParentId);
    }
}
