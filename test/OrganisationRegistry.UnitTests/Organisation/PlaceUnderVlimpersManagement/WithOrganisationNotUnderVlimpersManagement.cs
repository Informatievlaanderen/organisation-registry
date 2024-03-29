namespace OrganisationRegistry.UnitTests.Organisation.PlaceUnderVlimpersManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AutoFixture;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Domain;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
using Tests.Shared;
using Xunit;
using Xunit.Abstractions;

public class
    WithOrganisationNotUnderVlimpersManagement : Specification<PlaceUnderVlimpersManagementCommandHandler,
        PlaceUnderVlimpersManagement>
{
    private readonly Guid _organisationId;
    private readonly Fixture _fixture;
    private readonly Period _validity;
    private readonly Period _formalValidity;

    public WithOrganisationNotUnderVlimpersManagement(ITestOutputHelper helper) : base(helper)
    {
        _organisationId = Guid.NewGuid();
        _fixture = new Fixture();
        _fixture.CustomizeArticle();
        _fixture.CustomizePeriod();

        _validity = _fixture.Create<Period>();
        _formalValidity = _fixture.Create<Period>();
    }


    private IEvent[] Events
        => new IEvent[]
        {
            new OrganisationCreated(
                _organisationId,
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<string>(),
                _fixture.Create<Article>(),
                _fixture.Create<string>(),
                _fixture.Create<List<Purpose>>(),
                _fixture.Create<bool>(),
                _formalValidity.Start,
                _formalValidity.End,
                _validity.Start,
                _validity.End),
        };

    private PlaceUnderVlimpersManagement PlaceUnderVlimpersManagementCommand
        => new(new OrganisationId(_organisationId));

    protected override PlaceUnderVlimpersManagementCommandHandler BuildHandler(ISession session)
        => new(
            new Mock<ILogger<PlaceUnderVlimpersManagementCommandHandler>>().Object,
            session);

    [Fact]
    public async Task PublishesOneEvent()
    {
        await Given(Events).When(PlaceUnderVlimpersManagementCommand, TestUser.AlgemeenBeheerder)
            .ThenItPublishesTheCorrectNumberOfEvents(1);
    }

    [Fact]
    public async Task PlacesTheOrganisationUnderVlimpersManagement()
    {
        await Given(Events).When(PlaceUnderVlimpersManagementCommand, TestUser.AlgemeenBeheerder).Then();
        PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationPlacedUnderVlimpersManagement>>();
    }
}
