namespace OrganisationRegistry.UnitTests.Organisation.AddOrganisationParent
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Organisation.Exceptions;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAddingAVlimpersOrganisationParentAndUserIsNotVlimpers : ExceptionSpecification<
        AddOrganisationParentCommandHandler, AddOrganisationParent>
    {
        private Guid _organisationId;
        private Guid _organisationOrganisationParentId;
        private DateTime _validTo;
        private DateTime _validFrom;
        private readonly DateTimeProviderStub _dateTimeProviderStub = new(DateTime.Now);
        private Guid _organisationParentId;

        public WhenAddingAVlimpersOrganisationParentAndUserIsNotVlimpers(ITestOutputHelper helper) : base(helper)
        {
        }

        protected override AddOrganisationParentCommandHandler BuildHandler()
            => new(
                new Mock<ILogger<AddOrganisationParentCommandHandler>>().Object,
                Session,
                _dateTimeProviderStub);

        protected override IUser User
            => new UserBuilder().Build();

        protected override IEnumerable<IEvent> Given()
        {
            _organisationOrganisationParentId = Guid.NewGuid();
            _validFrom = _dateTimeProviderStub.Today;
            _validTo = _dateTimeProviderStub.Today.AddDays(2);
            _organisationId = Guid.NewGuid();
            _organisationParentId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(
                    _organisationId,
                    "Kind en Gezin",
                    "OVO000012345",
                    "K&G",
                    Article.None,
                    "Kindjes en gezinnetjes",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null),
                new OrganisationPlacedUnderVlimpersManagement(_organisationId),
                new OrganisationCreated(
                    _organisationParentId,
                    "Ouder en Gezin",
                    "OVO000012346",
                    "O&G",
                    Article.None,
                    "Moeder",
                    new List<Purpose>(),
                    false,
                    null,
                    null,
                    null,
                    null)
            };
        }

        protected override AddOrganisationParent When()
            => new(
                _organisationOrganisationParentId,
                new OrganisationId(_organisationId),
                new OrganisationId(_organisationParentId),
                new ValidFrom(_validFrom),
                new ValidTo(_validTo));

        protected override int ExpectedNumberOfEvents
            => 0;

        [Fact]
        public void ThrowsException()
        {
            Exception.Should().BeOfType<UserIsNotAuthorizedForVlimpersOrganisations>();
        }
    }
}
