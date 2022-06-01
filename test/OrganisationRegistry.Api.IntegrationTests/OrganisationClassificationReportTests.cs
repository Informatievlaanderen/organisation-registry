namespace OrganisationRegistry.Api.IntegrationTests;

using System;
using System.Collections.Generic;
using System.Linq;
using Backoffice.Report.OrganisationClassificationReport;
using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer.Body;
using SqlServer.Infrastructure;
using SqlServer.Reporting;
using Xunit;

public class OrganisationClassificationReportTests
{
    [Fact]
    public void DoesNotIncludeInactiveClassifications()
    {
        var currentlyActiveDate = DateTime.Today;

        var context = new OrganisationRegistryContext(
            new DbContextOptionsBuilder<OrganisationRegistryContext>()
                .UseInMemoryDatabase(
                    "org-classifications-test",
                    _ => { }).Options);

        var organisationClassificationId = Guid.NewGuid();
        var mepFormalFrameworkId = Guid.NewGuid();
        var apiConfiguration = new ApiConfigurationSection
        {
            Mep_FormalFrameworkId = mepFormalFrameworkId
        };

        var activeItemOrganisationId = AddItem(context, currentlyActiveDate, organisationClassificationId, mepFormalFrameworkId);

        context.SaveChanges();

        var classificationOrganisationParticipations =
            ClassificationOrganisationParticipation.Search(
                    context,
                    organisationClassificationId,
                    apiConfiguration,
                    DateTime.Today)
                .ToList();

        classificationOrganisationParticipations
            .Should()
            .HaveCount(1);

        classificationOrganisationParticipations
            .Single()
            .OrganisationId
            .Should()
            .Be(activeItemOrganisationId);
    }

    private static Guid AddItem(
        OrganisationRegistryContext context,
        DateTime currentlyActiveDate,
        Guid organisationClassificationId,
        Guid mepFormalFrameworkId)
    {
        Guid bodyId = Guid.NewGuid();
        Guid organisationId = Guid.NewGuid();
        context
            .BodySeatGenderRatioOrganisationClassificationList
            .Add(
                new BodySeatGenderRatioOrganisationClassificationItem
                {
                    ClassificationValidFrom = currentlyActiveDate,
                    ClassificationValidTo = currentlyActiveDate,
                    OrganisationClassificationId = organisationClassificationId,
                    OrganisationId = organisationId,
                    OrganisationClassificationTypeId = Guid.NewGuid(),
                    OrganisationOrganisationClassificationId = Guid.NewGuid()
                });

        context
            .BodySeatGenderRatioOrganisationPerBodyList
            .Add(
                new BodySeatGenderRatioOrganisationPerBodyListItem
                {
                    BodyId = bodyId,
                    OrganisationActive = true,
                    OrganisationId = organisationId,
                    OrganisationName = Guid.NewGuid().ToString(),
                    BodyOrganisationId = Guid.NewGuid()
                });

        context
            .BodyFormalFrameworkList
            .Add(
                new BodyFormalFrameworkListItem
                {
                    BodyId = bodyId,
                    ValidFrom = currentlyActiveDate,
                    ValidTo = currentlyActiveDate,
                    FormalFrameworkId = mepFormalFrameworkId,
                    FormalFrameworkName = Guid.NewGuid().ToString(),
                    BodyFormalFrameworkId = Guid.NewGuid(),
                });

        context
            .BodySeatGenderRatioBodyList
            .Add(
                new BodySeatGenderRatioBodyItem
                {
                    BodyId = bodyId,
                    BodyName = Guid.NewGuid().ToString(),
                    OrganisationId = organisationId,
                    OrganisationName = Guid.NewGuid().ToString(),
                    LifecyclePhaseValidities = new List<BodySeatGenderRatioBodyLifecyclePhaseValidityItem>
                    {
                        new BodySeatGenderRatioBodyLifecyclePhaseValidityItem
                        {
                            RepresentsActivePhase = true,
                            BodyId = bodyId,
                            ValidFrom = currentlyActiveDate,
                            ValidTo = currentlyActiveDate,
                            LifecyclePhaseId = Guid.NewGuid()
                        }
                    },
                    OrganisationIsActive = true,
                    PostsPerType = new List<BodySeatGenderRatioPostsPerTypeItem>()
                    {
                        new BodySeatGenderRatioPostsPerTypeItem
                        {
                            BodySeatValidFrom = currentlyActiveDate,
                            BodySeatValidTo = currentlyActiveDate
                        }
                    },
                });
        return organisationId;
    }
}
