namespace OrganisationRegistry.SqlServer.IntegrationTests.OnProjections.Reports.BodyParticipation;

using System;
using System.Collections.Generic;
using System.Linq;
using Api.Backoffice.Report.BodyParticipationReport;
using Api.Backoffice.Report.Participation;
using Api.Infrastructure.Search.Filtering;
using Api.Infrastructure.Search.Sorting;
using FluentAssertions;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using OrganisationRegistry.Person;
using Reporting;
using Xunit;

public class BodyParticipationReportTests
{
    private readonly SortingHeader _sorting;
    private readonly FilteringHeader<BodyParticipationFilter> _filtering;
    private readonly OrganisationRegistryContext _context;
    private readonly Guid _bodyId;

    public BodyParticipationReportTests()
    {
        // example taken from https://organisatie.dev-vlaanderen.local/#/bodies/65404842-785b-0b2b-1b0b-bceb7b3ec4e1/participation

        _bodyId = Guid.NewGuid();
        var bodySeatType1Id = Guid.NewGuid();
        const string bodySeatType1Name = "Effectief lid";
        const bool bodySeatType1IsEffective = true;
        var bodySeatType2Id = Guid.NewGuid();
        const string bodySeatType2Name = "Ondervoorzitter";
        const bool bodySeatType2IsEffective = false;
        var bodySeatType3Id = Guid.NewGuid();
        const string bodySeatType3Name = "Voorzitter";
        const bool bodySeatType3IsEffective = false;

        var dbContextOptions = new DbContextOptionsBuilder<OrganisationRegistryContext>()
            .UseInMemoryDatabase(
                $"org-report-test-{Guid.NewGuid()}",
                _ => { }).Options;

        _context = new OrganisationRegistryContext(dbContextOptions);

        var bodySeatGenderRatioBodyItem = new BodySeatGenderRatioBodyItem
        {
            BodyId = _bodyId,
            BodyName = "Ad-hoc adviescommissie impulssubsidies",
            OrganisationId = Guid.Parse("6715d6f2-db97-427d-b093-293862670826"),
            OrganisationName = "Literatuur Vlaanderen",
            OrganisationIsActive = true,
            LifecyclePhaseValidities = new List<BodySeatGenderRatioBodyLifecyclePhaseValidityItem>
            {
                new()
                {
                    BodyId = _bodyId,
                    RepresentsActivePhase = true,
                    ValidFrom = null,
                    ValidTo = null,
                },
            },
        };
        _context.BodySeatGenderRatioBodyList.Add(bodySeatGenderRatioBodyItem);

        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Female);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Female);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Female);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Male);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Male);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Male);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType1Id, bodySeatType1Name, bodySeatType1IsEffective, Sex.Male);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType2Id, bodySeatType2Name, bodySeatType2IsEffective, Sex.Female);
        AddPost(_context, _bodyId, bodySeatGenderRatioBodyItem, bodySeatType3Id, bodySeatType3Name, bodySeatType3IsEffective, Sex.Male);

        _context.SaveChanges();

        _filtering = new FilteringHeader<BodyParticipationFilter>(new BodyParticipationFilter
        {
            EntitledToVote = true,
        });

        _sorting = new SortingHeader(string.Empty, SortOrder.Ascending);
    }

    private static void AddPost(OrganisationRegistryContext context,
        Guid bodyId,
        BodySeatGenderRatioBodyItem bodySeatGenderRatioBodyItem,
        Guid bodySeatTypeId,
        string bodySeatTypeName,
        bool bodySeatTypeIsEffective,
        Sex sex)
    {

        var bodySeatGenderRatioPostsPerTypeItem = new BodySeatGenderRatioPostsPerTypeItem
        {
            BodyId = bodyId,
            BodySeatId = Guid.NewGuid(),
            BodySeatTypeId = bodySeatTypeId,
            BodySeatTypeName = bodySeatTypeName,
            BodySeatValidFrom = null,
            BodySeatValidTo = null,
            EntitledToVote = true,
            BodySeatTypeIsEffective = bodySeatTypeIsEffective,
        };
        bodySeatGenderRatioBodyItem.PostsPerType.Add(bodySeatGenderRatioPostsPerTypeItem);

        context.BodySeatGenderRatioBodyMandateList.Add(
            new BodySeatGenderRatioBodyMandateItem
            {
                BodyId = bodySeatGenderRatioPostsPerTypeItem.BodyId,
                BodySeatTypeId = bodySeatGenderRatioPostsPerTypeItem.BodySeatTypeId,
                BodySeatId = bodySeatGenderRatioPostsPerTypeItem.BodySeatId,
                BodyMandateId = Guid.NewGuid(),
                BodyMandateValidFrom = null,
                BodyMandateValidTo = null,
                BodySeatTypeIsEffective = bodySeatTypeIsEffective,
                Assignments = new List<BodySeatGenderRatioAssignmentItem>
                {
                    new()
                    {
                        Id = Guid.NewGuid(),
                        AssignmentValidFrom = null,
                        AssignmentValidTo = null,
                        PersonId = Guid.NewGuid(),
                        DelegationAssignmentId = null,
                        Sex = sex,
                    },
                },
            });
    }

    [Fact]
    public void DocumentExistingBodyParticipationBehaviour()
    {
        var participations =
            BodyParticipation.Sort(
                    BodyParticipation.Map(
                        BodyParticipation.Search(
                            _context,
                            _bodyId,
                            _filtering,
                            DateTime.Today)),
                    _sorting)
                .ToList();

        participations.Should().BeEquivalentTo(
            new List<BodyParticipation>
            {
                new()
                {
                    BodyId = _bodyId,
                    AssignedCount = 7,
                    BodyName = "Ad-hoc adviescommissie impulssubsidies",
                    IsEffective = true,
                    IsEffectiveTranslation = "Effectief",
                    FemaleCount = 3,
                    FemalePercentage = new decimal(0.43),
                    MaleCount = 4,
                    MalePercentage = new decimal(0.57),
                    TotalCount = 7,
                    UnassignedCount = 0,
                    UnknownCount = 0,
                    UnknownPercentage = 0,
                    TotalCompliance = BodyParticipationCompliance.Compliant,
                    FemaleCompliance = BodyParticipationCompliance.Compliant,
                    MaleCompliance = BodyParticipationCompliance.Compliant,
                },
                new()
                {
                    AssignedCount = 2,
                    BodyId = _bodyId,
                    BodyName = "Ad-hoc adviescommissie impulssubsidies",
                    IsEffective = false,
                    IsEffectiveTranslation = "Plaatsvervangend",
                    FemaleCount = 1,
                    FemalePercentage = 0.5m,
                    MaleCount = 1,
                    MalePercentage = 0.5m,
                    TotalCount = 2,
                    UnassignedCount = 0,
                    UnknownCount = 0,
                    UnknownPercentage = 0,
                    TotalCompliance = BodyParticipationCompliance.Compliant,
                    FemaleCompliance = BodyParticipationCompliance.Compliant,
                    MaleCompliance = BodyParticipationCompliance.Compliant,
                },
            });
    }

    [Fact]
    public void DocumentExistingBodyParticipationTotalsBehaviour()
    {
        var participationTotals =
            BodyParticipationTotals.Map(
                BodyParticipation.Search(
                    _context,
                    _bodyId,
                    _filtering,
                    DateTime.Today));

        participationTotals.Should().BeEquivalentTo(
            new BodyParticipationTotals
            {
                AssignedCount = 9,
                FemaleCount = 4,
                FemalePercentage = new decimal(0.44),
                MaleCount = 5,
                MalePercentage = new decimal(0.56),
                TotalCount = 9,
                UnassignedCount = 0,
                UnknownCount = 0,
                UnknownPercentage = 0,
                Compliance = BodyParticipationCompliance.Compliant,
            });
    }
}
