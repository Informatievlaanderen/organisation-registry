﻿namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Parent
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
    using OrganisationRegistry.Infrastructure.Commands;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Organisation;
    using OrganisationRegistry.Organisation.Commands;
    using OrganisationRegistry.Organisation.Events;
    using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

    public class ActiveOrganisationParentListItem
    {
        public Guid OrganisationOrganisationParentId { get; set; }

        public Guid OrganisationId { get; set; }

        public Guid ParentOrganisationId { get; set; }

        public DateTime? ValidTo { get; set; }
    }

    public class ActiveOrganisationParentListConfiguration : EntityMappingConfiguration<ActiveOrganisationParentListItem>
    {
        public override void Map(EntityTypeBuilder<ActiveOrganisationParentListItem> b)
        {
            b.ToTable(nameof(ActiveOrganisationParentListView.ProjectionTables.ActiveOrganisationParentList), WellknownSchemas.OrganisationRegistrySchema)
                .HasKey(p => p.OrganisationOrganisationParentId)
                .IsClustered(false);

            b.Property(p => p.OrganisationId).IsRequired();

            b.Property(p => p.ParentOrganisationId).IsRequired();

            b.Property(p => p.ValidTo);

            b.HasIndex(x => x.ValidTo);
        }
    }

    public class ActiveOrganisationParentListView :
        Projection<ActiveOrganisationParentListView>,
        IEventHandler<OrganisationParentAdded>,
        IEventHandler<OrganisationParentUpdated>,
        IEventHandler<ParentAssignedToOrganisation>,
        IEventHandler<ParentClearedFromOrganisation>,
        IReactionHandler<DayHasPassed>
    {
        private readonly Dictionary<Guid, ValidTo> _endDatePerOrganisationOrganisationParentId;
        private readonly IEventStore _eventStore;
        private readonly IDateTimeProvider _dateTimeProvider;


        public ActiveOrganisationParentListView(
            ILogger<ActiveOrganisationParentListView> logger,
            IEventStore eventStore,
            IDateTimeProvider dateTimeProvider,
            IContextFactory contextFactory) : base(logger, contextFactory)
        {
            _eventStore = eventStore;
            _dateTimeProvider = dateTimeProvider;


            using (var context = contextFactory.Create())
            {
                _endDatePerOrganisationOrganisationParentId =
                    context.OrganisationParentList
                        .AsNoTracking()
                        .ToDictionary(
                            item => item.OrganisationOrganisationParentId,
                            item => new ValidTo(item.ValidTo));
            }
        }

        public override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));

        public enum ProjectionTables
        {
            ActiveOrganisationParentList
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
        {
            // cache ValidTo for the OrganisationParent,
            // because we will need it when ParentAssignedToOrganisation is published, which does not contain the ValidTo.
            _endDatePerOrganisationOrganisationParentId.UpdateMemoryCache(message.Body.OrganisationOrganisationParentId, new ValidTo(message.Body.ValidTo));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
        {
            // cache ValidTo for the OrganisationParent,
            // because we will need it when ParentAssignedToOrganisation is published, which does not contain the ValidTo.
            var validTo = new ValidTo(message.Body.ValidTo);
            _endDatePerOrganisationOrganisationParentId.UpdateMemoryCache(message.Body.OrganisationOrganisationParentId, validTo);

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var activeOrganisationParent =
                    context.ActiveOrganisationParentList.SingleOrDefault(item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

                if (activeOrganisationParent == null)
                    return;

                activeOrganisationParent.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
                activeOrganisationParent.OrganisationId = message.Body.OrganisationId;
                activeOrganisationParent.ParentOrganisationId = message.Body.ParentOrganisationId;
                activeOrganisationParent.ValidTo = validTo;

                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
        {
            var validTo = _endDatePerOrganisationOrganisationParentId[message.Body.OrganisationOrganisationParentId];

            if (validTo.IsInPastOf(_dateTimeProvider.Today))
                return;

            var activeOrganisationParentListItem = new ActiveOrganisationParentListItem
            {
                OrganisationId = message.Body.OrganisationId,
                ParentOrganisationId = message.Body.ParentOrganisationId,
                OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
                ValidTo = validTo
            };

            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                await context.ActiveOrganisationParentList.AddAsync(activeOrganisationParentListItem);
                await context.SaveChangesAsync();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
            {
                var activeOrganisationParentListItem =
                    context.ActiveOrganisationParentList
                        .SingleOrDefault(item =>
                            item.ParentOrganisationId == message.Body.ParentOrganisationId &&
                            item.OrganisationId == message.Body.OrganisationId);

                if (activeOrganisationParentListItem == null)
                    return;

                context.ActiveOrganisationParentList.Remove(activeOrganisationParentListItem);

                await context.SaveChangesAsync();
            }
        }

        public async Task<List<ICommand>> Handle(IEnvelope<DayHasPassed> message)
        {
            using (var context = ContextFactory.Create())
            {
                return context.ActiveOrganisationParentList
                    .Where(item => item.ValidTo.HasValue)
                    .Where(item => item.ValidTo.Value <= message.Body.Date)
                    .Select(item => new UpdateCurrentOrganisationParent(new OrganisationId(item.OrganisationId)))
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
