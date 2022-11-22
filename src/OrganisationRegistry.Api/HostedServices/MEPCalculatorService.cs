namespace OrganisationRegistry.Api.HostedServices;

using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Backoffice.Report.Participation;
using ElasticSearch.Bodies;
using ElasticSearch.Client;
using Infrastructure.Search.Filtering;
using Microsoft.Extensions.Logging;
using SqlServer.Infrastructure;

public class MEPCalculatorService : BackgroundService
{
    private readonly Elastic _elastic;
    private readonly OrganisationRegistryContext _context;
    private readonly IDateTimeProvider _clock;

    public MEPCalculatorService(
        ILogger logger,
        Elastic elastic,
        OrganisationRegistryContext context,
        IDateTimeProvider clock) : base(logger)
    {
        _elastic = elastic;
        _context = context;
        _clock = clock;
    }

    protected override async Task Process(CancellationToken cancellationToken)
    {
        // 1 Get All Bodies
        var bodies = await _elastic.ReadClient.SearchAsync<BodyDocument>(ct: cancellationToken);

        // 2 Foreach Body
        foreach (var body in bodies.Documents)
        {
            // 2.1 Get MEP
            var entitledToVoteResult = BodyParticipation.Search(_context, body.Id, CreateFilteringHeader(true, false), _clock.Today).ToList();
            var notEntitledToVoteResult = BodyParticipation.Search(_context, body.Id, CreateFilteringHeader(false, true), _clock.Today).ToList();
            var totalEntitledToVoteResult = BodyParticipation.Search(_context, body.Id, CreateFilteringHeader(true, true), _clock.Today).ToList();


            // 2.2 Save MEP bij Body
            var effectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => r.IsEffective!.Value);
            if (effectiveEntitledToVote is { })
            {
                body.BodyMEP.Stemgerechtigd.Effectief = MapToMEP(effectiveEntitledToVote);
            }

            var notEffectiveEntitledToVote = entitledToVoteResult.SingleOrDefault(r => !r.IsEffective!.Value || r.IsEffective == null);
            if (notEffectiveEntitledToVote is { })
            {
                body.BodyMEP.Stemgerechtigd.NietEffectief = MapToMEP(notEffectiveEntitledToVote);
            }

            var totalEntitledToVote = CombineBodyParticipations(entitledToVoteResult);
            if (totalEntitledToVote is { })
            {
                body.BodyMEP.Stemgerechtigd.Totaal = MapToMEP(totalEntitledToVote);
            }
        }

        // 3 Rebuild index
    }

    private BodyMEPEffectiviteit CombineBodyParticipations(List<BodyParticipation> entitledToVoteResult)
    {
        throw new System.NotImplementedException();
    }

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
