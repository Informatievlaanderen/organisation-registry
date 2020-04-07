namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Person
{
    using System;
    using FluentAssertions;
    using Organisation;
    using OrganisationRegistry.Organisation.Events;
    using TestBases;
    using Tests.Shared;
    using Tests.Shared.TestDataBuilders;
    using Xunit;

    [Collection(SqlServerTestsCollection.Name)]
    public class OrganisationLabelListViewTests : ListViewTestBase
    {
        [Fact]
        public void OrganisationLabelAdded()
        {
            var organisationCreated = new OrganisationCreatedTestDataBuilder(new SequentialOvoNumberGenerator()).Build();
            var organisationCoupledWithKbo = new OrganisationCoupledWithKbo(
                organisationId: organisationCreated.OrganisationId,
                kboNumber: "0123456789",
                name: organisationCreated.Name,
                ovoNumber: organisationCreated.OvoNumber,
                validFrom: DateTime.Today);

            HandleEvents(
                organisationCreated,
                organisationCoupledWithKbo);

            Context.OrganisationLabelList.Should().BeEquivalentTo(
                new OrganisationLabelListItem
                {
                    OrganisationLabelId = organisationCoupledWithKbo.OrganisationId,
                    OrganisationId = organisationCoupledWithKbo.OrganisationId,
                    LabelTypeId = OrganisationLabelListView.ArchivedNameLabelTypeId,
                    LabelTypeName = OrganisationLabelListView.ArchivedNameLabelTypeName,
                    LabelValue = organisationCoupledWithKbo.Name,
                    ValidFrom = organisationCoupledWithKbo.ValidFrom,
                    ValidTo = null,
                    Source = Sources.Kbo
                });
        }
    }
}
