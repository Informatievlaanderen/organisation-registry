namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.FormalFramework
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.FormalFramework;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class ActiveOrganisationFormalFrameworkListItem
    {
        public Guid OrganisationFormalFrameworkId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid FormalFrameworkId { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class ActiveOrganisationFormalFrameworkListConfiguration : EntityMappingConfiguration<ActiveOrganisationFormalFrameworkListItem>
    {
        public override void Map(EntityTypeBuilder<ActiveOrganisationFormalFrameworkListItem> b)
        {
            b.ToTable(nameof(ActiveOrganisationFormalFrameworkListView.ProjectionTables.ActiveOrganisationFormalFrameworkList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.OrganisationFormalFrameworkId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.FormalFrameworkId).IsRequired();

            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ValidTo);
        }
    }

    public class ActiveOrganisationFormalFrameworkListView :
        Projection<ActiveOrganisationFormalFrameworkListView>,
        IEventHandler<OrganisationFormalFrameworkAdded>,
        IEventHandler<OrganisationFormalFrameworkUpdated>,
        IEventHandler<FormalFrameworkAssignedToOrganisation>,
        IEventHandler<FormalFrameworkClearedFromOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Dictionary<Guid, ValidTo> _endDatePerOrganisationFormalFrameworkId;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;
        public ActiveOrganisationFormalFrameworkListView(
            ILogger<ActiveOrganisationFormalFrameworkListView> logger,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;

            using (var context = contextFactory.Create())
            {
                _endDatePerOrganisationFormalFrameworkId =
                    context.OrganisationFormalFrameworkList
                        .AsNoTracking()
                        .ToDictionary(
                            item => item.OrganisationFormalFrameworkId,
                            item => new ValidTo(item.ValidTo));
            }
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ActiveOrganisationFormalFrameworkList
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            // cache ValidTo for the OrganisationFormalFrameworkId,
            // because we will need it when FormalFrameworkAssignedToOrganisation is published, which does not contain the ValidTo.
            _endDatePerOrganisationFormalFrameworkId.UpdateMemoryCache(message.Body.OrganisationFormalFrameworkId, new ValidTo(message.Body.ValidTo));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            // cache ValidTo for the OrganisationFormalFrameworkId,
            // because we will need it when FormalFrameworkAssignedToOrganisation is published, which does not contain the ValidTo.
            var validTo = new ValidTo(message.Body.ValidTo);
            _endDatePerOrganisationFormalFrameworkId.UpdateMemoryCache(message.Body.OrganisationFormalFrameworkId, validTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var activeOrganisationFormalFramework =
                    context.ActiveOrganisationFormalFrameworkList.SingleOrDefault(item => item.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

                if (activeOrganisationFormalFramework == null)
                    return;

                activeOrganisationFormalFramework.OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId;
                activeOrganisationFormalFramework.OrganisationId = message.Body.OrganisationId;
                activeOrganisationFormalFramework.FormalFrameworkId = message.Body.FormalFrameworkId;
                activeOrganisationFormalFramework.ValidTo = validTo;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkAssignedToOrganisation> message)
        {
            var validTo = _endDatePerOrganisationFormalFrameworkId[message.Body.OrganisationFormalFrameworkId];

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            var activeOrganisationFormalFrameworkListItem = new ActiveOrganisationFormalFrameworkListItem
            {
                OrganisationId = message.Body.OrganisationId,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                ValidTo = validTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.ActiveOrganisationFormalFrameworkList.AddAsync(activeOrganisationFormalFrameworkListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var activeOrganisationFormalFramework =
                    context.ActiveOrganisationFormalFrameworkList
                        .SingleOrDefault(item => item.OrganisationFormalFrameworkId ==
                                                 message.Body.OrganisationFormalFrameworkId);

                if (activeOrganisationFormalFramework == null)
                    return;

                context.ActiveOrganisationFormalFrameworkList.Remove(activeOrganisationFormalFramework);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ICommand>> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = ContextFactory.Create())
            {
                var contextActiveOrganisationFormalFrameworkList = context.ActiveOrganisationFormalFrameworkList.ToList();
                return contextActiveOrganisationFormalFrameworkList
                    .Where(item => item.ValidTo.HasValue)
                    .Where(item => item.ValidTo.Value <= message.Body.Date)
                    .Select(item => new UpdateOrganisationFormalFrameworkParents(new OrganisationId(item.OrganisationId), new FormalFrameworkId(item.FormalFrameworkId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
