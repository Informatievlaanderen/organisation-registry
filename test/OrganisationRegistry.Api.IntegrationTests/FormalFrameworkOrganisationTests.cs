namespace OrganisationRegistry.Api.IntegrationTests
{
    using System;
    using System.Collections.Generic;
    using Configuration;
    using ElasticSearch.Common;
    using ElasticSearch.Organisations;
    using FluentAssertions;
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
                        Guid.NewGuid(),
                        responsibleMinisterClassificationTypeId,
                        "Responsible Minister",
                        Guid.NewGuid(),
                        "Not the current minister",
                        new Period(currentlyInactiveDate, currentlyInactiveDate)),
                    new OrganisationDocument.OrganisationOrganisationClassification(
                        Guid.NewGuid(),
                        responsibleMinisterClassificationTypeId,
                        "Responsible Minister",
                        Guid.NewGuid(),
                        "The current minister",
                        new Period(currentlyActiveDate, currentlyActiveDate))
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
                    apiConfiguration, DateTime.Today);

            formalFrameworkOrganisationBase
                .ResponsibleMinister
                .Should()
                .Be("The current minister");
        }
    }
}
