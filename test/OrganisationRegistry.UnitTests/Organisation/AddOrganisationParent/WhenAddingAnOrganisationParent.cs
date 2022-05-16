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
    using Tests.Shared.TestDataBuilders;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAnOrganisationParent
        : Specification<AddOrganisationParentCommandHandler, AddOrganisationParent>
    {
        private Guid _organisationOrganisationParentId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);

        private readonly SequentialOvoNumberGenerator
            _sequentialOvoNumberGenerator = new();

        private OrganisationCreated _childCreated;
        private OrganisationCreated _parentCreated;

        public WhenAddingAnOrganisationParent(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub);

        protected override IUser User
            => new UserBuilder()
                .AddOrganisations(_childCreated.OvoNumber)
                .AddRoles(Role.DecentraalBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationOrganisationParentId = Guid.NewGuid();
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);

            _childCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();
            _parentCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator).Build();
            return new List<IEvent>
            {
                _childCreated,
                _parentCreated,
            };
        }

        protected override AddOrganisationParent When()
            => new(
                _organisationOrganisationParentId,
                new OrganisationId(_childCreated.OrganisationId),
                new OrganisationId(_parentCreated.OrganisationId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));

        protected override int ExpectedNumberOfEvents
            => 2;

        [Fact]
        public void AddsAnOrganisationParent()
        {
            var organisationParentAdded = PublishedEvents[0].UnwrapBody<OrganisationParentAdded>();

            organisationParentAdded.OrganisationOrganisationParentId.Should().Be(_organisationOrganisationParentId);
            organisationParentAdded.OrganisationId.Should().Be(_childCreated.OrganisationId);
            organisationParentAdded.ParentOrganisationId.Should().Be(_parentCreated.OrganisationId);
            organisationParentAdded.ValidFrom.Should().Be(_validFrom);
            organisationParentAdded.ValidTo.Should().Be(_validTo);
        }

        [Fact]
        public void AssignsAParent()
        {
            var parentAssignedToOrganisation = PublishedEvents[1].UnwrapBody<ParentAssignedToOrganisation>();
            parentAssignedToOrganisation.OrganisationId.Should().Be(_childCreated.OrganisationId);
            parentAssignedToOrganisation.ParentOrganisationId.Should().Be(_parentCreated.OrganisationId);
        }


    }
}
