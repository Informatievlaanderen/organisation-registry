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
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationParentWithCircularDependencies : OldExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private OrganisationCreatedBuilder _organisationACreated;
        private OrganisationCreatedBuilder _organisationBCreated;
        private OrganisationParentAddedBuilder _organisationABecameDaughterOfOrganisationB;

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
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            _organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationABecameDaughterOfOrganisationB = new OrganisationParentAddedBuilder(_organisationACreated.Id, _organisationBCreated.Id);

            return new List<IEvent>
            {
                _organisationACreated.Build(),
                _organisationBCreated.Build(),
                _organisationABecameDaughterOfOrganisationB.Build(),
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                Guid.NewGuid(),
                _organisationBCreated.Id,
                _organisationACreated.Id,
                new ValidFrom(),
                new ValidTo())
            {
                User = new UserBuilder()
                    .AddOrganisations(_organisationBCreated.OvoNumber)
                    .AddRoles(Role.DecentraalBeheerder)
                    .Build()
            };;
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<CircularRelationshipDetected>();
        }

        public WhenAddingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper) { }
    }
}
