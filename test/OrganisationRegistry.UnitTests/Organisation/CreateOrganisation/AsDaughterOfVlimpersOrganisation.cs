namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Purpose;
    using Tests.Shared;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class AsDaughterOfVlimpersOrganisation: Specification<Organisation, OrganisationCommandHandlers, CreateOrganisation>
    {
        private OrganisationCreatedBuilder _organisationCreatedBuilder;

        protected override IEnumerable<IEvent> Given()
        {
            _organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());

            return new List<IEvent>
            {
                _organisationCreatedBuilder.Build(),
                new OrganisationPlacedUnderVlimpersManagement(_organisationCreatedBuilder.Id)
            };
        }

        protected override CreateOrganisation When()
        {
            return new CreateOrganisation(
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
                new ValidTo())
            {
                User = new UserBuilder()
                    .AddRoles(Role.VlimpersBeheerder)
                    .Build()
            };
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override int ExpectedNumberOfEvents => 5;

        [Fact]
        public void CreatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
            organisationCreated.Should().NotBeNull();
        }

        [Fact]
        public void TheOrganisationIsPlacedUnderVlimpersManagement()
        {
            var organisationBecameActive = PublishedEvents[4].UnwrapBody<OrganisationPlacedUnderVlimpersManagement>();;
            organisationBecameActive.Should().NotBeNull();
        }


        public AsDaughterOfVlimpersOrganisation(ITestOutputHelper helper) : base(helper) { }
    }
}
