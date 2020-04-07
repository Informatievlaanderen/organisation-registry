namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Building
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
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class FutureActiveOrganisationBuildingListItem
    {
        public Guid OrganisationBuildingId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid BuildingId { get; set; }

        public DateTime? ValidFrom { get; set; }
    }

    public class FutureActiveOrganisationBuildingListConfiguration : EntityMappingConfiguration<FutureActiveOrganisationBuildingListItem>
    {
        public override void Map(EntityTypeBuilder<FutureActiveOrganisationBuildingListItem> b)
        {
            b.ToTable(nameof(FutureActiveOrganisationBuildingListView.ProjectionTables.FutureActiveOrganisationBuildingList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationBuildingId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.BuildingId).IsRequired();

            b.Property(p => p.ValidFrom);

            b.HasIndex(x => x.ValidFrom);
        }
    }

    public class FutureActiveOrganisationBuildingListView :
        Projection<FutureActiveOrganisationBuildingListView>,
        IEventHandler<OrganisationBuildingAdded>,
        IEventHandler<OrganisationBuildingUpdated>,
        IEventHandler<MainBuildingAssignedToOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FutureActiveOrganisationBuildingListView(
            ILogger<FutureActiveOrganisationBuildingListView> logger,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider,
            IContextFactory contextFactory
        ) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            FutureActiveOrganisationBuildingList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingAdded> message)
        {
            var validFrom = new ValidFrom(message.Body.ValidFrom);
            if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
                InsertFutureActiveOrganisationBuilding(context, message);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var validFrom = new ValidFrom(message.Body.ValidFrom);
                if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                {
                    DeleteFutureActiveOrganisationBuilding(context, message.Body.OrganisationBuildingId);
                }
                else
                {
                    UpsertFutureActiveOrganisationBuilding(context, message);
                }
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainBuildingAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
                DeleteFutureActiveOrganisationBuilding(context, message.Body.OrganisationBuildingId);
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = ContextFactory.Create())
            {
                var contextFutureActiveOrganisationBuildingList = context.FutureActiveOrganisationBuildingList.ToList();
                return contextFutureActiveOrganisationBuildingList
                    .Where(item => item.ValidFrom.HasValue)
                    .Where(item => item.ValidFrom.Value <= message.Body.Date)
                    .Select(item => new UpdateMainBuilding(new OrganisationId(item.OrganisationId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static void InsertFutureActiveOrganisationBuilding(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationBuildingAdded> message)
        {
            var futureActiveOrganisationBuildingListItem = new FutureActiveOrganisationBuildingListItem
            {
                OrganisationId = message.Body.OrganisationId,
                BuildingId = message.Body.BuildingId,
                OrganisationBuildingId = message.Body.OrganisationBuildingId,
                ValidFrom = message.Body.ValidFrom
            };

            context.FutureActiveOrganisationBuildingList.Add(futureActiveOrganisationBuildingListItem);
            context.SaveChanges();
        }

        private static void UpsertFutureActiveOrganisationBuilding(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationBuildingUpdated> message)
        {
            var futureActiveOrganisationBuilding =
                context.FutureActiveOrganisationBuildingList.SingleOrDefault(
                    item => item.OrganisationBuildingId == message.Body.OrganisationBuildingId);

            if (futureActiveOrganisationBuilding == null)
            {
                var futureActiveOrganisationBuildingListItem =
                    new FutureActiveOrganisationBuildingListItem
                    {
                        OrganisationId = message.Body.OrganisationId,
                        BuildingId = message.Body.BuildingId,
                        OrganisationBuildingId = message.Body.OrganisationBuildingId,
                        ValidFrom = message.Body.ValidFrom
                    };

                context.FutureActiveOrganisationBuildingList.Add(futureActiveOrganisationBuildingListItem);
            }
            else
            {
                futureActiveOrganisationBuilding.OrganisationBuildingId = message.Body.OrganisationBuildingId;
                futureActiveOrganisationBuilding.OrganisationId = message.Body.OrganisationId;
                futureActiveOrganisationBuilding.BuildingId = message.Body.BuildingId;
                futureActiveOrganisationBuilding.ValidFrom = message.Body.ValidFrom;
            }

            context.SaveChanges();
        }

        private static void DeleteFutureActiveOrganisationBuilding(
            OrganisationRegistryContext context,
            Guid organisationBuildingId)
        {
            var futureActiveOrganisationBuilding =
                context.FutureActiveOrganisationBuildingList.SingleOrDefault(
                    item => item.OrganisationBuildingId == organisationBuildingId);

            if (futureActiveOrganisationBuilding == null)
                return;

            context.FutureActiveOrganisationBuildingList.Remove(futureActiveOrganisationBuilding);
            context.SaveChanges();
        }
    }
}
