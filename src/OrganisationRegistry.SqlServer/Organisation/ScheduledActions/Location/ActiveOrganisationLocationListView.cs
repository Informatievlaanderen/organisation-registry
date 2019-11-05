namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Location
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

    public class ActiveOrganisationLocationListItem
    {
        public Guid OrganisationLocationId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid LocationId { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class ActiveOrganisationLocationListConfiguration : EntityMappingConfiguration<ActiveOrganisationLocationListItem>
    {
        public override void Map(EntityTypeBuilder<ActiveOrganisationLocationListItem> b)
        {
            b.ToTable(nameof(ActiveOrganisationLocationListView.ProjectionTables.ActiveOrganisationLocationList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationLocationId)
                .ForSqlServerIsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.LocationId).IsRequired();

            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ValidTo);
        }
    }

    public class ActiveOrganisationLocationListView :
        Projection<ActiveOrganisationLocationListView>,
        IEventHandler<OrganisationLocationAdded>,
        IEventHandler<OrganisationLocationUpdated>,
        IEventHandler<MainLocationAssignedToOrganisation>,
        IEventHandler<MainLocationClearedFromOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;
        private readonly Dictionary<Guid, ValidTo> _endDatePerOrganisationLocationId;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public ActiveOrganisationLocationListView(
            ILogger<ActiveOrganisationLocationListView> logger,
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
                _endDatePerOrganisationLocationId =
                    context.OrganisationLocationList
                        .AsNoTracking()
                        .ToDictionary(
                            item => item.OrganisationLocationId,
                            item => new ValidTo(item.ValidTo));
            }
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ActiveOrganisationLocationList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
        {
            // cache ValidTo for the OrganisationLocationId,
            // because we will need it when MainLocationAssignedToOrganisation is published, which does not contain the ValidTo.
            _endDatePerOrganisationLocationId.UpdateMemoryCache(message.Body.OrganisationLocationId, new ValidTo(message.Body.ValidTo));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationUpdated> message)
        {
            // cache ValidTo for the OrganisationFormalFrameworkId,
            // because we will need it when FormalFrameworkAssignedToOrganisation is published, which does not contain the ValidTo.
            var validTo = new ValidTo(message.Body.ValidTo);
            _endDatePerOrganisationLocationId.UpdateMemoryCache(message.Body.OrganisationLocationId, validTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activeOrganisationLocation =
                    context.ActiveOrganisationLocationList.SingleOrDefault(item => item.OrganisationLocationId == message.Body.OrganisationLocationId);

                if (activeOrganisationLocation == null)
                    return;

                activeOrganisationLocation.OrganisationLocationId = message.Body.OrganisationLocationId;
                activeOrganisationLocation.OrganisationId = message.Body.OrganisationId;
                activeOrganisationLocation.LocationId = message.Body.LocationId;
                activeOrganisationLocation.ValidTo = validTo;

                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainLocationAssignedToOrganisation> message)
        {
            var validTo = _endDatePerOrganisationLocationId[message.Body.OrganisationLocationId];

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            var activeOrganisationLocationListItem = new ActiveOrganisationLocationListItem
            {
                OrganisationId = message.Body.OrganisationId,
                OrganisationLocationId = message.Body.OrganisationLocationId,
                LocationId = message.Body.MainLocationId,
                ValidTo = _endDatePerOrganisationLocationId[message.Body.OrganisationLocationId]
            };

            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                context.ActiveOrganisationLocationList.Add(activeOrganisationLocationListItem);
                context.SaveChanges();
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainLocationClearedFromOrganisation> message)
        {
            using (var context = new OrganisationRegistryTransactionalContext(dbConnection, dbTransaction))
            {
                var activeOrganisationLocationListItem =
                    context.ActiveOrganisationLocationList.SingleOrDefault(item =>
                        item.LocationId == message.Body.MainLocationId &&
                        item.OrganisationId == message.Body.OrganisationId);

                if (activeOrganisationLocationListItem == null)
                    return;

                context.ActiveOrganisationLocationList.Remove(activeOrganisationLocationListItem);

                context.SaveChanges();
            }
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = _contextFactory().Value)
            {
                return context.ActiveOrganisationLocationList
                    .Where(item => item.ValidTo.HasValue)
                    .Where(item => item.ValidTo.Value <= message.Body.Date)
                    .Select(item => new UpdateMainLocation(new OrganisationId(item.OrganisationId)))
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
