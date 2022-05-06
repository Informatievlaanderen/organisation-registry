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
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WithAnInactiveValidity : Specification<CreateOrganisationCommandHandler, CreateOrganisation>
    {
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Today);
        private DateTime _yesterday;

        public WithAnInactiveValidity(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override IEnumerable<IEvent> Given()
        {
            _yesterday = _dateTimeProviderStub.Now.AddDays(-1);

            return new List<IEvent>();
        }

        protected override CreateOrganisation When()
            => new(
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

        protected override CreateOrganisationCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<CreateOrganisationCommandHandler>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                new UniqueOvoNumberValidatorStub(false),
                _dateTimeProviderStub);


        protected override IUser User
            => new UserBuilder().AddRoles(Role.AlgemeenBeheerder).Build();

        protected override int ExpectedNumberOfEvents
            => 1;

        [Fact]
        public void CreatesAnOrganisation()
        {
            var organisationCreated = PublishedEvents[0].UnwrapBody<OrganisationCreated>();
            organisationCreated.Should().NotBeNull();
        }
    }
}
