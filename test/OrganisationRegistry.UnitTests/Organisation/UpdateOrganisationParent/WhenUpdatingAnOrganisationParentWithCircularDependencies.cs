namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent
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

    public class WhenUpdatingAnOrganisationParentWithCircularDependencies : OldExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationParent>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private OrganisationCreatedBuilder _organisationACreated;
        private OrganisationCreatedBuilder _organisationBCreated;
        private OrganisationParentAddedBuilder _organisationABecameDaughterOfOrganisationBFor2016;
        private OrganisationParentAddedBuilder _organisationBBecameDaughterOfOrganisationFor2017;

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
                    .WithValidity(new DateTime(2016,1,1), new DateTime(2016, 12, 31));
            _organisationBBecameDaughterOfOrganisationFor2017 =
                new OrganisationParentAddedBuilder(_organisationACreated.Id,
                        _organisationBCreated.Id)
                    .WithValidity(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));

            return new List<IEvent>
            {
                _organisationACreated.Build(),
                _organisationBCreated.Build(),
                _organisationABecameDaughterOfOrganisationBFor2016.Build(),
                _organisationBBecameDaughterOfOrganisationFor2017.Build()
            };
        }

        protected override UpdateOrganisationParent When()
        {
            return new UpdateOrganisationParent(
                _organisationBBecameDaughterOfOrganisationFor2017.OrganisationOrganisationParentId,
                _organisationBBecameDaughterOfOrganisationFor2017.OrganisationId,
                _organisationBBecameDaughterOfOrganisationFor2017.ParentOrganisationId,
                new ValidFrom(new DateTime(2016, 1, 1)), new ValidTo(new DateTime(2016, 12, 31)))
            {
                User = new UserBuilder()
                    .AddOrganisations(_organisationACreated.OvoNumber)
                    .AddRoles(Role.DecentraalBeheerder)
                    .Build()
            };;
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
        }

        public WhenUpdatingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper) { }
    }
}
