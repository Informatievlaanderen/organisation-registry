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
using Tests.Shared.Stubs;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class AsDaughterOfVlimpersOrganisation : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
{
    private readonly Guid _organisationId;

    public AsDaughterOfVlimpersOrganisation(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
    }

    private IEvent[] Events
        => new IEvent[] {
            new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator())
                .WithId(new OrganisationId(_organisationId))
                .Build(),
            new OrganisationPlacedUnderVlimpersManagement(_organisationId),
        };

    private CreateOrganisation CreateOrganisationCommand
        => new(
            new OrganisationId(Guid.NewGuid()),
            "Test",
            "OVO0001234",
            "",
            Article.None,
            new OrganisationId(_organisationId),
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
        await Given(Events).When(CreateOrganisationCommand, TestUser.VlimpersBeheerder).ThenItPublishesTheCorrectNumberOfEvents(5);
    }

    [Fact]
    public async Task CreatesAnOrganisation()
    {
        await Given(Events).When(CreateOrganisationCommand, TestUser.VlimpersBeheerder).Then();
        PublishedEvents[0]
            .UnwrapBody<OrganisationCreated>()
            .Should()
            .NotBeNull();
    }

    [Fact]
    public async Task TheOrganisationIsPlacedUnderVlimpersManagement()
    {
        await Given(Events).When(CreateOrganisationCommand, TestUser.VlimpersBeheerder).Then();
        PublishedEvents[4]
            .UnwrapBody<OrganisationPlacedUnderVlimpersManagement>()
            .Should()
            .NotBeNull();
    }
}
