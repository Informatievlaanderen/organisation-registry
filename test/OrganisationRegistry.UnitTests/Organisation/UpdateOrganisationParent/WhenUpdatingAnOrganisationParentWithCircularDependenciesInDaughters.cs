namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationParent>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();
        private OrganisationCreatedTestDataBuilder _organisationACreated;
        private OrganisationCreatedTestDataBuilder _organisationBCreated;
        private OrganisationCreatedTestDataBuilder _organisationCCreated;
        private OrganisationParentAddedTestDataBuilder _organisationABecameDaughterOfOrganisationBFor2016;
        private OrganisationParentAddedTestDataBuilder _organisationBBecameDaughterOfOrganisationCFor2017;
        private OrganisationParentAddedTestDataBuilder _organisationCBecameDaughterOfOrganisationAFor2018;

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

            _organisationACreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            _organisationCCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);

            _organisationABecameDaughterOfOrganisationBFor2016 =
                new OrganisationParentAddedTestDataBuilder(_organisationACreated.Id, _organisationBCreated.Id)
                    .WithValidity(new DateTime(2016,1,1), new DateTime(2016, 12, 31));

            _organisationBBecameDaughterOfOrganisationCFor2017 =
                new OrganisationParentAddedTestDataBuilder(_organisationBCreated.Id, _organisationCCreated.Id)
                    .WithValidity(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));

            _organisationCBecameDaughterOfOrganisationAFor2018 =
                new OrganisationParentAddedTestDataBuilder(_organisationACreated.Id,
                        _organisationBCreated.Id)
                    .WithValidity(new DateTime(2018, 1, 1), new DateTime(2018, 12, 31));

            return new List<IEvent>
            {
                _organisationACreated.Build(),
                _organisationBCreated.Build(),
                _organisationABecameDaughterOfOrganisationBFor2016.Build(),
                _organisationBBecameDaughterOfOrganisationCFor2017.Build(),
                _organisationCBecameDaughterOfOrganisationAFor2018.Build(),
            };
        }

        protected override UpdateOrganisationParent When()
        {
            return new UpdateOrganisationParent(
                _organisationCBecameDaughterOfOrganisationAFor2018.OrganisationOrganisationParentId,
                _organisationCBecameDaughterOfOrganisationAFor2018.OrganisationId,
                _organisationCBecameDaughterOfOrganisationAFor2018.ParentOrganisationId,
                new ValidFrom(new DateTime(2016, 1, 1)), new ValidTo(new DateTime(2016, 12, 31)))
            {
                User = new UserBuilder()
                    .AddOrganisations(_organisationACreated.OvoNumber)
                    .AddRoles(Role.OrganisatieBeheerder)
                    .Build()
            };;
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
        }

        public WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters(ITestOutputHelper helper) : base(helper) { }
    }
}
