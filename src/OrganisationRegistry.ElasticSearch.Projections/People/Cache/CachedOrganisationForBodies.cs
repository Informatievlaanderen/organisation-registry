namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class CachedOrganisationForBodies :
        Infrastructure.BaseProjection<CachedOrganisationForBodies>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,
        IEventHandler<BodyOrganisationUpdated>
    {
        private readonly IContextFactory _contextFactory;

        public CachedOrganisationForBodies(
            ILogger<CachedOrganisationForBodies> logger,
            IContextFactory contextFactory) : base(logger)
        {
            _contextFactory = contextFactory;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = _contextFactory.Create())
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
            using (var context = _contextFactory.Create())
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
            await using var context = _contextFactory.Create();

            var organisation = await context.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);
            var body = context
                .OrganisationPerBodyListForES
                .SingleOrDefault(x => x.BodyId == message.Body.BodyId);

            if (body == null)
                return;

            body.OrganisationId = message.Body.BodyOrganisationId;
            body.OrganisationName = organisation.Name;
        }
    }
}
