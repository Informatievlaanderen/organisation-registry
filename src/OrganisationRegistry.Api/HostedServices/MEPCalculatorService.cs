namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Backoffice.Report.BodyParticipationReport;
using Backoffice.Report.Participation;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using Infrastructure.Search.Filtering;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Configuration;
using SqlServer;

public class MEPCalculatorService : BackgroundService
{
    private readonly Elastic _elastic;
    private readonly IContextFactory _contextFactory;
    private readonly IDateTimeProvider _clock;
    private readonly HostedServiceConfiguration _configuration;

    public MEPCalculatorService(
        ILogger<MEPCalculatorService> logger,
        IOrganisationRegistryConfiguration configuration,
        Elastic elastic,
        IContextFactory contextFactory,
        IDateTimeProvider clock) : base(logger)
    {
        _configuration = configuration.HostedServices.MEPCalculatorService;
        _elastic = elastic;
        _contextFactory = contextFactory;
        _clock = clock;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        if (!_configuration.Enabled)
        {
            Logger.LogInformation($"{nameof(MEPCalculatorService)} disabled, skipping execution");
            return;
        }

        Logger.LogInformation("Starting MEP Calculator service");

        while (!cancellationToken.IsCancellationRequested)
        {
            await ProcessBodies(cancellationToken);

            await _configuration.Delay(cancellationToken);
        }
    }

    protected async Task ProcessBodies(CancellationToken cancellationToken)
    {
        await using var context = _contextFactory.Create();
        var bodies = await _elastic.ReadClient.SearchAsync<BodyDocument>(descriptor => descriptor.Size(10000), cancellationToken);

        Logger.LogInformation("Found {NumberOfBodies} bodies to process", bodies.Documents.Count);

        foreach (var body in bodies.Documents)
        {
            try
            {
                var entitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(true, false), _clock.Today).ToList();
                var notEntitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(false, true), _clock.Today).ToList();
                var totalEntitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(true, true), _clock.Today).ToList();

                body.MEP.EntitledToVote = MapMaybeBodyParticipations(entitledToVoteResult);
                body.MEP.NotEntitledToVote = MapMaybeBodyParticipations(notEntitledToVoteResult);
                body.MEP.Total = MapMaybeBodyParticipations(totalEntitledToVoteResult);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Something went wrong processing bodies for MEP calculation");

                body.MEP.EntitledToVote = new BodyMEPVotingEligibility();
                body.MEP.NotEntitledToVote = new BodyMEPVotingEligibility();
                body.MEP.Total = new BodyMEPVotingEligibility();
            }

            await _elastic.ReadClient.IndexDocumentAsync(body, cancellationToken);
        }
        Logger.LogInformation("Done processing bodies");
    }

    private static BodyMEPVotingEligibility MapMaybeBodyParticipations(IList<BodyParticipation> bodyParticipations)
    {
        var result = new BodyMEPVotingEligibility();

        var effectiveEntitledToVote = bodyParticipations.SingleOrDefault(r => r.IsEffective!.Value);
        result.Effective = effectiveEntitledToVote is { }
            ? MapToMEP(effectiveEntitledToVote)
            : new BodyMEPEffectivity();

        var notEffectiveEntitledToVote = bodyParticipations.SingleOrDefault(r => !r.IsEffective!.Value || r.IsEffective == null);
        result.NotEffective = notEffectiveEntitledToVote is { }
            ? MapToMEP(notEffectiveEntitledToVote)
            : new BodyMEPEffectivity();

        var totalEntitledToVote = BodyParticipationTotals.Map(bodyParticipations);
        result.Total = MapToMEPTotals(totalEntitledToVote);

        return result;
    }

    private static BodyMEPEffectivity MapToMEP(BodyParticipation bodyParticipation)
    {
        bodyParticipation = BodyParticipation.Map(bodyParticipation);
        return new BodyMEPEffectivity
        {
            MaleCount = bodyParticipation.MaleCount,
            UnknownCount = bodyParticipation.UnknownCount,
            TotalSeatCount = bodyParticipation.TotalCount,
            FemaleCount = bodyParticipation.FemaleCount,
            MalePercentage = bodyParticipation.MalePercentage,
            UnknownPercentage = bodyParticipation.UnknownPercentage,
            FemalePercentage = bodyParticipation.FemalePercentage,
            AssignedSeatCount = bodyParticipation.AssignedCount,
            MEPCompliance = CalculateMEPCompliance(bodyParticipation.TotalCompliance, bodyParticipation.TotalCount, bodyParticipation.AssignedCount),
        };
    }

    private static MEPCompliance CalculateMEPCompliance(BodyParticipationCompliance complianceToCheck, int seatCount, int assignedSeats)
        => complianceToCheck switch
        {
            BodyParticipationCompliance.Unknown when seatCount == 0 => MEPCompliance.NoSeats,
            BodyParticipationCompliance.Unknown when seatCount != assignedSeats => MEPCompliance.NotAllSeatsAssigned,
            BodyParticipationCompliance.Unknown => MEPCompliance.NotCompliant,
            BodyParticipationCompliance.NonCompliant => MEPCompliance.NotCompliant,
            BodyParticipationCompliance.Compliant => MEPCompliance.Compliant,
            _ => throw new ArgumentOutOfRangeException(nameof(complianceToCheck), complianceToCheck, null),
        };

    private static BodyMEPEffectivity MapToMEPTotals(BodyParticipationTotals bodyParticipationTotals)
        => new()
        {
            MaleCount = bodyParticipationTotals.MaleCount,
            UnknownCount = bodyParticipationTotals.UnknownCount,
            TotalSeatCount = bodyParticipationTotals.TotalCount,
            FemaleCount = bodyParticipationTotals.FemaleCount,
            MalePercentage = bodyParticipationTotals.MalePercentage,
            UnknownPercentage = bodyParticipationTotals.UnknownPercentage,
            FemalePercentage = bodyParticipationTotals.FemalePercentage,
            AssignedSeatCount = bodyParticipationTotals.AssignedCount,
            MEPCompliance = CalculateTotalMEPCompliance(bodyParticipationTotals.Compliance, bodyParticipationTotals.TotalCount, bodyParticipationTotals.AssignedCount),
        };

    private static MEPCompliance CalculateTotalMEPCompliance(BodyParticipationCompliance complianceToCheck, int seatCount, int assignedSeats)
    {
        if (seatCount == 0)
            return MEPCompliance.NoSeats;
        if (seatCount != assignedSeats)
            return MEPCompliance.NotAllSeatsAssigned;

        return complianceToCheck switch
        {
            BodyParticipationCompliance.NonCompliant => MEPCompliance.NotCompliant,
            BodyParticipationCompliance.Compliant => MEPCompliance.Compliant,
            BodyParticipationCompliance.Unknown => throw new ArgumentOutOfRangeException(nameof(complianceToCheck), complianceToCheck, null),
            _ => throw new ArgumentOutOfRangeException(nameof(complianceToCheck), complianceToCheck, null),
        };
    }

    private static FilteringHeader<BodyParticipationFilter> CreateFilteringHeader(bool entitledToVote, bool notEntitledToVote)
        => new(
            new BodyParticipationFilter
            {
                EntitledToVote = entitledToVote,
                NotEntitledToVote = notEntitledToVote,
            });
}
