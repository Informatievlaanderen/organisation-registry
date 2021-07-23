namespace OrganisationRegistry.ElasticSearch.Projections.People.Cache
{
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using SqlServer.ElasticSearchProjections;
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
            await using var context = _contextFactory.Create();
            if (context.OrganisationPerBodyListForES.Any(x => x.BodyId == message.Body.BodyId))
                return;

            var body = new OrganisationPerBody
            {
                BodyId = message.Body.BodyId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationName = message.Body.OrganisationName,
            };

            context.OrganisationPerBodyListForES.Add(body);

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            await using var context = _contextFactory.Create();
            var body = await context
                .OrganisationPerBodyListForES
                .FindAsync(message.Body.BodyId);

            if (body == null)
                return;

            context.OrganisationPerBodyListForES.Remove(body);

            await context.SaveChangesAsync();
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            await using var context = _contextFactory.Create();

            var organisation = await context.OrganisationCache.FindAsync(message.Body.OrganisationId);
            var body = await context
                .OrganisationPerBodyListForES
                .FindAsync(message.Body.BodyId);

            if (body == null)
                return;

            body.OrganisationId = message.Body.BodyOrganisationId;
            body.OrganisationName = organisation.Name;
        }
    }
}
