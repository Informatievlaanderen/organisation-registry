namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.OrganisationFormalFramework
{
    using System;
    using System.Linq;
    using FluentAssertions;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using OrganisationRegistry.Organisation.Events;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class OrganisationFormalFrameworkListViewTests : ListViewTestBase
    {
        private readonly DateTime _yesterday = DateTime.Today.AddDays(-1);
        private readonly SequentialOvoNumberGenerator _sequentialOvoNumberGenerator = new SequentialOvoNumberGenerator();

        [Fact]
        public void WhenAssigningAFormalFramework()
        {
            var childOrganisationCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedBuilder();
            var formalFrameworkACreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkBCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkAddedToChild =
                new OrganisationFormalFrameworkAddedBuilder(childOrganisationCreated.Id, formalFrameworkACreated.Id, parentOrganisationACreated.Id);
            var parentAssignedToChild =
                new FormalFrameworkAssignedToOrganisationBuilder(
                    formalFrameworkAddedToChild.OrganisationFormalFrameworkId, formalFrameworkACreated.Id, childOrganisationCreated.Id, parentOrganisationACreated.Id);


            HandleEvents(
                childOrganisationCreated.Build(),
                parentOrganisationACreated.Build(),
                parentOrganisationBCreated.Build(),
                formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                formalFrameworkACreated.Build(),
                formalFrameworkBCreated.Build(),
                formalFrameworkAddedToChild.Build(),
                parentAssignedToChild.Build()
            );

            var organisationsForFormalFrameworkA =
                Context.OrganisationList
                    .AsQueryable()
                    .Where(item => item.FormalFrameworkId == formalFrameworkACreated.Id);

            organisationsForFormalFrameworkA.Should().HaveCount(2);

            var parent = organisationsForFormalFrameworkA.SingleOrDefault(item => item.Name == parentOrganisationACreated.Name);
            parent.Should().NotBeNull();
            parent.OrganisationId.Should().Be((Guid)parentOrganisationACreated.Id);

            var child = organisationsForFormalFrameworkA.SingleOrDefault(item => item.OrganisationId == childOrganisationCreated.Id);
            child.Should().NotBeNull();
            child.ParentOrganisationId.Should().Be(parentOrganisationACreated.Id);
        }

        [Fact]
        public void WhenChangingAnActiveFormalFrameworkToAnInactiveOne()
        {
            var childOrganisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var childOrganisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationACreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationBCreated = new OrganisationCreatedBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedBuilder();
            var formalFrameworkACreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkBCreated = new FormalFrameworkCreatedBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);

            var formalFrameworkAWithParentAAddedToChildA =
                new OrganisationFormalFrameworkAddedBuilder(childOrganisationACreated.Id, formalFrameworkACreated.Id, parentOrganisationACreated.Id);

            var parentAAssignedToChildAForFormalFrameworkA =
                new FormalFrameworkAssignedToOrganisationBuilder(
                    formalFrameworkAWithParentAAddedToChildA.OrganisationFormalFrameworkId, formalFrameworkACreated.Id, childOrganisationACreated.Id, parentOrganisationACreated.Id);

            var formalFrameworkBWithParentAAddedToChildB =
                new OrganisationFormalFrameworkAddedBuilder(childOrganisationBCreated.Id, formalFrameworkBCreated.Id, parentOrganisationACreated.Id);

            var parentAAssignedToChildBForFormalFrameworkB =
                new FormalFrameworkAssignedToOrganisationBuilder(
                    formalFrameworkBWithParentAAddedToChildB.OrganisationFormalFrameworkId, formalFrameworkBCreated.Id, childOrganisationBCreated.Id, parentOrganisationACreated.Id);

            var formalFrameworkForChildAUpdatedToInactive =
                new OrganisationFormalFrameworkUpdatedBuilder(
                    formalFrameworkAWithParentAAddedToChildA.OrganisationFormalFrameworkId,
                    childOrganisationACreated.Id,
                    formalFrameworkACreated.Id,
                    parentOrganisationACreated.Id,
                    parentOrganisationACreated.Id).WithValidity(_yesterday, _yesterday);

            var formalFrameworkClearedForChildA =
                new FormalFrameworkClearedFromOrganisation(
                    formalFrameworkAWithParentAAddedToChildA.OrganisationFormalFrameworkId,
                    childOrganisationACreated.Id,
                    formalFrameworkACreated.Id,
                    parentOrganisationACreated.Id);


            HandleEvents(
                childOrganisationACreated.Build(),
                childOrganisationBCreated.Build(),
                parentOrganisationACreated.Build(),
                parentOrganisationBCreated.Build(),
                formalFrameworkCategoryCreatedTestDataBuilder.Build(),
                formalFrameworkACreated.Build(),
                formalFrameworkBCreated.Build(),
                formalFrameworkAWithParentAAddedToChildA.Build(),
                parentAAssignedToChildAForFormalFrameworkA.Build(),
                formalFrameworkBWithParentAAddedToChildB.Build(),
                parentAAssignedToChildBForFormalFrameworkB.Build(),
                formalFrameworkForChildAUpdatedToInactive.Build(),
                formalFrameworkClearedForChildA
            );

            var organisationsForFormalFrameworkA =
                Context.OrganisationList
                    .AsQueryable()
                    .Where(item => item.FormalFrameworkId == formalFrameworkACreated.Id);

            organisationsForFormalFrameworkA.Should().HaveCount(2);

            var organisationsForFormalFrameworkB = Context.OrganisationList
                .AsQueryable()
                .Where(item => item.FormalFrameworkId == formalFrameworkBCreated.Id);

            organisationsForFormalFrameworkB.Should().HaveCount(2);
        }
    }
}
