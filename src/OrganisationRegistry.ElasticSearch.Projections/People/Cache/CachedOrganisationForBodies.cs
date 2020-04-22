namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Microsoft.Extensions.Logging;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;

    public class CachedOrganisationForBodies :
        Infrastructure.BaseProjection<CachedOrganisationForBodies>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,
        IEventHandler<BodyOrganisationUpdated>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly IMemoryCaches _memoryCaches;

        public CachedOrganisationForBodies(
            ILogger<CachedOrganisationForBodies> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _contextFactory = contextFactory;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = _contextFactory().Value)
            {
                if (context.OrganisationPerBodyListForES.Any(x => x.BodyId == message.Body.BodyId))
                    return;

                var body = new OrganisationPerBody
                {
                    BodyId = message.Body.BodyId,
                    OrganisationId = message.Body.OrganisationId,
                    OrganisationName = message.Body.OrganisationName,
                };

                context.OrganisationPerBodyListForES.Add(body);

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            using (var context = _contextFactory().Value)
            {
                var body = context
                    .OrganisationPerBodyListForES
                    .SingleOrDefault(x => x.BodyId == message.Body.BodyId);

                if (body == null)
                    return;

                context.OrganisationPerBodyListForES.Remove(body);

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            using (var context = _contextFactory().Value)
            {
                var body = context
                    .OrganisationPerBodyListForES
                    .SingleOrDefault(x => x.BodyId == message.Body.BodyId);

                if (body == null)
                    return;

                body.OrganisationId = message.Body.BodyOrganisationId;
                body.OrganisationName = _memoryCaches.OrganisationNames[message.Body.OrganisationId];
            }
        }
    }
}
