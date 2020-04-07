namespace OrganisationRegistry.SqlServer.Body.ScheduledActions.Organisation
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using Autofac.Features.OwnedInstances;
    using Day.Events;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.EntityFrameworkCore.Metadata.Builders;
    using Microsoft.Extensions.Logging;
    using OrganisationRegistry.Body;
    using OrganisationRegistry.Body.Commands;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class FutureActiveBodyOrganisationListItem
    {
        public Guid BodyOrganisationId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid BodyId { get; set; }

        public DateTime? ValidFrom { get; set; }
    }

    public class FutureActiveBodyOrganisationListConfiguration : EntityMappingConfiguration<FutureActiveBodyOrganisationListItem>
    {
        public override void Map(EntityTypeBuilder<FutureActiveBodyOrganisationListItem> b)
        {
            b.ToTable(nameof(FutureActiveBodyOrganisationListView.ProjectionTables.FutureActiveBodyOrganisationList), "OrganisationRegistry")
                .HasKey(p => p.BodyOrganisationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.BodyId).IsRequired();

            b.Property(p => p.ValidFrom);

            b.HasIndex(x => x.ValidFrom);
        }
    }

    public class FutureActiveBodyOrganisationListView :
        Projection<FutureActiveBodyOrganisationListView>,
        IEventHandler<BodyOrganisationAdded>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<BodyAssignedToOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _reactionContextFactory;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;
        private Func<DbConnection, DbTransaction, OrganisationRegistryContext> _contextFactory;

        public FutureActiveBodyOrganisationListView(
            ILogger<FutureActiveBodyOrganisationListView> logger,
            Func<Owned<OrganisationRegistryContext>> reactionContextFactory,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider,
            Func<DbConnection, DbTransaction, OrganisationRegistryContext> contextFactory = null) : base(logger)
        {
            _reactionContextFactory = reactionContextFactory;
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;
            _contextFactory = contextFactory ?? ((connection, transaction) =>
                new OrganisationRegistryTransactionalContext(connection, transaction));
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            FutureActiveBodyOrganisationList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
        {
            var validFrom = new ValidFrom(message.Body.ValidFrom);
            if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                return;

            using (var context = _contextFactory(dbConnection, dbTransaction))
                InsertFutureActiveBodyOrganisation(context, message);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
            {
                var validFrom = new ValidFrom(message.Body.ValidFrom);
                if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                {
                    DeleteFutureActiveBodyOrganisation(context, message.Body.BodyOrganisationId);
                }
                else
                {
                    UpsertFutureActiveBodyOrganisation(context, message);
                }
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = _contextFactory(dbConnection, dbTransaction))
                DeleteFutureActiveBodyOrganisation(context, message.Body.BodyOrganisationId);
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = _reactionContextFactory().Value)
            {
                var futureActiveBodyOrganisations = context.FutureActiveBodyOrganisationList.ToList();
                return futureActiveBodyOrganisations
                    .Where(item => item.ValidFrom.HasValue)
                    .Where(item => item.ValidFrom.Value <= message.Body.Date)
                    .Select(item => new UpdateCurrentBodyOrganisation(new BodyId(item.BodyId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static void InsertFutureActiveBodyOrganisation(
            OrganisationRegistryContext context,
            IEnvelope<BodyOrganisationAdded> message)
        {
            var futureActiveOrganisationParentListItem = new FutureActiveBodyOrganisationListItem
            {
                OrganisationId = message.Body.OrganisationId,
                BodyId = message.Body.BodyId,
                BodyOrganisationId = message.Body.BodyOrganisationId,
                ValidFrom = message.Body.ValidFrom
            };

            context.FutureActiveBodyOrganisationList.Add(futureActiveOrganisationParentListItem);
            context.SaveChanges();
        }

        private static void UpsertFutureActiveBodyOrganisation(
            OrganisationRegistryContext context,
            IEnvelope<BodyOrganisationUpdated> message)
        {
            var futureActiveOrganisationParent =
                context.FutureActiveBodyOrganisationList.SingleOrDefault(
                    item => item.BodyOrganisationId == message.Body.BodyOrganisationId);

            if (futureActiveOrganisationParent == null)
            {
                var futureActiveOrganisationParentListItem =
                    new FutureActiveBodyOrganisationListItem
                    {
                        OrganisationId = message.Body.OrganisationId,
                        BodyId = message.Body.BodyId,
                        BodyOrganisationId = message.Body.BodyOrganisationId,
                        ValidFrom = message.Body.ValidFrom
                    };

                context.FutureActiveBodyOrganisationList.Add(futureActiveOrganisationParentListItem);
            }
            else
            {
                futureActiveOrganisationParent.BodyOrganisationId = message.Body.BodyOrganisationId;
                futureActiveOrganisationParent.OrganisationId = message.Body.OrganisationId;
                futureActiveOrganisationParent.BodyId = message.Body.BodyId;
                futureActiveOrganisationParent.ValidFrom = message.Body.ValidFrom;
            }

            context.SaveChanges();
        }

        private static void DeleteFutureActiveBodyOrganisation(
            OrganisationRegistryContext context,
            Guid bodyOrganisationId)
        {
            var futureActiveOrganisationParent =
                context.FutureActiveBodyOrganisationList.SingleOrDefault(
                    item => item.BodyOrganisationId == bodyOrganisationId);

            if (futureActiveOrganisationParent == null)
                return;

            context.FutureActiveBodyOrganisationList.Remove(futureActiveOrganisationParent);
            context.SaveChanges();
        }
    }
}
