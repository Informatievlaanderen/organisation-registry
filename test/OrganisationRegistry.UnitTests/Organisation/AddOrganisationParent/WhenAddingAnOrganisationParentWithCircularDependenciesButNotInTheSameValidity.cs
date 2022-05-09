namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity : OldExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private OrganisationCreatedBuilder _organisationACreated;
        private OrganisationCreatedBuilder _organisationBCreated;
        private OrganisationParentAddedBuilder _organisationABecameDaughterOfOrganisationBFor2016;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                _sequentialOvoNumberGenerator,
                null,
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2016, 6, 1));

            _organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationABecameDaughterOfOrganisationBFor2016 =
                new OrganisationParentAddedBuilder(_organisationACreated.Id, _organisationBCreated.Id)
                    .WithValidity(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31));

            return new List<IEvent>
            {
                _organisationACreated.Build(),
                _organisationBCreated.Build(),
                _organisationABecameDaughterOfOrganisationBFor2016.Build()
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                Guid.NewGuid(),
                _organisationABecameDaughterOfOrganisationBFor2016.OrganisationId,
                _organisationABecameDaughterOfOrganisationBFor2016.ParentOrganisationId,
                new ValidFrom(new DateTime(2017, 1, 1)), new ValidTo(new DateTime(2017, 12, 31)))
            {
                User = new UserBuilder()
                    .AddOrganisations(_organisationACreated.OvoNumber)
                    .AddRoles(Role.DecentraalBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void DoesNotThrowAnException()
        {
            Exception.Should().BeNull();
        }

        [Fact]
        public void AnOrganisationParentWasAdded()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

            organisationParentAdded.OrganisationId.Should().Be((Guid)_organisationACreated.Id);
            organisationParentAdded.ParentOrganisationId.Should().Be((Guid)_organisationBCreated.Id);
            organisationParentAdded.ValidFrom.Should().Be(new DateTime(2017, 1, 1));
            organisationParentAdded.ValidTo.Should().Be(new DateTime(2017, 12, 31));
        }

        public WhenAddingAnOrganisationParentWithCircularDependenciesButNotInTheSameValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
