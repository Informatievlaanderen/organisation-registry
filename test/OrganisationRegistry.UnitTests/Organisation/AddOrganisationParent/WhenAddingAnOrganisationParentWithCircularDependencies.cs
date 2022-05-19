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
using OrganisationRegistry.Organisation.Exceptions;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingAnOrganisationParentWithCircularDependencies
    : ExceptionSpecification<AddOrganisationParentCommandHandler, AddOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new (DateTime.Now);
    private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new();
    private string _ovoNumberB = null!;
    private Guid _organisationAId;
    private Guid _organisationBId;

    public WhenAddingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override AddOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
            Session,
            _dateTimeProviderStub);

    protected override IUser User
        => new UserBuilder()
            .AddOrganisations(_ovoNumberB)
            .AddRoles(Role.DecentraalBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        var organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
        var organisationABecameDaughterOfOrganisationB = new OrganisationParentAddedBuilder(
            organisationACreated.Id,
            organisationBCreated.Id);

        _ovoNumberB = organisationBCreated.OvoNumber;
        _organisationAId = organisationACreated.Id;
        _organisationBId = organisationBCreated.Id;

        return new List<IEvent>
        {
            organisationACreated.Build(),
            organisationBCreated.Build(),
            organisationABecameDaughterOfOrganisationB.Build(),
        };
    }

    protected override AddOrganisationParent When()
        => new(
            Guid.NewGuid(),
            new OrganisationId(_organisationBId),
            new OrganisationId(_organisationAId),
            new ValidFrom(),
            new ValidTo());

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsADomainException()
    {
        Exception.Should().BeOfType<CircularRelationshipDetected>();
    }
}
