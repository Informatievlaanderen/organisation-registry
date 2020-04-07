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

    public class FutureActiveOrganisationLocationListItem
    {
        public Guid OrganisationLocationId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid LocationId { get; set; }

        public DateTime? ValidFrom { get; set; }
    }

    public class FutureActiveOrganisationLocationListConfiguration : EntityMappingConfiguration<FutureActiveOrganisationLocationListItem>
    {
        public override void Map(EntityTypeBuilder<FutureActiveOrganisationLocationListItem> b)
        {
            b.ToTable(nameof(FutureActiveOrganisationLocationListView.ProjectionTables.FutureActiveOrganisationLocationList), "OrganisationRegistry")
                .HasKey(p => p.OrganisationLocationId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.LocationId).IsRequired();

            b.Property(p => p.ValidFrom);

            b.HasIndex(x => x.ValidFrom);
        }
    }

    public class FutureActiveOrganisationLocationListView :
        Projection<FutureActiveOrganisationLocationListView>,
        IEventHandler<OrganisationLocationAdded>,
        IEventHandler<OrganisationLocationUpdated>,
        IEventHandler<MainLocationAssignedToOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;

        public FutureActiveOrganisationLocationListView(
            ILogger<FutureActiveOrganisationLocationListView> logger,
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
            FutureActiveOrganisationLocationList
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
        {
            var validFrom = new ValidFrom(message.Body.ValidFrom);
            if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
                InsertFutureActiveOrganisationLocation(context, message);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationUpdated> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var validFrom = new ValidFrom(message.Body.ValidFrom);
                if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
                {
                    DeleteFutureActiveOrganisationLocation(context, message.Body.OrganisationLocationId);
                }
                else
                {
                    UpsertFutureActiveOrganisationLocation(context, message);
                }
            }
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<MainLocationAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
                DeleteFutureActiveOrganisationLocation(context, message.Body.OrganisationLocationId);
        }

        public List<ICommand> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = ContextFactory.Create())
            {
                var contextFutureActiveOrganisationLocationList = context.FutureActiveOrganisationLocationList.ToList();
                return contextFutureActiveOrganisationLocationList
                    .Where(item => item.ValidFrom.HasValue)
                    .Where(item => item.ValidFrom.Value <= message.Body.Date)
                    .Select(item => new UpdateMainLocation(new OrganisationId(item.OrganisationId)))
                    .Cast<ICommand>()
                    .ToList();
            }
        }

        public override void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static void InsertFutureActiveOrganisationLocation(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationLocationAdded> message)
        {
            var futureActiveOrganisationLocationListItem = new FutureActiveOrganisationLocationListItem
            {
                OrganisationId = message.Body.OrganisationId,
                LocationId = message.Body.LocationId,
                OrganisationLocationId = message.Body.OrganisationLocationId,
                ValidFrom = message.Body.ValidFrom
            };

            context.FutureActiveOrganisationLocationList.Add(futureActiveOrganisationLocationListItem);
            context.SaveChanges();
        }

        private static void UpsertFutureActiveOrganisationLocation(
            OrganisationRegistryContext context,
            IEnvelope<OrganisationLocationUpdated> message)
        {
            var futureActiveOrganisationLocation =
                context.FutureActiveOrganisationLocationList.SingleOrDefault(
                    item => item.OrganisationLocationId == message.Body.OrganisationLocationId);

            if (futureActiveOrganisationLocation == null)
            {
                var futureActiveOrganisationLocationListItem =
                    new FutureActiveOrganisationLocationListItem
                    {
                        OrganisationId = message.Body.OrganisationId,
                        LocationId = message.Body.LocationId,
                        OrganisationLocationId = message.Body.OrganisationLocationId,
                        ValidFrom = message.Body.ValidFrom
                    };

                context.FutureActiveOrganisationLocationList.Add(futureActiveOrganisationLocationListItem);
            }
            else
            {
                futureActiveOrganisationLocation.OrganisationLocationId = message.Body.OrganisationLocationId;
                futureActiveOrganisationLocation.OrganisationId = message.Body.OrganisationId;
                futureActiveOrganisationLocation.LocationId = message.Body.LocationId;
                futureActiveOrganisationLocation.ValidFrom = message.Body.ValidFrom;
            }

            context.SaveChanges();
        }

        private static void DeleteFutureActiveOrganisationLocation(
            OrganisationRegistryContext context,
            Guid organisationLocationId)
        {
            var futureActiveOrganisationLocation =
                context.FutureActiveOrganisationLocationList.SingleOrDefault(
                    item => item.OrganisationLocationId == organisationLocationId);

            if (futureActiveOrganisationLocation == null)
                return;

            context.FutureActiveOrganisationLocationList.Remove(futureActiveOrganisationLocation);
            context.SaveChanges();
        }
    }
}
