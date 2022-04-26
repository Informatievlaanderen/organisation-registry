namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
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
    using Purpose;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToChangeKboOwnedData : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfo>
    {
        private OrganisationCreatedBuilder _organisationCreatedBuilder;
        private DateTime _yesterday;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today), Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationCreatedBuilder = new OrganisationCreatedBuilder(new SequentialOvoNumberGenerator());
            _yesterday = DateTime.Today.AddDays(-1);

            return new List<IEvent>
            {
                _organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationCoupledWithKbo(
                    _organisationCreatedBuilder.Id,
                    "012313212",
                    _organisationCreatedBuilder.Name,
                    "OVO999999",
                    new DateTime()),
                new OrganisationBecameActive(_organisationCreatedBuilder.Id)
            };
        }

        protected override UpdateOrganisationInfo When()
        {
            return new UpdateOrganisationInfo(
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
                new ValidTo())
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            Exception.Should().BeOfType<CannotChangeDataOwnedByKbo>();
        }

        public WhenTryingToChangeKboOwnedData(ITestOutputHelper helper) : base(helper) { }
    }
}
