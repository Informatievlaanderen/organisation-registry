namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationOpeningHours;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using Xunit;
using Xunit.Abstractions;

public class WhenAddingOrganisationOpeningHours : Specification<AddOrganisationOpeningHourCommandHandler, AddOrganisationOpeningHour>
{
    private readonly Guid _organisationId;
    private readonly Guid _organisationOpeningHourId;

    public WhenAddingOrganisationOpeningHours(ITestOutputHelper helper) : base(helper)
    {
        var fixture = new Fixture();
        _organisationId = fixture.Create<Guid>();
        _organisationOpeningHourId = fixture.Create<Guid>();
    }

    protected override AddOrganisationOpeningHourCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<AddOrganisationOpeningHourCommandHandler>>(), session);

    private AddOrganisationOpeningHour AddOrganisationOpeningHourCommand
        => new(
            _organisationOpeningHourId,
            new OrganisationId(_organisationId),
            new TimeSpan(),
            new TimeSpan(),
            null,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated())
            .When(AddOrganisationOpeningHourCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationOpeningHourAdded = PublishedEvents[0].UnwrapBody<OrganisationOpeningHourAdded>();
        organisationOpeningHourAdded.OrganisationId.Should().Be(_organisationId);
        organisationOpeningHourAdded.OrganisationOpeningHourId.Should().Be(_organisationOpeningHourId);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);
}
