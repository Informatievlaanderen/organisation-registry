namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity
    : ExceptionOldSpecification2<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(new DateTime(2016, 6, 1));

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator = new();

    private string _ovoNumberA = null!;
    private Guid _organisationAId;
    private Guid _organisationBId;

    public WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity(ITestOutputHelper helper) :
        base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumberA)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationABecameDaughterOfOrganisationBFor2016 =
            new OrganisationParentAddedBuilder(organisationACreated.Id, organisationBCreated.Id)
                .WithValidity(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31));

        _ovoNumberA = organisationACreated.OvoNumber;
        _organisationAId = organisationACreated.Id;
        _organisationBId = organisationBCreated.Id;

        return new List<IEvent>
        {
            organisationACreated.Build(),
            organisationBCreated.Build(),
            organisationABecameDaughterOfOrganisationBFor2016.Build()
        };
    }

    protected override AddOrganisationParent When()
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationAId),
            new OrganisationId(_organisationBId),
            new ValidFrom(new DateTime(2017, 1, 1)),
            new ValidTo(new DateTime(2017, 12, 31)));

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void DoesNotThrowAnException()
    {
        Exception.Should().BeNull();
    }

    [Fact]
    public void AnOrganisationParentWasAdded()
    {
        var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

        organisationParentAdded.OrganisationId.Should().Be(_organisationAId);
        organisationParentAdded.ParentOrganisationId.Should().Be(_organisationBId);
        organisationParentAdded.ValidFrom.Should().Be(new DateTime(2017, 1, 1));
        organisationParentAdded.ValidTo.Should().Be(new DateTime(2017, 12, 31));
    }
}
