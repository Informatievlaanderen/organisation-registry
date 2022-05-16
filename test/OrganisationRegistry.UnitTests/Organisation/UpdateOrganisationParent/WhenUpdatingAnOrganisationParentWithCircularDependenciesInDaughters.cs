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

    public class WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters : ExceptionSpecification<
        UpdateOrganisationParentCommandHandler, UpdateOrganisationParent>
    {
        private DateTimeProviderStub _dateTimeProviderStub;

        private readonly SequentialOvoNumberGenerator
            _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        private OrganisationCreatedBuilder _organisationACreated;
        private OrganisationCreatedBuilder _organisationBCreated;
        private OrganisationCreatedBuilder _organisationCCreated;
        private OrganisationParentAddedBuilder _organisationABecameDaughterOfOrganisationBFor2016;
        private OrganisationParentAddedBuilder _organisationBBecameDaughterOfOrganisationCFor2017;
        private OrganisationParentAddedBuilder _organisationCBecameDaughterOfOrganisationAFor2018;

        public WhenUpdatingAnOrganisationParentWithCircularDependenciesInDaughters(ITestOutputHelper helper) : base(
            helper)
        {
        }

        protected override UpdateOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationParentCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub);

        protected override IUser User
            => new UserBuilder()
                .AddOrganisations(_organisationACreated.OvoNumber)
                .AddRoles(Role.DecentraalBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(new DateTime(2016, 6, 1));

            _organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationCCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);

            _organisationABecameDaughterOfOrganisationBFor2016 =
                new OrganisationParentAddedBuilder(_organisationACreated.Id, _organisationBCreated.Id)
                    .WithValidity(new DateTime(2016, 1, 1), new DateTime(2016, 12, 31));

            _organisationBBecameDaughterOfOrganisationCFor2017 =
                new OrganisationParentAddedBuilder(_organisationBCreated.Id, _organisationCCreated.Id)
                    .WithValidity(new DateTime(2017, 1, 1), new DateTime(2017, 12, 31));

            _organisationCBecameDaughterOfOrganisationAFor2018 =
                new OrganisationParentAddedBuilder(
                        _organisationACreated.Id,
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
            => new(
                _organisationCBecameDaughterOfOrganisationAFor2018.OrganisationOrganisationParentId,
                _organisationCBecameDaughterOfOrganisationAFor2018.OrganisationId,
                _organisationCBecameDaughterOfOrganisationAFor2018.ParentOrganisationId,
                new ValidFrom(new DateTime(2016, 1, 1)),
                new ValidTo(new DateTime(2016, 12, 31)));

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
        }
    }
}
