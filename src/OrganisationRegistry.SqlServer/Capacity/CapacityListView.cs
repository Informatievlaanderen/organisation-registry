namespace OrganisationRegistry.SqlServer.Capacity;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Capacity.Events;
using OrganisationRegistry.Infrastructure;

public class CapacityListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
    public bool IsRemoved { get; set; }
}

public class CapacityListConfiguration : EntityMappingConfiguration<CapacityListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<CapacityListItem> b)
    {
        b.ToTable(nameof(CapacityListView.ProjectionTables.CapacityList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class CapacityListView :
    Projection<CapacityListView>,
    IEventHandler<CapacityCreated>,
    IEventHandler<CapacityUpdated>,
    IEventHandler<CapacityRemoved>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        CapacityList
    }

    private readonly IEventStore _eventStore;

    public CapacityListView(
        ILogger<CapacityListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityCreated> message)
    {
        var capacity = new CapacityListItem
        {
            Id = message.Body.CapacityId,
            Name = message.Body.Name,
        };

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        await context.CapacityList.AddAsync(capacity);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var capacity = context.CapacityList.Single(x => x.Id == message.Body.CapacityId);

        capacity.Name = message.Body.Name;
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);

        var capacity = context.CapacityList.Single(x => x.Id == message.Body.CapacityId);

        capacity.IsRemoved = true;

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}