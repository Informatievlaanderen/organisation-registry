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

        public OrganisationFormalFrameworkListViewTests(SqlServerFixture fixture) : base(fixture)
        {
        }

        [Fact]
        public void WhenAssigningAFormalFramework()
        {
            var childOrganisationCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationACreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationBCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            var formalFrameworkACreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkBCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkAddedToChild =
                new OrganisationFormalFrameworkAddedTestDataBuilder(childOrganisationCreated.Id, formalFrameworkACreated.Id, parentOrganisationACreated.Id);
            var parentAssignedToChild =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
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

            var organisationsForFormalFrameworkA = Context.OrganisationList.Where(item => item.FormalFrameworkId == formalFrameworkACreated.Id);
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
            var childOrganisationACreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var childOrganisationBCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationACreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var parentOrganisationBCreated = new OrganisationCreatedTestDataBuilder(_sequentialOvoNumberGenerator);
            var formalFrameworkCategoryCreatedTestDataBuilder = new FormalFrameworkCategoryCreatedTestDataBuilder();
            var formalFrameworkACreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);
            var formalFrameworkBCreated = new FormalFrameworkCreatedTestDataBuilder(formalFrameworkCategoryCreatedTestDataBuilder.Id, formalFrameworkCategoryCreatedTestDataBuilder.Name);

            var formalFrameworkAWithParentAAddedToChildA =
                new OrganisationFormalFrameworkAddedTestDataBuilder(childOrganisationACreated.Id, formalFrameworkACreated.Id, parentOrganisationACreated.Id);

            var parentAAssignedToChildAForFormalFrameworkA =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    formalFrameworkAWithParentAAddedToChildA.OrganisationFormalFrameworkId, formalFrameworkACreated.Id, childOrganisationACreated.Id, parentOrganisationACreated.Id);

            var formalFrameworkBWithParentAAddedToChildB =
                new OrganisationFormalFrameworkAddedTestDataBuilder(childOrganisationBCreated.Id, formalFrameworkBCreated.Id, parentOrganisationACreated.Id);

            var parentAAssignedToChildBForFormalFrameworkB =
                new FormalFrameworkAssignedToOrganisationTestDataBuilder(
                    formalFrameworkBWithParentAAddedToChildB.OrganisationFormalFrameworkId, formalFrameworkBCreated.Id, childOrganisationBCreated.Id, parentOrganisationACreated.Id);

            var formalFrameworkForChildAUpdatedToInactive =
                new OrganisationFormalFrameworkUpdatedTestDataBuilder(
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

            var organisationsForFormalFrameworkA = Context.OrganisationList.Where(item => item.FormalFrameworkId == formalFrameworkACreated.Id);
            organisationsForFormalFrameworkA.Should().HaveCount(2);

            var organisationsForFormalFrameworkB = Context.OrganisationList.Where(item => item.FormalFrameworkId == formalFrameworkBCreated.Id);
            organisationsForFormalFrameworkB.Should().HaveCount(2);
        }
    }
}
