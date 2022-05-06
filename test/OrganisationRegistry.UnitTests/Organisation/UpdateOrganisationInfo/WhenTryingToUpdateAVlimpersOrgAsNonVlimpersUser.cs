namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Purpose;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using OrganisationRegistry.Organisation.Update;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToUpdateAVlimpersOrgAsNonVlimpersUser : ExceptionSpecification<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
    {
        private readonly OrganisationCreatedBuilder _organisationCreatedBuilder = new(new SequentialOvoNumberGenerator());
        private DateTime _yesterday;

        public WhenTryingToUpdateAVlimpersOrgAsNonVlimpersUser(ITestOutputHelper helper) : base(helper) { }

        protected override UpdateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
                Session,
                new DateTimeProviderStub(DateTime.Today));

        protected override IUser User
            => new UserBuilder().Build();

        protected override IEnumerable<IEvent> Given()
        {
            _yesterday = DateTime.Today.AddDays(-1);

            return new List<IEvent>
            {
                _organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedBuilder.Id),
                new OrganisationPlacedUnderVlimpersManagement(_organisationCreatedBuilder.Id)
            };
        }

        protected override UpdateOrganisationInfo When()
            => new(
                _organisationCreatedBuilder.Id,
                "Test",
                Article.None,
                "testing",
                "",
                new List<PurposeId>(),
                false,
                new ValidFrom(_yesterday),
                new ValidTo(_yesterday),
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsAnException()
        {
            Exception.Should().BeOfType<InsufficientRights>();
        }
    }
}
