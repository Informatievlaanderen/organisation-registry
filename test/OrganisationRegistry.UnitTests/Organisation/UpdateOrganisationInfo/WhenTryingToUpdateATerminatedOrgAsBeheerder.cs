namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using Configuration;
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
    using Xunit;
    using Xunit.Abstractions;

    public class WhenTryingToUpdateATerminatedOrgAsBeheerder : ExceptionSpecification<Organisation, OrganisationCommandHandlers, UpdateOrganisationInfo>
    {
        private OrganisationCreatedTestDataBuilder _organisationCreatedTestDataBuilder;

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
            var fixture = new Fixture();
            _organisationCreatedTestDataBuilder = new OrganisationCreatedTestDataBuilder(new SequentialOvoNumberGenerator());

            return new List<IEvent>
            {
                _organisationCreatedTestDataBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedTestDataBuilder.Id),
                new OrganisationTerminatedV2(_organisationCreatedTestDataBuilder.Id,
                    fixture.Create<string>(),
                    fixture.Create<string>(),
                    fixture.Create<DateTime>(),
                    new FieldsToTerminateV2(null,
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>(),
                        new Dictionary<Guid, DateTime>()),
                    new KboFieldsToTerminateV2(
                        new Dictionary<Guid, DateTime>(),
                        new KeyValuePair<Guid, DateTime>?(),
                        new KeyValuePair<Guid, DateTime>?(),
                        new KeyValuePair<Guid, DateTime>?()
                        ),
                    fixture.Create<bool>(),
                    fixture.Create<DateTime?>()
                    ),
            };
        }

        protected override UpdateOrganisationInfo When()
        {
            var user = new UserBuilder()
                .AddRoles(Role.OrganisatieBeheerder)
                .AddOrganisations(_organisationCreatedTestDataBuilder.OvoNumber)
                .Build();

            return new UpdateOrganisationInfo(
                _organisationCreatedTestDataBuilder.Id,
                "Test",
                Article.None,
                "testing",
                "shortname",
                new List<PurposeId>(),
                true,
                new ValidFrom(),
                new ValidTo(),
                new ValidFrom(),
                new ValidTo())
            {
                User = user
            };
        }

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsOrganisationTerminatedException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyTerminated>();
        }

        public WhenTryingToUpdateATerminatedOrgAsBeheerder(ITestOutputHelper helper) : base(helper) { }
    }
}
