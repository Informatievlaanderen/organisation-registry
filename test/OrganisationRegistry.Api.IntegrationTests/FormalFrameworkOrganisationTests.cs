namespace OrganisationRegistry.Api.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using ElasticSearch.Common;
    using ElasticSearch.Organisations;
    using FluentAssertions;
    using OrganisationRegistry.Infrastructure.Configuration;
    using Report.Responses;
    using Xunit;

    public class FormalFrameworkOrganisationTests
    {
        [Fact]
        public void HasTheCurrentMinister()
        {
            var currentlyInactiveDate = DateTime.Today.AddDays(-2);
            var currentlyActiveDate = DateTime.Today;

            var responsibleMinisterClassificationTypeId = Guid.NewGuid();

            var organisationDocument = new OrganisationDocument
            {
                OrganisationClassifications = new List<OrganisationDocument.OrganisationOrganisationClassification>
                {
                    new OrganisationDocument.OrganisationOrganisationClassification(
                        organisationOrganisationClassificationId: Guid.NewGuid(),
                        organisationClassificationTypeId: responsibleMinisterClassificationTypeId,
                        organisationClassificationTypeName: "Responsible Minister",
                        organisationClassificationId: Guid.NewGuid(),
                        organisationClassificationName: "Not the current minister",
                        validity: new Period(currentlyInactiveDate, currentlyInactiveDate)),
                    new OrganisationDocument.OrganisationOrganisationClassification(
                        organisationOrganisationClassificationId: Guid.NewGuid(),
                        organisationClassificationTypeId: responsibleMinisterClassificationTypeId,
                        organisationClassificationTypeName: "Responsible Minister",
                        organisationClassificationId: Guid.NewGuid(),
                        organisationClassificationName: "The current minister",
                        validity: new Period(currentlyActiveDate, currentlyActiveDate))
                }
            };
            var apiConfiguration = new ApiConfiguration
            {
                ResponsibleMinisterClassificationTypeId = responsibleMinisterClassificationTypeId,
                DataVlaanderenOrganisationUri = "https://example.com",
            };

            var formalFrameworkOrganisationBase =
                new FormalFrameworkOrganisationBase(
                    organisationDocument,
                    apiConfiguration,
                    Guid.NewGuid(),
                    DateTime.Today);

            formalFrameworkOrganisationBase
                .ResponsibleMinister
                .Should()
                .Be("The current minister");
        }

        [Fact]
        public void ShowsTheParentOrgOfTheFormalFramework()
        {
            var currentlyActiveDate = DateTime.Today;

            var responsibleMinisterClassificationTypeId = Guid.NewGuid();

            var parentOrganisationId = Guid.NewGuid();
            var formalFrameworkId = Guid.NewGuid();
            var organisationDocument = new OrganisationDocument
            {
                FormalFrameworks = new List<OrganisationDocument.OrganisationFormalFramework>
                {
                    new OrganisationDocument.OrganisationFormalFramework(
                        organisationFormalFrameworkId: Guid.NewGuid(),
                        formalFrameworkId: Guid.NewGuid(),
                        formalFrameworkName: "FormalFrameworkName",
                        parentOrganisationId: Guid.NewGuid(),
                        parentOrganisationName: "parentOrganisationName",
                        validity: new Period(currentlyActiveDate, currentlyActiveDate)),
                    new OrganisationDocument.OrganisationFormalFramework(
                        organisationFormalFrameworkId: Guid.NewGuid(),
                        formalFrameworkId: formalFrameworkId,
                        formalFrameworkName: "FormalFrameworkName",
                        parentOrganisationId: parentOrganisationId,
                        parentOrganisationName: "The Actual ParentOrganisationName",
                        validity: new Period(currentlyActiveDate, currentlyActiveDate))
                }
            };
            var apiConfiguration = new ApiConfiguration
            {
                ResponsibleMinisterClassificationTypeId = responsibleMinisterClassificationTypeId,
                DataVlaanderenOrganisationUri = "https://example.com",
            };

            var formalFrameworkOrganisationBase =
                new FormalFrameworkOrganisationBase(
                    organisationDocument,
                    apiConfiguration,
                    formalFrameworkId,
                    DateTime.Today);

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
