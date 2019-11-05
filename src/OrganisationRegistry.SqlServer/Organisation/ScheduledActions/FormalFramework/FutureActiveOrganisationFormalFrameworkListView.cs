namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.FormalFramework
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
    using OrganisationRegistry.FormalFramework;
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class FutureActiveOrganisationFormalFrameworkListItem
    {
        public Guid OrganisationFormalFrameworkId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid FormalFrameworkId { get; set; }

        public DateTime? ValidFrom { get; set; }
    }

    public class FutureActiveOrganisationFormalFrameworkListConfiguration : EntityMappingConfiguration<FutureActiveOrganisationFormalFrameworkListItem>
    {
        public override void Map(EntityTypeBuilder<FutureActiveOrganisationFormalFrameworkListItem> b)
        {
            b.ToTable(nameof(FutureActiveOrganisationFormalFrameworkListView.ProjectionTables.FutureActiveOrganisationFormalFrameworkList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationFormalFrameworkId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.FormalFrameworkId).IsRequired();

            b.Property(p => p.ValidFrom);

            b.HasIndex(x => x.ValidFrom);
        }
    }

    public class FutureActiveOrganisationFormalFrameworkListView :
        Projection<FutureActiveOrganisationFormalFrameworkListView>,
        IEventHandler<OrganisationFormalFrameworkAdded>,
        IEventHandler<OrganisationFormalFrameworkUpdated>,
        IEventHandler<FormalFrameworkAssignedToOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FutureActiveOrganisationFormalFrameworkListView(
            ILogger<FutureActiveOrganisationFormalFrameworkListView> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider
        ) : base(logger)
        {
            _contextFactory = contextFactory;
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            FutureActiveOrganisationFormalFrameworkList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            var validFrom = new ValidFrom(message.Body.ValidFrom);
            if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                return;

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
                InsertFutureActiveOrganisationFormalFramework(context, message);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var validFrom = new ValidFrom(message.Body.ValidFrom);
                if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                {
                    DeleteFutureActiveOrganisationFormalFramework(context, message.Body.OrganisationFormalFrameworkId);
                }
                else
                {
                    UpsertFutureActiveOrganisationFormalFramework(context, message);
                }
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FormalFrameworkAssignedToOrganisation> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
                DeleteFutureActiveOrganisationFormalFramework(context, message.Body.OrganisationFormalFrameworkId);
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = _contextFactory().Value)
            {
                var contextFutureActiveOrganisationFormalFrameworkList = context.FutureActiveOrganisationFormalFrameworkList.ToList();
                return contextFutureActiveOrganisationFormalFrameworkList
                    .Where(item => item.ValidFrom.HasValue)
                    .Where(item => item.ValidFrom.Value <= message.Body.Date)
                    .Select(item => new UpdateOrganisationFormalFrameworkParents(new OrganisationId(item.OrganisationId), new FormalFrameworkId(item.FormalFrameworkId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static void InsertFutureActiveOrganisationFormalFramework(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationFormalFrameworkAdded> message)
        {
            var futureActiveOrganisationFormalFrameworkListItem = new FutureActiveOrganisationFormalFrameworkListItem
            {
                OrganisationId = message.Body.OrganisationId,
                FormalFrameworkId = message.Body.FormalFrameworkId,
                OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                ValidFrom = message.Body.ValidFrom
            };

            context.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFrameworkListItem);
            context.SaveChanges();
        }

        private static void UpsertFutureActiveOrganisationFormalFramework(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationFormalFrameworkUpdated> message)
        {
            var futureActiveOrganisationFormalFramework =
                context.FutureActiveOrganisationFormalFrameworkList.SingleOrDefault(
                    item => item.OrganisationFormalFrameworkId == message.Body.OrganisationFormalFrameworkId);

            if (futureActiveOrganisationFormalFramework == null)
            {
                var futureActiveOrganisationFormalFrameworkListItem =
                    new FutureActiveOrganisationFormalFrameworkListItem
                    {
                        OrganisationId = message.Body.OrganisationId,
                        FormalFrameworkId = message.Body.FormalFrameworkId,
                        OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId,
                        ValidFrom = message.Body.ValidFrom
                    };

                context.FutureActiveOrganisationFormalFrameworkList.Add(futureActiveOrganisationFormalFrameworkListItem);
            }
            else
            {
                futureActiveOrganisationFormalFramework.OrganisationFormalFrameworkId = message.Body.OrganisationFormalFrameworkId;
                futureActiveOrganisationFormalFramework.OrganisationId = message.Body.OrganisationId;
                futureActiveOrganisationFormalFramework.FormalFrameworkId = message.Body.FormalFrameworkId;
                futureActiveOrganisationFormalFramework.ValidFrom = message.Body.ValidFrom;
            }

            context.SaveChanges();
        }

        private static void DeleteFutureActiveOrganisationFormalFramework(
            OrganisationRegistryContext context,
            Guid organisationFormalFrameworkId)
        {
            var futureActiveOrganisationFormalFramework =
                context.FutureActiveOrganisationFormalFrameworkList.SingleOrDefault(
                    item => item.OrganisationFormalFrameworkId == organisationFormalFrameworkId);

            if (futureActiveOrganisationFormalFramework == null)
                return;

            context.FutureActiveOrganisationFormalFrameworkList.Remove(futureActiveOrganisationFormalFramework);
            context.SaveChanges();
        }
    }
}
