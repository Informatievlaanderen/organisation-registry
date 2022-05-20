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

public class WhenUpdatingAnOrganisationParentValidityToTheFuture
    : OldSpecification2<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private Guid _organisationId;
    private Guid _organisationParentId;
    private Guid _organisationOrganisationParentId;
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
    private const string OvoNumber = "OVO000012345";

    public WhenUpdatingAnOrganisationParentValidityToTheFuture(ITestOutputHelper helper) : base(helper)
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
                null,
                null),
            new ParentAssignedToOrganisation(
                _organisationId,
                _organisationParentId,
                _organisationOrganisationParentId)
        };
    }

    protected override UpdateOrganisationParent When()
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(_validFrom.AddYears(1)),
            new ValidTo(_validTo.AddYears(1)));

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void UpdatesTheOrganisationBuilding()
    {
        PublishedEvents[0].Should().BeOfType<Envelope<OrganisationParentUpdated>>();

        var organisationParentUpdated = PublishedEvents.First().UnwrapBody<OrganisationParentUpdated>();
        organisationParentUpdated.OrganisationId.Should().Be(_organisationId);
        organisationParentUpdated.ParentOrganisationId.Should().Be(_organisationParentId);
        organisationParentUpdated.ValidFrom.Should().Be(_validFrom.AddYears(1));
        organisationParentUpdated.ValidTo.Should().Be(_validTo.AddYears(1));
    }

    [Fact]
    public void ClearsAParent()
    {
        var parentClearedFromOrganisation = PublishedEvents[1].UnwrapBody<ParentClearedFromOrganisation>();
        parentClearedFromOrganisation.OrganisationId.Should().Be(_organisationId);
        parentClearedFromOrganisation.ParentOrganisationId.Should().Be(_organisationParentId);
    }
}
