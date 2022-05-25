namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation;

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using Purpose;
using Tests.Shared;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WithAnActiveValidity : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
{
    public WithAnActiveValidity(ITestOutputHelper helper) : base(helper)
    {
    }

    private static IEvent[] Events
        => Array.Empty<IEvent>();

    private static CreateOrganisation CreateOrganisationCommand
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

    protected override CreateOrganisationCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<CreateOrganisationCommandHandler>>().Object,
            session,
            new SequentialOvoNumberGenerator(),
            new UniqueOvoNumberValidatorStub(false),
            new DateTimeProviderStub(DateTime.Today));

    [Fact]
    public async Task PublishesFiveEvents()
    {
        await Given(Events).When(CreateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).ThenItPublishesTheCorrectNumberOfEvents(2);
    }

    [Fact]
    public async Task CreatesAnOrganisation()
    {
        await Given(Events).When(CreateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).Then();
        PublishedEvents[0]
            .UnwrapBody<OrganisationCreated>().Should().NotBeNull();
    }

    [Fact]
    public async Task TheOrganisationBecomesActive()
    {
        await Given(Events).When(CreateOrganisationCommand, UserBuilder.AlgemeenBeheerder()).Then();

        PublishedEvents[1].UnwrapBody<OrganisationBecameActive>().Should().NotBeNull();
    }
}
