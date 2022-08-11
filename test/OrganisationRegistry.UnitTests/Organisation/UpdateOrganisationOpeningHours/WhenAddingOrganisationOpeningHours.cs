namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationOpeningHours;

using System;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Tests.Shared;
using Tests.Shared.TestDataBuilders;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Xunit;
using Xunit.Abstractions;

public class WhenUpdatingOrganisationOpeningHours : Specification<UpdateOrganisationOpeningHourCommandHandler, UpdateOrganisationOpeningHour>
{
    private readonly Fixture _fixture;

    private readonly Guid _organisationId;
    private readonly Guid _organisationOpeningHourId;
    private readonly TimeSpan _opens;
    private readonly TimeSpan _closes;

    public WhenUpdatingOrganisationOpeningHours(ITestOutputHelper helper) : base(helper)
    {
        _fixture = new Fixture();
        _organisationId = _fixture.Create<Guid>();
        _organisationOpeningHourId = _fixture.Create<Guid>();
        _opens = _fixture.Create<TimeSpan>();
        _closes = _fixture.Create<TimeSpan>();
    }

    protected override UpdateOrganisationOpeningHourCommandHandler BuildHandler(ISession session)
        => new(Mock.Of<ILogger<UpdateOrganisationOpeningHourCommandHandler>>(), session);

    private UpdateOrganisationOpeningHour UpdateOrganisationOpeningHourCommand
        => new(
            _organisationOpeningHourId,
            new OrganisationId(_organisationId),
            _opens,
            _closes,
            null,
            new ValidFrom(),
            new ValidTo());

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(OrganisationCreated(), OrganisationOpeningHourAdded())
            .When(UpdateOrganisationOpeningHourCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);

        var organisationOpeningHourUpdated = PublishedEvents[0].UnwrapBody<OrganisationOpeningHourUpdated>();
        organisationOpeningHourUpdated.OrganisationId.Should().Be(_organisationId);
        organisationOpeningHourUpdated.OrganisationOpeningHourId.Should().Be(_organisationOpeningHourId);
        organisationOpeningHourUpdated.Opens.Should().Be(_opens);
        organisationOpeningHourUpdated.Closes.Should().Be(_closes);
    }

    private OrganisationCreated OrganisationCreated()
        => new OrganisationCreatedBuilder().WithId(_organisationId);

    private OrganisationOpeningHourAdded OrganisationOpeningHourAdded()
        => new(_organisationId, _organisationOpeningHourId, _fixture.Create<TimeSpan>(), _fixture.Create<TimeSpan>(), null, null, null);
}
