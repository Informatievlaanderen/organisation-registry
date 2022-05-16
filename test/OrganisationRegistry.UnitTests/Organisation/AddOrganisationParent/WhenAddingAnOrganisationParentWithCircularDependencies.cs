namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent
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
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationParentWithCircularDependencies : ExceptionSpecification<
        AddOrganisationParentCommandHandler, AddOrganisationParent>
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub = new (DateTime.Now);
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new();
        private OrganisationCreatedBuilder _organisationACreated;
        private OrganisationCreatedBuilder _organisationBCreated;
        private OrganisationParentAddedBuilder _organisationABecameDaughterOfOrganisationB;

        public WhenAddingAnOrganisationParentWithCircularDependencies(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub);

        protected override IUser User
            => new UserBuilder()
                .AddOrganisations(_organisationBCreated.OvoNumber)
                .AddRoles(Role.DecentraalBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            _organisationABecameDaughterOfOrganisationB = new OrganisationParentAddedBuilder(
                _organisationACreated.Id,
                _organisationBCreated.Id);

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
                ;
        }

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsADomainException()
        {
            Exception.Should().BeOfType<CircularRelationshipDetected>();
        }
    }
}
