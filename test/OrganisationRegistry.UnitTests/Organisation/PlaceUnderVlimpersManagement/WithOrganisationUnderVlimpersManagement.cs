namespace OrganisationRegistry.UnitTests.Organisation.PlaceUnderVlimpersManagement
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using AutoFixture;
    using FluentAssertions;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.UnitTests.Infrastructure.Tests.Extensions.TestHelpers;
    using Xunit;
    using Xunit.Abstractions;

    public class WithOrganisationUnderVlimpersManagement: Specification<PlaceUnderVlimpersManagementCommandHandler, PlaceUnderVlimpersManagement>
    {
        private readonly OrganisationId _organisationId= new(Guid.NewGuid());

        public WithOrganisationUnderVlimpersManagement(ITestOutputHelper helper) : base(helper) { }

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();
            fixture.CustomizeArticle();
            fixture.CustomizePeriod();

            var validity = fixture.Create<Period>();
            var formalValidity = fixture.Create<Period>();
            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<Article>(),
                    fixture.Create<string>(),
                    fixture.Create<List<Purpose>>(),
                    fixture.Create<bool>(),
                    formalValidity.Start,
                    formalValidity.End,
                    validity.Start,
                    validity.End),
                new OrganisationPlacedUnderVlimpersManagement(_organisationId)
            };
        }

        protected override PlaceUnderVlimpersManagement When()
            => new(_organisationId);

        protected override PlaceUnderVlimpersManagementCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<PlaceUnderVlimpersManagementCommandHandler>>().Object,
                Session);

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void PlacesTheOrganisationUnderVlimpersManagement()
        {
            PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationPlacedUnderVlimpersManagement>>();
        }
    }
}
