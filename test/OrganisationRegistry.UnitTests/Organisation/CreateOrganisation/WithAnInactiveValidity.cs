namespace OrganisationRegistry.UnitTests.Organisation.CreateOrganisation
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
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WithAnInactiveValidity: Specification<Organisation, OrganisationCommandHandlers, CreateOrganisation>
    {
        private DateTimeProviderStub _dateTimeProviderStub;
        private DateTime _yesterday;

        protected override IEnumerable<IEvent> Given()
        {
            _dateTimeProviderStub = new DateTimeProviderStub(DateTime.Today);
            _yesterday = _dateTimeProviderStub.Now.AddDays(-1);

            return new List<IEvent>();
        }

        protected override CreateOrganisation When()
        {
            return new CreateOrganisation(
                new OrganisationId(Guid.NewGuid()),
                "Test",
                "OVO0001234",
                "",
                Article.None,
                null,
                "",
                new List<PurposeId>(),
                false,
                new ValidFrom(_yesterday),
                new ValidTo(_yesterday),
                new ValidFrom(),
                new ValidTo());
        }

        protected override OrganisationCommandHandlers BuildHandler()
        {
            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                _dateTimeProviderStub,
                Mock.Of<IOrganisationRegistryConfiguration>(),
                Mock.Of<ISecurityService>());
        }

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void CreatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
            organisationCreated.Should().NotBeNull();
        }

        public WithAnInactiveValidity(ITestOutputHelper helper) : base(helper) { }
    }
}
