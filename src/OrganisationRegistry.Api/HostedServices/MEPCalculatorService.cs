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
        var bodies = await _elastic.ReadClient.SearchAsync<BodyDocument>(ct: cancellationToken);

        Logger.LogInformation("Found {NumberOfBodies} to process", bodies.Documents.Count);

        foreach (var body in bodies.Documents)
        {
            try
            {
                var entitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(true, false), _clock.Today).ToList();
                var notEntitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(false, true), _clock.Today).ToList();
                var totalEntitledToVoteResult = BodyParticipation.Search(context, body.Id, CreateFilteringHeader(true, true), _clock.Today).ToList();

                body.MEP.Stemgerechtigd = MapMaybeEntitledToVoteResult(entitledToVoteResult);
                body.MEP.NietStemgerechtigd = MapMaybeEntitledToVoteResult(notEntitledToVoteResult);
                body.MEP.Totaal = MapMaybeEntitledToVoteResult(totalEntitledToVoteResult);
            }
            catch (Exception ex)
            {
                Logger.LogError(ex, "Something went wrong processing bodies for MEP calculation");

                body.MEP.Stemgerechtigd = new BodyMEPStemgerechtigdheid();
                body.MEP.NietStemgerechtigd = new BodyMEPStemgerechtigdheid();
                body.MEP.Totaal = new BodyMEPStemgerechtigdheid();
            }

            await _elastic.ReadClient.IndexDocumentAsync(body, cancellationToken);
        }
        Logger.LogInformation("Done processing bodies");
    }

    private static BodyMEPStemgerechtigdheid MapMaybeEntitledToVoteResult(IList<BodyParticipation> entitledToVoteResult)
    {
        var result = new BodyMEPStemgerechtigdheid();

        var effectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => r.IsEffective!.Value);
        result.Effectief = effectiveEntitledToVote is { }
            ? MapToMEP(effectiveEntitledToVote)
            : new BodyMEPEffectiviteit();

        var notEffectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => !r.IsEffective!.Value || r.IsEffective == null);
        result.NietEffectief = notEffectiveEntitledToVote is { }
            ? MapToMEP(notEffectiveEntitledToVote)
            : new BodyMEPEffectiviteit();

        var totalEntitledToVote = CombineBodyParticipations(entitledToVoteResult);
        result.Totaal = MapToMEP(totalEntitledToVote);

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

    private static BodyMEPEffectiviteit MapToMEP(BodyParticipation effectiveEntitledToVote)
    {
        effectiveEntitledToVote = BodyParticipation.Map(effectiveEntitledToVote);
        return new BodyMEPEffectiviteit
        {
            AantalMannen = effectiveEntitledToVote.MaleCount,
            AantalOnbekend = effectiveEntitledToVote.UnknownCount,
            AantalPosten = effectiveEntitledToVote.TotalCount,
            AantalVrouwen = effectiveEntitledToVote.FemaleCount,
            ProcentMannen = effectiveEntitledToVote.MalePercentage,
            ProcentOnbekend = effectiveEntitledToVote.UnknownPercentage,
            ProcentVrouwen = effectiveEntitledToVote.FemalePercentage,
            AantalToegewezenPosten = effectiveEntitledToVote.AssignedCount,
            MEPCompliant = effectiveEntitledToVote.TotalCompliance == BodyParticipationCompliance.Compliant,
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
