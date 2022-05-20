namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation;

using System;
using System.Collections.Generic;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using Purpose;
using Tests.Shared;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class AsDaughterOfVlimpersOrganisation : OldSpecification2<CreateOrganisationCommandHandler, CreateOrganisation>
{
    private readonly OrganisationCreatedBuilder _organisationCreatedBuilder = new(new SequentialOvoNumberGenerator());

    public AsDaughterOfVlimpersOrganisation(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IEnumerable<IEvent> Given()
        => new List<IEvent>
        {
            _organisationCreatedBuilder.Build(),
            new OrganisationPlacedUnderVlimpersManagement(_organisationCreatedBuilder.Id)
        };

    protected override CreateOrganisation When()
        => new(
            new OrganisationId(Guid.NewGuid()),
            "Test",
            "OVO0001234",
            "",
            Article.None,
            _organisationCreatedBuilder.Id,
            "",
            new List<PurposeId>(),
            false,
            new ValidFrom(),
            new ValidTo(),
            new ValidFrom(),
            new ValidTo());

    protected override CreateOrganisationCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<CreateOrganisationCommandHandler>>().Object,
            Session,
            new SequentialOvoNumberGenerator(),
            new UniqueOvoNumberValidatorStub(false),
            new DateTimeProviderStub(DateTime.Today));

    protected override IUser User
        => new UserBuilder().AddRoles(Role.VlimpersBeheerder).Build();

    protected override int ExpectedNumberOfEvents
        => 5;

    [Fact]
    public void CreatesAnOrganisation()
    {
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void TheOrganisationIsPlacedUnderVlimpersManagement()
    {
        var organisationBecameActive = PublishedEvents[4].UnwrapBody<OrganisationPlacedUnderVlimpersManagement>();
        organisationBecameActive.Should().NotBeNull();
    }
}
