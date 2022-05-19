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
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingANonVlimpersOrganisationAsParentForAVlimpersOrganisation
    : ExceptionSpecification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly Guid _organisationId = Guid.NewGuid();
    private readonly Guid _organisationOrganisationParentId = Guid.NewGuid();
    private DateTime _validTo;
    private DateTime _validFrom;
    private readonly DateTimeProviderStub _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);
    private readonly Guid _organisationParentId = Guid.NewGuid();

    public WhenAddingANonVlimpersOrganisationAsParentForAVlimpersOrganisation(ITestOutputHelper helper) : base(
        helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub
        );

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.VlimpersBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _validFrom = _dateTimeProviderStub.Today;
        _validTo = _dateTimeProviderStub.Today.AddDays(2);

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
            new OrganisationPlacedUnderVlimpersManagement(_organisationId)
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
        => 0;

    [Fact]
    public void AddsAnOrganisationParent()
    {
        Exception.Should().BeOfType<VlimpersAndNonVlimpersOrganisationCannotBeInParentalRelationship>();
    }
}
