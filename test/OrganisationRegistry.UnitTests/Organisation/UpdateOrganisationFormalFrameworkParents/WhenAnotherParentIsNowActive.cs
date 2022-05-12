namespace OrganisationRegistry.UnitTests.Organisation.UpdateOrganisationFormalFrameworkParents
{
    using System;
    using System.Collections.Generic;
    using FluentAssertions;
    using FormalFramework;
    using Infrastructure.Tests.Extensions.TestHelpers;
    using Microsoft.Extensions.Logging;
    using Moq;
    using OrganisationRegistry.Infrastructure.Authorization;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;
    using Xunit.Abstractions;

    public class WhenAnotherParentIsNowActive: Specification<UpdateOrganisationFormalFrameworkParentsCommandHandler, UpdateOrganisationFormalFrameworkParents>
    {
        private readonly DateTimeProviderStub _dateTimeProvider = new(DateTime.Now);
        private Guid _organisationCreatedId;
        private Guid _parentOrganisationCreatedId;
        private Guid _formalFrameworkCreatedId;

        public WhenAnotherParentIsNowActive(ITestOutputHelper helper) : base(helper) { }

        protected override IUser User
            => new UserBuilder().Build();

        protected override IEnumerable<IEvent> Given()
        {
            var sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

            var organisationCreated = new OrganisationCreatedBuilder(sequentialOvoNumberGenerator);
            _organisationCreatedId = organisationCreated.Id;

            var parentOrganisationCreated = new OrganisationCreatedBuilder(sequentialOvoNumberGenerator);
            _parentOrganisationCreatedId = parentOrganisationCreated.Id;

            var newParentOrganisationCreated = new OrganisationCreatedBuilder(sequentialOvoNumberGenerator);

            var formalFrameworkCategoryCreated = new FormalFrameworkCategoryCreatedBuilder();
            var formalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);
            _formalFrameworkCreatedId = formalFrameworkCreated.Id;

            var anotherFormalFrameworkCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreated.Id, formalFrameworkCategoryCreated.Name);

            var organisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        organisationCreated.Id,
                        formalFrameworkCreated.Id,
                        parentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-2), _dateTimeProvider.Today.AddDays(-1));

            var newOrganisationFormalFrameworkAdded =
                new OrganisationFormalFrameworkAddedBuilder(
                        organisationCreated.Id,
                        anotherFormalFrameworkCreated.Id,
                        newParentOrganisationCreated.Id)
                    .WithValidity(_dateTimeProvider.Today.AddDays(-1), _dateTimeProvider.Today.AddDays(0));

            var formalFrameworkAssignedToOrganisation = new FormalFrameworkAssignedToOrganisationBuilder(
                organisationFormalFrameworkAdded.OrganisationFormalFrameworkId,
                organisationFormalFrameworkAdded.FormalFrameworkId,
                organisationFormalFrameworkAdded.OrganisationId,
                organisationFormalFrameworkAdded.ParentOrganisationId);

            return new List<IEvent>
            {
                organisationCreated.Build(),
                parentOrganisationCreated.Build(),
                newParentOrganisationCreated.Build(),
                formalFrameworkCategoryCreated.Build(),
                formalFrameworkCreated.Build(),
                organisationFormalFrameworkAdded.Build(),
                newOrganisationFormalFrameworkAdded.Build(),
                formalFrameworkAssignedToOrganisation.Build()
            };
        }

        protected override UpdateOrganisationFormalFrameworkParents When()
            => new(new OrganisationId(_organisationCreatedId), new FormalFrameworkId(_formalFrameworkCreatedId));

        protected override UpdateOrganisationFormalFrameworkParentsCommandHandler BuildHandler()
            => new(
                Mock.Of<ILogger<UpdateOrganisationFormalFrameworkParentsCommandHandler>>(),
                Session,
                _dateTimeProvider);

        protected override int ExpectedNumberOfEvents => 1;

        [Fact]
        public void PublishesFormalFrameworkClearedFromOrganisation()
        {
            var @event = PublishedEvents[0];
            @event.Should().BeOfType<Envelope<FormalFrameworkClearedFromOrganisation>>();
            var formalFrameworkClearedFromOrganisation = @event.UnwrapBody<FormalFrameworkClearedFromOrganisation>();
            formalFrameworkClearedFromOrganisation.OrganisationId.Should().Be(_organisationCreatedId);
            formalFrameworkClearedFromOrganisation.FormalFrameworkId.Should().Be(_formalFrameworkCreatedId);
            formalFrameworkClearedFromOrganisation.ParentOrganisationId.Should().Be(_parentOrganisationCreatedId);
        }
    }
}
