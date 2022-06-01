namespace OrganisationRegistry.SqlServer.Body.ScheduledActions.Organisation;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

public class ActiveBodyOrganisationListItem
{
    public Guid BodyOrganisationId { get; set; }

    public Guid OrganisationId { get; set; }

    public Guid BodyId { get; set; }

    public DateTime? ValidTo { get; set; }
}

public class ActiveBodyOrganisationListConfiguration : EntityMappingConfiguration<ActiveBodyOrganisationListItem>
{
    public override void Map(EntityTypeBuilder<ActiveBodyOrganisationListItem> b)
    {
        b.ToTable(nameof(ActiveBodyOrganisationListView.ProjectionTables.ActiveBodyOrganisationList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.BodyOrganisationId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.BodyId).IsRequired();

        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.ValidTo);
    }
}

public class ActiveBodyOrganisationListView :
    Projection<ActiveBodyOrganisationListView>,
    IEventHandler<BodyOrganisationAdded>,
    IEventHandler<BodyOrganisationUpdated>,
    IEventHandler<BodyAssignedToOrganisation>,
    IEventHandler<BodyClearedFromOrganisation>
{
    private readonly Dictionary<Guid, ValidTo> _endDatePerBodyOrganisationId;
    private readonly IEventStore _eventStore;
    private readonly IDateTimeProvider _dateTimeProvider;
    public ActiveBodyOrganisationListView(
        ILogger<ActiveBodyOrganisationListView> logger,
        IEventStore eventStore,
        IDateTimeProvider dateTimeProvider,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _dateTimeProvider = dateTimeProvider;

        using (var context = contextFactory.Create())
        {
            _endDatePerBodyOrganisationId =
                context.BodyOrganisationList
                    .AsNoTracking()
                    .ToDictionary(
                        item => item.BodyOrganisationId,
                        item => new ValidTo(item.ValidTo));
        }
    }

    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        ActiveBodyOrganisationList
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
    {
        // cache ValidTo for the OrganisationParent,
        // because we will need it when BodyAssignedToOrganisation is published, which does not contain the ValidTo.
        _endDatePerBodyOrganisationId.UpdateMemoryCache(message.Body.BodyOrganisationId, new ValidTo(message.Body.ValidTo));
        await Task.CompletedTask;

    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
    {
        // cache ValidTo for the OrganisationParent,
        // because we will need it when BodyAssignedToOrganisation is published, which does not contain the ValidTo.
        var validTo = new ValidTo(message.Body.ValidTo);
        _endDatePerBodyOrganisationId.UpdateMemoryCache(message.Body.BodyOrganisationId, validTo);

        if (validTo.IsInPastOf(_dateTimeProvider.Today))
            return;

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var activeBodyOrganisation =
                context.ActiveBodyOrganisationList.SingleOrDefault(item => item.BodyOrganisationId == message.Body.BodyOrganisationId);

            if (activeBodyOrganisation == null)
                return;

            activeBodyOrganisation.BodyOrganisationId = message.Body.BodyOrganisationId;
            activeBodyOrganisation.OrganisationId = message.Body.OrganisationId;
            activeBodyOrganisation.BodyId = message.Body.BodyId;
            activeBodyOrganisation.ValidTo = validTo;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
    {
        var validTo = _endDatePerBodyOrganisationId[message.Body.BodyOrganisationId];

        if (validTo.IsInPastOf(_dateTimeProvider.Today))
            return;

        var activeBodyOrganisationListItem = new ActiveBodyOrganisationListItem
        {
            OrganisationId = message.Body.OrganisationId,
            BodyId = message.Body.BodyId,
            BodyOrganisationId = message.Body.BodyOrganisationId,
            ValidTo = validTo
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.ActiveBodyOrganisationList.AddAsync(activeBodyOrganisationListItem);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var activeBodyOrganisationListItem =
                context.ActiveBodyOrganisationList
                    .SingleOrDefault(item =>
                        item.BodyId == message.Body.BodyId &&
                        item.OrganisationId == message.Body.OrganisationId);

            if (activeBodyOrganisationListItem == null)
                return;

            context.ActiveBodyOrganisationList.Remove(activeBodyOrganisationListItem);

            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
