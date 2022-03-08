namespace OrganisationRegistry.UnitTests.Organisation.ReleaseFromVlimpersManagement
{
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
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using TerminateOrganisation;
    using Tests.Shared;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class WithOrganisationUnderVlimpersManagement: Specification<Organisation, OrganisationCommandHandlers, ReleaseFromVlimpersManagement>
    {
        private OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub;

        private OrganisationId _organisationId;
        private DateTimeProviderStub _dateTimeProviderStub;

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();
            fixture.CustomizeArticle();
            fixture.CustomizePeriod();

            _dateTimeProviderStub = new DateTimeProviderStub(fixture.Create<DateTime>());
            _organisationRegistryConfigurationStub = new OrganisationRegistryConfigurationStub();
            _organisationId = new OrganisationId(fixture.Create<Guid>());

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

        protected override ReleaseFromVlimpersManagement When()
            => new ReleaseFromVlimpersManagement(
                    _organisationId)
                .WithUserRole(Role.OrganisationRegistryBeheerder);

        protected override OrganisationCommandHandlers BuildHandler()
            => new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                _dateTimeProviderStub,
                _organisationRegistryConfigurationStub,
                Mock.Of<ISecurityService>());


        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void PlacesTheOrganisationUnderVlimpersManagement()
        {
            PublishedEvents.Single().Should().BeOfType<Envelope<OrganisationReleasedFromVlimpersManagement>>();
        }

        public WithOrganisationUnderVlimpersManagement(ITestOutputHelper helper) : base(helper) { }
    }
}
