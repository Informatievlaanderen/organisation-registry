namespace OrganisationRegistry.UnitTests.Organisation.TerminateOrganisation.CoupledToKbo
{
    using System;
    using System.Collections.Generic;
    using AutoFixture;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Tests.Shared.Stubs;
    using Xunit;
    using Xunit.Abstractions;

    public class ForceTerminateOrganisationAlreadyTerminatedInKbo : ExceptionSpecification<TerminateOrganisationCommandHandler, TerminateOrganisation>
    {
        private readonly OrganisationRegistryConfigurationStub _organisationRegistryConfigurationStub = new()
        {
            Kbo = new KboConfigurationStub
            {
                KboV2LegalFormOrganisationClassificationTypeId = Guid.NewGuid(),
                KboV2RegisteredOfficeLocationTypeId = Guid.NewGuid(),
                KboV2FormalNameLabelTypeId = Guid.NewGuid(),
            }
        };

        private readonly OrganisationId _organisationId = new(Guid.NewGuid());
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Today);
        private DateTime _dateOfTermination;

        public ForceTerminateOrganisationAlreadyTerminatedInKbo(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override IUser User
            => new UserBuilder()
                .AddRoles(Role.AlgemeenBeheerder)
                .Build();

        protected override IEnumerable<IEvent> Given()
        {
            var fixture = new Fixture();

            _dateOfTermination = _dateTimeProviderStub.Today.AddDays(fixture.Create<int>());

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "organisation X",
                    "OVO001234",
                    "org",
                    Article.None,
                    "",
                    new List<Purpose>(),
                    false,
                    new ValidFrom(),
                    new ValidTo(),
                    new ValidFrom(),
                    new ValidTo()),
                new OrganisationTerminationFoundInKbo(
                    _organisationId,
                    fixture.Create<string>(),
                    fixture.Create<DateTime>(),
                    fixture.Create<string>(),
                    fixture.Create<string>())
            };
        }

        protected override TerminateOrganisation When()
            => new(_organisationId, _dateOfTermination, true);

        protected override TerminateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<TerminateOrganisationCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub,
                _organisationRegistryConfigurationStub);


        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsOrganisationAlreadyCoupledWithKbo()
        {
            Exception.Should().BeOfType<OrganisationAlreadyTerminatedInKbo>();
        }
    }
}
