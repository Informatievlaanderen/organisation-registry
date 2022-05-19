namespace OrganisationRegistry.UnitTests.Organisation.ReleaseFromVlimpersManagement;

using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using FluentAssertions;
using Infrastructure.Tests.Extensions.TestHelpers;
using Microsoft.Extensions.Logging;
using Moq;
using OrganisationRegistry.Infrastructure.Authorization;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation;
using OrganisationRegistry.Organisation.Events;
using Xunit;
using Xunit.Abstractions;

public class WithOrganisationNotUnderVlimpersManagement : Specification<ReleaseFromVlimpersManagementCommandHandler, ReleaseFromVlimpersManagement>
{
    private readonly OrganisationId _organisationId = new(Guid.NewGuid());
    private readonly Fixture _fixture = new();

    public WithOrganisationNotUnderVlimpersManagement(ITestOutputHelper helper) : base(helper)
    {
    }

    protected override IUser User
        => new UserBuilder()
            .AddRoles(Role.AlgemeenBeheerder)
            .Build();

    protected override IEnumerable<IEvent> Given()
    {
        _fixture.CustomizeArticle();
        _fixture.CustomizePeriod();

        var validity = _fixture.Create<Period>();
        var formalValidity = _fixture.Create<Period>();
        return new List<IEvent>
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
                formalValidity.Start,
                formalValidity.End,
                validity.Start,
                validity.End)
        };
    }

    protected override ReleaseFromVlimpersManagement When()
        => new(_organisationId);

    protected override ReleaseFromVlimpersManagementCommandHandler BuildHandler()
        => new(
            new Mock<ILogger<ReleaseFromVlimpersManagementCommandHandler>>().Object,
            Session);

    protected override int ExpectedNumberOfEvents
        => 1;

    [Fact]
    public void PlacesTheOrganisationUnderVlimpersManagement()
    {
        PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationReleasedFromVlimpersManagement>>();
    }
}
