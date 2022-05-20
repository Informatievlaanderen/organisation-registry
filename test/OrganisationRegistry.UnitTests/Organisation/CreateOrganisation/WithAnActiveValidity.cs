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
using Xunit;
using Xunit.Abstractions;

public class WithAnActiveValidity : OldSpecification2<CreateOrganisationCommandHandler, CreateOrganisation>
{
    public WithAnActiveValidity(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IEnumerable<IEvent> Given()
        => new List<IEvent>();

    protected override CreateOrganisation When()
        => new(
            new OrganisationId(Guid.NewGuid()),
            "Test",
            "OVO0001234",
            "",
            Article.None,
            null,
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
        => new UserBuilder().AddRoles(Role.AlgemeenBeheerder).Build();

    protected override int ExpectedNumberOfEvents
        => 2;

    [Fact]
    public void CreatesAnOrganisation()
    {
        var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
        organisationCreated.Should().NotBeNull();
    }

    [Fact]
    public void TheOrganisationBecomesActive()
    {
        var organisationBecameActive = PublishedEvents[1].UnwrapBody<OrganisationBecameActive>();
        organisationBecameActive.Should().NotBeNull();
    }
}
