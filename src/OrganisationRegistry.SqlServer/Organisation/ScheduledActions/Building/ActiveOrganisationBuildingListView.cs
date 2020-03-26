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

    public class ActiveOrganisationBuildingListItem
    {
        public Guid OrganisationBuildingId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid BuildingId { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class ActiveOrganisationBuildingListConfiguration : EntityMappingConfiguration<ActiveOrganisationBuildingListItem>
    {
        public override void Map(EntityTypeBuilder<ActiveOrganisationBuildingListItem> b)
        {
            b.ToTable(nameof(ActiveOrganisationBuildingListView.ProjectionTables.ActiveOrganisationBuildingList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationBuildingId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.BuildingId).IsRequired();

            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ValidTo);
        }
    }

    public class ActiveOrganisationBuildingListView :
        Projection<ActiveOrganisationBuildingListView>,
        IEventHandler<OrganisationBuildingAdded>,
        IEventHandler<OrganisationBuildingUpdated>,
        IEventHandler<MainBuildingAssignedToOrganisation>,
        IEventHandler<MainBuildingClearedFromOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly Dictionary<Guid, ValidTo> _endDatePerOrganisationBuildingId;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ActiveOrganisationBuildingListView(
            ILogger<ActiveOrganisationBuildingListView> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider
        ) : base(logger)
        {
            _contextFactory = contextFactory;
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;

            using (var context = contextFactory().Value)
            {
                _endDatePerOrganisationBuildingId =
                    context.OrganisationBuildingList
                        .AsNoTracking()
                        .ToDictionary(
                            item => item.OrganisationBuildingId,
                            item => new ValidTo(item.ValidTo));
            }
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ActiveOrganisationBuildingList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingAdded> message)
        {
            // cache ValidTo for the OrganisationBuildingId,
            // because we will need it when MainBuildingAssignedToOrganisation is published, which does not contain the ValidTo.
            _endDatePerOrganisationBuildingId.UpdateMemoryCache(message.Body.OrganisationBuildingId, new ValidTo(message.Body.ValidTo));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingUpdated> message)
        {
            // cache ValidTo for the OrganisationFormalFrameworkId,
            // because we will need it when FormalFrameworkAssignedToOrganisation is published, which does not contain the ValidTo.
            var validTo = new ValidTo(message.Body.ValidTo);
            _endDatePerOrganisationBuildingId.UpdateMemoryCache(message.Body.OrganisationBuildingId, validTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activeOrganisationBuilding =
                    context.ActiveOrganisationBuildingList.SingleOrDefault(item => item.OrganisationBuildingId == message.Body.OrganisationBuildingId);

                if (activeOrganisationBuilding == null)
                    return;

                activeOrganisationBuilding.OrganisationBuildingId = message.Body.OrganisationBuildingId;
                activeOrganisationBuilding.OrganisationId = message.Body.OrganisationId;
                activeOrganisationBuilding.BuildingId = message.Body.BuildingId;
                activeOrganisationBuilding.ValidTo = validTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainBuildingAssignedToOrganisation> message)
        {
            var validTo = _endDatePerOrganisationBuildingId[message.Body.OrganisationBuildingId];

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            var activeOrganisationBuildingListItem = new ActiveOrganisationBuildingListItem
            {
                OrganisationId = message.Body.OrganisationId,
                OrganisationBuildingId = message.Body.OrganisationBuildingId,
                BuildingId = message.Body.MainBuildingId,
                ValidTo = _endDatePerOrganisationBuildingId[message.Body.OrganisationBuildingId]
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.ActiveOrganisationBuildingList.Add(activeOrganisationBuildingListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainBuildingClearedFromOrganisation> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activeOrganisationBuildingListItem =
                    context.ActiveOrganisationBuildingList
                        .SingleOrDefault(item =>
                            item.BuildingId == message.Body.MainBuildingId &&
                            item.OrganisationId == message.Body.OrganisationId);

                if (activeOrganisationBuildingListItem == null)
                    return;

                context.ActiveOrganisationBuildingList.Remove(activeOrganisationBuildingListItem);

                context.SaveChanges();
            }
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = _contextFactory().Value)
            {
                return context.ActiveOrganisationBuildingList
                    .Where(item => item.ValidTo.HasValue)
                    .Where(item => item.ValidTo.Value <= message.Body.Date)
                    .Select(item => new UpdateMainBuilding(new OrganisationId(item.OrganisationId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }
    }
}
