namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer.Infrastructure;

    public class IndividualRebuildRunner
    {
        public string ProjectionName => "IndividualRebuild";

        private readonly ILogger<IndividualRebuildRunner> _logger;
        private readonly IEventStore _store;
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly IEventPublisher _bus;

        public IndividualRebuildRunner(
            ILogger<IndividualRebuildRunner> logger,
            IEventStore store,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IEventPublisher bus)
        {
            _logger = logger;
            _store = store;
            _contextFactory = contextFactory;
            _bus = bus;
        }

        public async Task Run()
        {
            using var context = _contextFactory().Value;

            var organisationToRebuilds = await context.OrganisationsToRebuild.ToListAsync();

            if (organisationToRebuilds.Count == 0)
            {
                _logger.LogInformation("[{ProjectionName}] No organisations to rebuild.", ProjectionName, organisationToRebuilds);
                return;
            }

            _logger.LogInformation("[{ProjectionName}] Found {NumberOfOrganisations} organisations to rebuild.", ProjectionName, organisationToRebuilds);

            try
            {
                foreach (var organisation in organisationToRebuilds)
                {
                    var envelopes = _store
                        .GetEventEnvelopes<OrganisationRegistry.Organisation.Organisation>(organisation.OrganisationId)
                        .ToList();

                    _logger.LogInformation("[{ProjectionName}] Found {NumberOfEnvelopes} envelopes to process for Organisation {OrgId}.",
                        ProjectionName, envelopes.Count, organisation.OrganisationId);

                    foreach (var envelope in envelopes)
                    {
                        await ProcessEnvelope(envelope);
                    }

                    context.OrganisationsToRebuild.Remove(organisation);
                    await context.SaveChangesAsync();
                }
            }
            catch (Exception ex)
            {
                _logger.LogCritical(0, ex, "[{ProjectionName}] An exception occurred while handling envelopes.", ProjectionName);
                throw;
            }
        }

        private async Task<int?> ProcessEnvelope(IEnvelope envelope)
        {
            try
            {
                await _bus.Publish(null, null, (dynamic)envelope);
                return envelope.Number;
            }
            catch
            {
                _logger.LogCritical(
                    "[{ProjectionName}] An exception occurred while processing envelope #{EnvelopeNumber}:{@EnvelopeJson}",
                    ProjectionName,
                    envelope.Number,
                    envelope);

                throw;
            }
        }
    }
}
