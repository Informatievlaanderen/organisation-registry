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
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class
        WhenAddingAParentWithOverlappingValidity
        : ExceptionSpecification<AddOrganisationParentCommandHandler, AddOrganisationParent>
    {
        private DateTime _validTo;
        private DateTime _validFrom;
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
        private OrganisationCreated _childCreated;
        private OrganisationCreated _parentCreated;

        private readonly SequentialOvoNumberGenerator
            _sequentialOvoNumberGenerator = new();

        public WhenAddingAParentWithOverlappingValidity(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
                Session,
                new DateTimeProvider()
            );

        protected override IUser User
            => new UserBuilder()
                .AddOrganisations(_childCreated.OvoNumber)
                .AddRoles(Role.DecentraalBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);

            _childCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();
            _parentCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();
            return new List<IEvent>
            {
                _childCreated,
                _parentCreated,
                new OrganisationParentAdded(
                    _childCreated.OrganisationId,
                    Guid.NewGuid(),
                    _parentCreated.OrganisationId,
                    "Ouder en Gezin",
                    null,
                    null)
            };
        }

        protected override AddOrganisationParent When()
            => new(
                Guid.NewGuid(),
                new OrganisationId(_childCreated.OrganisationId),
                new OrganisationId(_parentCreated.OrganisationId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyCoupledToParentInThisPeriod>();
            Exception?.Message.Should()
                .Be("Deze organisatie is in deze periode reeds gekoppeld aan een moeder entiteit.");
        }


    }
}
