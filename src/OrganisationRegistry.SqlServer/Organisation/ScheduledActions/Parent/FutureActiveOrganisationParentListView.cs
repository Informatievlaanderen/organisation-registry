namespace OrganisationRegistry.SqlServer.Organisation.ScheduledActions.Parent;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using RebuildProjection = OrganisationRegistry.Infrastructure.Events.RebuildProjection;

public class FutureActiveOrganisationParentListItem
{
    public Guid OrganisationOrganisationParentId { get; set; }

    public Guid OrganisationId { get; set; }

    public Guid ParentOrganisationId { get; set; }

    public DateTime? ValidFrom { get; set; }
}

public class FutureActiveOrganisationParentListConfiguration : EntityMappingConfiguration<FutureActiveOrganisationParentListItem>
{
    public override void Map(EntityTypeBuilder<FutureActiveOrganisationParentListItem> b)
    {
        b.ToTable(nameof(FutureActiveOrganisationParentListView.ProjectionTables.FutureActiveOrganisationParentList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationOrganisationParentId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.ParentOrganisationId).IsRequired();

        b.Property(p => p.ValidFrom);

        b.HasIndex(x => x.ValidFrom);
    }
}

public class FutureActiveOrganisationParentListView :
    Projection<FutureActiveOrganisationParentListView>,
    IEventHandler<OrganisationParentAdded>,
    IEventHandler<OrganisationParentUpdated>,
    IEventHandler<ParentAssignedToOrganisation>
{
    private readonly IEventStore _eventStore;
    private readonly IDateTimeProvider _dateTimeProvider;
    public FutureActiveOrganisationParentListView(
        ILogger<FutureActiveOrganisationParentListView> logger,
        IEventStore eventStore,
        IDateTimeProvider dateTimeProvider,
        IContextFactory contextFactory
    ) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _dateTimeProvider = dateTimeProvider;
    }

    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        FutureActiveOrganisationParentList,
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
    {
        var validFrom = new ValidFrom(message.Body.ValidFrom);
        if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        InsertFutureActiveOrganisationParent(context, message);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var validFrom = new ValidFrom(message.Body.ValidFrom);
        if (validFrom.IsInPastOf(_dateTimeProvider.Today, true))
        {
            DeleteFutureActiveOrganisationParent(context, message.Body.OrganisationOrganisationParentId);
        }
        else
        {
            UpsertFutureActiveOrganisationParent(context, message);
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ParentAssignedToOrganisation> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        DeleteFutureActiveOrganisationParent(context, message.Body.OrganisationOrganisationParentId);
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    private static void InsertFutureActiveOrganisationParent(
        OrganisationRegistryContext context,
        IEnvelope<OrganisationParentAdded> message)
    {
        var futureActiveOrganisationParentListItem = new FutureActiveOrganisationParentListItem
        {
            OrganisationId = message.Body.OrganisationId,
            ParentOrganisationId = message.Body.ParentOrganisationId,
            OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
            ValidFrom = message.Body.ValidFrom,
        };

        context.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParentListItem);
        context.SaveChanges();
    }

    private static void UpsertFutureActiveOrganisationParent(
        OrganisationRegistryContext context,
        IEnvelope<OrganisationParentUpdated> message)
    {
        var futureActiveOrganisationParent =
            context.FutureActiveOrganisationParentList.SingleOrDefault(
                item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

        if (futureActiveOrganisationParent == null)
        {
            var futureActiveOrganisationParentListItem =
                new FutureActiveOrganisationParentListItem
                {
                    OrganisationId = message.Body.OrganisationId,
                    ParentOrganisationId = message.Body.ParentOrganisationId,
                    OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
                    ValidFrom = message.Body.ValidFrom,
                };

            context.FutureActiveOrganisationParentList.Add(futureActiveOrganisationParentListItem);
        }
        else
        {
            futureActiveOrganisationParent.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
            futureActiveOrganisationParent.OrganisationId = message.Body.OrganisationId;
            futureActiveOrganisationParent.ParentOrganisationId = message.Body.ParentOrganisationId;
            futureActiveOrganisationParent.ValidFrom = message.Body.ValidFrom;
        }

        context.SaveChanges();
    }

    private static void DeleteFutureActiveOrganisationParent(
        OrganisationRegistryContext context,
        Guid organisationOrganisationParentId)
    {
        var futureActiveOrganisationParent =
            context.FutureActiveOrganisationParentList.SingleOrDefault(
                item => item.OrganisationOrganisationParentId == organisationOrganisationParentId);

        if (futureActiveOrganisationParent == null)
            return;

        context.FutureActiveOrganisationParentList.Remove(futureActiveOrganisationParent);
        context.SaveChanges();
    }
}
