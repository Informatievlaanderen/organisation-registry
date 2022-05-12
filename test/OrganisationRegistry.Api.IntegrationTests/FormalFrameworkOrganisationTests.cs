namespace OrganisationRegistry.Api.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Backoffice.Report.FormalFrameworkOrganisationReport;
    using ElasticSearch.Common;
    using ElasticSearch.Organisations;
    using FluentAssertions;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Xunit;

    public class FormalFrameworkOrganisationTests
    {
        [Fact]
        public void HasTheCurrentMinister()
        {
            var today = new DateTime(2022, 2, 2);
            var aDateInThePast = today.AddDays(-2);

            var responsibleMinisterClassificationTypeId = Guid.NewGuid();

            var organisationDocument = new OrganisationDocument
            {
                OrganisationClassifications = new List<OrganisationDocument.OrganisationOrganisationClassification>
                {
                    new(
                        organisationOrganisationClassificationId: Guid.NewGuid(),
                        responsibleMinisterClassificationTypeId,
                        "Responsible Minister",
                        organisationClassificationId: Guid.NewGuid(),
                        "Not the current minister",
                        validity: Period.FromDates(aDateInThePast, aDateInThePast)),
                    new(
                        organisationOrganisationClassificationId: Guid.NewGuid(),
                        responsibleMinisterClassificationTypeId,
                        "Responsible Minister",
                        organisationClassificationId: Guid.NewGuid(),
                        "The current minister",
                        validity: Period.FromDates(today, today))
                }
            };
            var apiConfiguration = new ApiConfigurationSection
            {
                ResponsibleMinisterClassificationTypeId = responsibleMinisterClassificationTypeId,
                DataVlaanderenOrganisationUri = "https://example.com",
            };

            var formalFrameworkOrganisationBase =
                new FormalFrameworkOrganisationBase(
                    organisationDocument,
                    apiConfiguration,
                    today);

            formalFrameworkOrganisationBase
                .ResponsibleMinister
                .Should()
                .Be("The current minister");
        }

        [Fact]
        public void ShowsTheParentOrgOfTheFormalFramework()
        {
            var today = new DateTime(2022, 2, 2);
            var aDayInThePast = today.AddDays(-50);
            var theNextDayInThePast = today.AddDays(-49);

            var responsibleMinisterClassificationTypeId = Guid.NewGuid();

            var parentOrganisationId = Guid.NewGuid();
            var organisationDocument = new OrganisationDocument
            {
                Parents = new List<OrganisationDocument.OrganisationParent>
                {
                    new OrganisationDocument.OrganisationParent(
                        organisationOrganisationParentId: Guid.NewGuid(),
                        parentOrganisationId: Guid.NewGuid(),
                        parentOrganisationName: "Some other organisations parent parent",
                        validity: Period.FromDates(null, aDayInThePast)),
                    new OrganisationDocument.OrganisationParent(
                        organisationOrganisationParentId: Guid.NewGuid(),
                        parentOrganisationId: parentOrganisationId,
                        parentOrganisationName: "The Actual ParentOrganisationName",
                        validity: Period.FromDates(theNextDayInThePast, today)),
                }
            };
            var apiConfiguration = new ApiConfigurationSection
            {
                ResponsibleMinisterClassificationTypeId = responsibleMinisterClassificationTypeId,
                DataVlaanderenOrganisationUri = "https://example.com",
            };

            var formalFrameworkOrganisationBase =
                new FormalFrameworkOrganisationBase(
                    organisationDocument,
                    apiConfiguration,
                    today);

            formalFrameworkOrganisationBase
                .ParentOrganisationId
                .Should()
                .Be(parentOrganisationId);

            formalFrameworkOrganisationBase
                .ParentOrganisationName
                .Should()
                .Be("The Actual ParentOrganisationName");
        }
    }
}
