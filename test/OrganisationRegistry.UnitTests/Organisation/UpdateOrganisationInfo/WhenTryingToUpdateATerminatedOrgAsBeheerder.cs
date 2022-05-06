namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationInfo
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
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

    public class WhenTryingToUpdateATerminatedOrgAsBeheerder : ExceptionSpecification<UpdateOrganisationCommandHandler, UpdateOrganisationInfo>
    {
        private readonly OrganisationCreatedBuilder _organisationCreatedBuilder = new(new SequentialOvoNumberGenerator());

        public WhenTryingToUpdateATerminatedOrgAsBeheerder(ITestOutputHelper helper) : base(helper) { }

        protected override UpdateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<UpdateOrganisationCommandHandler>>().Object,
                Session,
                new DateTimeProviderStub(DateTime.Today));

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.DecentraalBeheerder)
                .AddOrganisations(_organisationCreatedBuilder.OvoNumber)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();

            return new List<IEvent>
            {
                _organisationCreatedBuilder
                    .WithValidity(null, null)
                    .Build(),
                new OrganisationBecameActive(_organisationCreatedBuilder.Id),
                new OrganisationTerminatedV2(_organisationCreatedBuilder.Id,
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
            => new(
                _organisationCreatedBuilder.Id,
                "Test",
                Article.None,
                "testing",
                "shortname",
                new List<PurposeId>(),
                true,
                new ValidFrom(),
                new ValidTo(),
                new ValidFrom(),
                new ValidTo());

        protected override int ExpectedNumberOfEvents => 0;

        [Fact]
        public void ThrowsOrganisationTerminatedException()
        {
            Exception.Should().BeOfType<OrganisationAlreadyTerminated>();
        }
    }
}
