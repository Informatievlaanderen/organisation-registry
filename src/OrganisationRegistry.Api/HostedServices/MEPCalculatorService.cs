namespace OrganisationRegistry.Api.HostedServices;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
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

                body.MEP.EntitledToVote = MapMaybeEntitledToVoteResult(entitledToVoteResult);
                body.MEP.NotEntitledToVote = MapMaybeEntitledToVoteResult(notEntitledToVoteResult);
                body.MEP.Total = MapMaybeEntitledToVoteResult(totalEntitledToVoteResult);
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

    private static BodyMEPVotingEligibility MapMaybeEntitledToVoteResult(IList<BodyParticipation> entitledToVoteResult)
    {
        var result = new BodyMEPVotingEligibility();

        var effectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => r.IsEffective!.Value);
        result.Effective = effectiveEntitledToVote is { }
            ? MapToMEP(effectiveEntitledToVote)
            : new BodyMEPEffectivity();

        var notEffectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => !r.IsEffective!.Value || r.IsEffective == null);
        result.NotEffective = notEffectiveEntitledToVote is { }
            ? MapToMEP(notEffectiveEntitledToVote)
            : new BodyMEPEffectivity();

        var totalEntitledToVote = CombineBodyParticipations(entitledToVoteResult);
        result.Total = MapToMEP(totalEntitledToVote);

        return result;
    }

    private static BodyParticipation CombineBodyParticipations(IEnumerable<BodyParticipation> list)
        => list
            .Aggregate(
                new BodyParticipation(),
                (total, current) =>
                {
                    total.AssignedCount += current.AssignedCount;
                    total.FemaleCount += current.FemaleCount;
                    total.MaleCount += current.MaleCount;
                    total.UnknownCount += current.UnknownCount;
                    total.TotalCount += current.TotalCount;

                    return total;
                });

    private static BodyMEPEffectivity MapToMEP(BodyParticipation effectiveEntitledToVote)
    {
        effectiveEntitledToVote = BodyParticipation.Map(effectiveEntitledToVote);
        return new BodyMEPEffectivity
        {
            MaleCount = effectiveEntitledToVote.MaleCount,
            UnknownCount = effectiveEntitledToVote.UnknownCount,
            TotalSeatCount = effectiveEntitledToVote.TotalCount,
            FemaleCount = effectiveEntitledToVote.FemaleCount,
            MalePercentage = effectiveEntitledToVote.MalePercentage,
            UnknownPercentage = effectiveEntitledToVote.UnknownPercentage,
            FemalePercentage = effectiveEntitledToVote.FemalePercentage,
            AssignedSeatCount = effectiveEntitledToVote.AssignedCount,
            MEPCompliance = effectiveEntitledToVote.TotalCompliance == BodyParticipationCompliance.Compliant,
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
