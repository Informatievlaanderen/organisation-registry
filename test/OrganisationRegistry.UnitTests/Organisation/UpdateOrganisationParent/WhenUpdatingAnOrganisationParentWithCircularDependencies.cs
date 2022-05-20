namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent;

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

public class WhenUpdatingAnOrganisationParentWithCircularDependencies
    : ExceptionOldSpecification2<UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
{
    private readonly DateTimeProviderStub _dateTimeProviderStub = new(new DateTime(2016, 6, 1));

    private readonly SequentialOvoNumberGenerator
        _sequentialOvoNumberGenerator = new();

    private string _ovoNumberA = null!;
    private Guid _organisationId;
    private Guid _organisationParentId;
    private Guid _organisationOrganisationParentId;


    public WhenUpdatingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override UpdateOrganisationParentCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
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
        var organisationBBecameDaughterOfOrganisationFor2017 =
            new OrganisationParentAddedBuilder(
                    organisationACreated.Id,
                    organisationBCreated.Id)
                .WithValidity(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));

        _ovoNumberA = organisationACreated.OvoNumber;
        _organisationId = organisationBBecameDaughterOfOrganisationFor2017.OrganisationId;
        _organisationParentId = organisationBBecameDaughterOfOrganisationFor2017.ParentOrganisationId;
        _organisationOrganisationParentId =
            organisationBBecameDaughterOfOrganisationFor2017.OrganisationOrganisationParentId;

        return new List<IEvent>
        {
            organisationACreated.Build(),
            organisationBCreated.Build(),
            organisationABecameDaughterOfOrganisationBFor2016.Build(),
            organisationBBecameDaughterOfOrganisationFor2017.Build()
        };
    }

    protected override UpdateOrganisationParent When()
        => new(
            _organisationOrganisationParentId,
            new OrganisationId(_organisationId),
            new OrganisationId(_organisationParentId),
            new ValidFrom(new DateTime(2016, 1, 1)),
            new ValidTo(new DateTime(2016, 12, 31)));

    protected override int ExpectedNumberOfEvents
        => 0;

    [Fact]
    public void ThrowsADomainException()
    {
        Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
    }
}
