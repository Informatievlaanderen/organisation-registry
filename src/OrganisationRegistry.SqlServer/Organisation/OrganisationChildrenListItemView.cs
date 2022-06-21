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
using OrganisationRegistry.Infrastructure.AppSpecific;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;

public class OrganisationChildListItem
{
    public Guid OrganisationOrganisationParentId { get; set; }

    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public string OvoNumber { get; set; } = null!;

    public Guid ParentOrganisationId { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public DateTime? OrganisationValidFrom { get; set; }
    public DateTime? OrganisationValidTo { get; set; }
}

public class OrganisationChildListConfiguration : EntityMappingConfiguration<OrganisationChildListItem>
{
    public override void Map(EntityTypeBuilder<OrganisationChildListItem> b)
    {
        b.ToTable(nameof(OrganisationChildListView.ProjectionTables.OrganisationChildList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationOrganisationParentId)
            .IsClustered(false);

        b.Property(p => p.Id).IsRequired();

        b.Property(p => p.Name).HasMaxLength(OrganisationListConfiguration.NameLength).IsRequired();

        b.Property(p => p.OvoNumber).IsRequired();

        b.Property(p => p.ParentOrganisationId).IsRequired();

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.Property(p => p.OrganisationValidFrom);
        b.Property(p => p.OrganisationValidTo);

        b.HasIndex(x => x.Name).IsClustered();
        b.HasIndex(x => x.OvoNumber);

        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
        b.HasIndex(x => x.OrganisationValidFrom);
        b.HasIndex(x => x.OrganisationValidTo);
    }
}

public class OrganisationChildListView :
    Projection<OrganisationChildListView>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationValidityUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<OrganisationParentAdded>,
    IEventHandler<OrganisationParentUpdated>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationChildList,
    }

    private readonly IEventStore _eventStore;
    private readonly IMemoryCaches _memoryCaches;
    public OrganisationChildListView(
        ILogger<OrganisationChildListView> logger,
        IEventStore eventStore,
        IMemoryCaches memoryCaches,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _memoryCaches = memoryCaches;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
            {
                organisation.Name = message.Body.Name;
                organisation.OvoNumber = message.Body.OvoNumber;
                organisation.OrganisationValidFrom = message.Body.ValidFrom;
                organisation.OrganisationValidTo = message.Body.ValidTo;
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
            {
                organisation.Name = message.Body.Name;
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationValidityUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
            {
                organisation.OrganisationValidFrom = message.Body.ValidFrom;
                organisation.OrganisationValidTo = message.Body.ValidTo;
            }

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
                organisation.Name = message.Body.Name;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
                organisation.Name = message.Body.NameBeforeKboCoupling;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentAdded> message)
    {
        var organisationParentListItem = new OrganisationChildListItem
        {
            OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId,
            Id = message.Body.OrganisationId,
            Name = _memoryCaches.OrganisationNames[message.Body.OrganisationId],
            OvoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId],
            ParentOrganisationId = message.Body.ParentOrganisationId,
            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
            OrganisationValidFrom = _memoryCaches.OrganisationValidFroms[message.Body.OrganisationId],
            OrganisationValidTo = _memoryCaches.OrganisationValidTos[message.Body.OrganisationId],
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.OrganisationChildrenList.AddAsync(organisationParentListItem);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationParentUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var key = await context.OrganisationChildrenList.SingleAsync(item => item.OrganisationOrganisationParentId == message.Body.OrganisationOrganisationParentId);

            key.OrganisationOrganisationParentId = message.Body.OrganisationOrganisationParentId;
            key.Id = message.Body.OrganisationId;
            key.Name = _memoryCaches.OrganisationNames[message.Body.OrganisationId];
            key.OvoNumber = _memoryCaches.OvoNumbers[message.Body.OrganisationId];
            key.ParentOrganisationId = message.Body.ParentOrganisationId;
            key.ValidFrom = message.Body.ValidFrom;
            key.ValidTo = message.Body.ValidTo;
            key.OrganisationValidFrom = _memoryCaches.OrganisationValidFroms[message.Body.OrganisationId];
            key.OrganisationValidTo = _memoryCaches.OrganisationValidTos[message.Body.OrganisationId];

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        if (!message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
            return;

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
                organisation.OrganisationValidTo = message.Body.FieldsToTerminate.OrganisationValidity;

            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        if (!message.Body.FieldsToTerminate.OrganisationValidity.HasValue)
            return;

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var organisations = context.OrganisationChildrenList.Where(x => x.Id == message.Body.OrganisationId);
            if (!organisations.Any())
                return;

            foreach (var organisation in organisations)
                organisation.OrganisationValidTo = message.Body.FieldsToTerminate.OrganisationValidity;

            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
