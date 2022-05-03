namespace OrganisationRegistry.UnitTests.Organisation.RemoveOrganisationKey
{
    using System;
    using System.Collections.Generic;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using OrganisationRegistry.Infrastructure.Events;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Tests.Shared;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using Xunit.Abstractions;

    public class WhenRemovingAnOrganisationKeyToAnUncoupledKey : Specification<Organisation, OrganisationCommandHandlers, RemoveOrganisationKey>
    {
        private Guid _organisationId;
        private Guid _organisationKeyId;

        protected override OrganisationCommandHandlers BuildHandler()
        {
            var securityServiceMock = new Mock<ISecurityService>();
            securityServiceMock.Setup(service =>
                    service.CanUseKeyType(
                        It.IsAny<IUser>(),
                        It.IsAny<Guid>()))
                .Returns(true);

            return new OrganisationCommandHandlers(
                new Mock<ILogger<OrganisationCommandHandlers>>().Object,
                Session,
                new SequentialOvoNumberGenerator(),
                null!,
                new DateTimeProvider(),
                Mock.Of<IOrganisationRegistryConfiguration>(),
                securityServiceMock.Object);
        }

        protected override IEnumerable<IEvent> Given()
        {
            _organisationId = Guid.NewGuid();
            _organisationKeyId = Guid.NewGuid();

            return new List<IEvent>
            {
                new OrganisationCreated(_organisationId, "Kind en Gezin", "OVO000012345", "K&G", Article.None, "Kindjes en gezinnetjes", new List<Purpose>(), false, null, null, null, null)
            };
        }

        protected override RemoveOrganisationKey When()
            => new(
                new OrganisationId(_organisationId),
                new OrganisationKeyId(_organisationKeyId)
            )
            {
                User = new UserBuilder()
                    .AddRoles(Role.AlgemeenBeheerder)
                    .Build()
            };

        protected override int ExpectedNumberOfEvents => 0;

        public WhenRemovingAnOrganisationKeyToAnUncoupledKey(ITestOutputHelper helper) : base(helper) { }
    }
}
