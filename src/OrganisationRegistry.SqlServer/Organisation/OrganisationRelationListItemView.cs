namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRelationType;
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.OrganisationRelationType.Events;

public class OrganisationRelationListItem
{
    public Guid OrganisationRelationId { get; set; }
    public Guid OrganisationId { get; set; }

    public Guid RelationId { get; set; }
    public string RelationName { get; set; } = null!;
    public string RelationInverseName { get; set; } = null!;

    public Guid RelatedOrganisationId { get; set; }
    public string RelatedOrganisationName { get; set; } = null!;

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class OrganisationRelationListConfiguration : EntityMappingConfiguration<OrganisationRelationListItem>
{
    public override void Map(EntityTypeBuilder<OrganisationRelationListItem> b)
    {
        b.ToTable(nameof(OrganisationRelationListView.ProjectionTables.OrganisationRelationList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationRelationId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.RelatedOrganisationId).IsRequired();
        b.Property(p => p.RelatedOrganisationName).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

        b.Property(p => p.RelationId).IsRequired();
        b.Property(p => p.RelationName).HasMaxLength(OrganisationRelationTypeListConfiguration.NameLength).IsRequired();
        b.Property(p => p.RelationInverseName).HasMaxLength(OrganisationRelationTypeListConfiguration.NameLength).IsRequired();

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.RelationName);
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
    }
}

public class OrganisationRelationListView :
    Projection<OrganisationRelationListView>,
    IEventHandler<OrganisationRelationAdded>,
    IEventHandler<OrganisationRelationUpdated>,
    IEventHandler<OrganisationRelationTypeUpdated>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationRelationList
    }

    private readonly IEventStore _eventStore;
    private readonly IMemoryCaches _memoryCaches;

    public OrganisationRelationListView(
        ILogger<OrganisationRelationListView> logger,
        IEventStore eventStore,
        IMemoryCaches memoryCaches,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _memoryCaches = memoryCaches;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationTypeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationRelations = context.OrganisationRelationList.Where(x => x.RelationId == message.Body.OrganisationRelationTypeId);
        if (!organisationRelations.Any())
            return;

        foreach (var organisationRelation in organisationRelations)
        {
            organisationRelation.RelationName = message.Body.Name;
            organisationRelation.RelationInverseName = message.Body.InverseName;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationAdded> message)
    {
        var organisationRelationListItem = new OrganisationRelationListItem
        {
            OrganisationRelationId = message.Body.OrganisationRelationId,
            OrganisationId = message.Body.OrganisationId,

            RelatedOrganisationId = message.Body.RelatedOrganisationId,
            RelatedOrganisationName = _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId],

            RelationId = message.Body.RelationId,
            RelationName = message.Body.RelationName,
            RelationInverseName = message.Body.RelationInverseName,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo
        };

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationRelationList.AddAsync(organisationRelationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationRelationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var key = context.OrganisationRelationList.SingleOrDefault(item => item.OrganisationRelationId == message.Body.OrganisationRelationId);
        if (key == null)
            return;

        key.OrganisationRelationId = message.Body.OrganisationRelationId;
        key.OrganisationId = message.Body.OrganisationId;

        key.RelatedOrganisationId = message.Body.RelatedOrganisationId;
        key.RelatedOrganisationName = _memoryCaches.OrganisationNames[message.Body.RelatedOrganisationId];

        key.RelationId = message.Body.RelationId;
        key.RelationName = message.Body.RelationName;
        key.RelationInverseName = message.Body.RelationInverseName;
        key.ValidFrom = message.Body.ValidFrom;
        key.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
    {
        UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        await Task.CompletedTask;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
    {
        UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        await Task.CompletedTask;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.Name);
        await Task.CompletedTask;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        UpdateRelatedOrganisationName(dbConnection, dbTransaction, ContextFactory, message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        await Task.CompletedTask;
    }

    private static void UpdateRelatedOrganisationName(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IContextFactory contextFactory,
        Guid organisationId,
        string organisationName)
    {
        using var context = contextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationRelations = context.OrganisationRelationList.Where(x => x.RelatedOrganisationId == organisationId);
        if (!organisationRelations.Any())
            return;

        foreach (var organisationRelation in organisationRelations)
            organisationRelation.RelatedOrganisationName = organisationName;

        context.SaveChanges();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var relations = context.OrganisationRelationList.Where(item =>
            message.Body.FieldsToTerminate.Relations.Keys.Contains(item.OrganisationRelationId));

        foreach (var relation in relations)
            relation.ValidTo = message.Body.FieldsToTerminate.Relations[relation.OrganisationRelationId];

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var relations = context.OrganisationRelationList.Where(item =>
            message.Body.FieldsToTerminate.Relations.Keys.Contains(item.OrganisationRelationId));

        foreach (var relation in relations)
            relation.ValidTo = message.Body.FieldsToTerminate.Relations[relation.OrganisationRelationId];

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}