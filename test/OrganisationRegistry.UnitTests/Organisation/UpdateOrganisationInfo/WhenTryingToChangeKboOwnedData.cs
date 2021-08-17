namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using Purpose;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToChangeKboOwnedData : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfo>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreatedTestDataBuilder;
        private DateTime _yesterday;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                new DateTimeProviderStub(DateTime.Today), Mock.Of<IOrganisationRegistryConfiguration>());
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationCreatedTestDataBuilder = new OrganisationCreatedTestDataBuilder(new SequentialOvoNumberGenerator());
            _yesterday = DateTime.Today.AddDays(-1);

            return new List<IEvent>
            {
                _organisationCreatedTestDataBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationCoupledWithKbo(
                    _organisationCreatedTestDataBuilder.Id,
                    "012313212",
                    _organisationCreatedTestDataBuilder.Name,
                    "OVO999999",
                    new DateTime()),
                new OrganisationBecameActive(_organisationCreatedTestDataBuilder.Id)
            };
        }

        protected override UpdateOrganisationInfo When()
        {
            return new UpdateOrganisationInfo(
                _organisationCreatedTestDataBuilder.Id,
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
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void TheOrganisationBecomesActive()
        {
            Exception.Should().BeOfType<CannotChangeKboDataException>();
        }

        public WhenTryingToChangeKboOwnedData(ITestOutputHelper helper) : base(helper) { }
    }
}
