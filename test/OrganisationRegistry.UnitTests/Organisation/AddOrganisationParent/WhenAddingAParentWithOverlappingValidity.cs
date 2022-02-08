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
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAParentWithOverlappingValidity: ExceptionSpecification<Organisation, OrganisationCommandHandlers, AddOrganisationParent>
    {
        private DateTime _validTo;
        private DateTime _validFrom;
        private DateTimeProviderStub _dateTimeProviderStub;
        private OrganisationCreated _childCreated;
        private OrganisationCreated _parentCreated;
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,

                _sequentialOvoNumberGenerator,
                null,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Now);

            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);

            _childCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator).Build();
            _parentCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator).Build();
            return new List<IEvent>
            {
                _childCreated,
                _parentCreated,
                new OrganisationParentAdded(_childCreated.OrganisationId, Guid.NewGuid(),
                    _parentCreated.OrganisationId, "Ouder en Gezin", null, null)
            };
        }

        protected override AddOrganisationParent When()
        {
            return new AddOrganisationParent(
                Guid.NewGuid(),
                new OrganisationId(_childCreated.OrganisationId),
                new OrganisationId(_parentCreated.OrganisationId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo))
            {
                User = new UserBuilder()
                    .AddOrganisations(_childCreated.OvoNumber)
                    .AddRoles(Role.OrganisatieBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
            Exception.Message.Should().Be("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.");
        }

        public WhenAddingAParentWithOverlappingValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
