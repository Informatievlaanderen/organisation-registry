namespace OrganisationRegistry.SqlServer.Building;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Building.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;

public class BuildingListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;

    public int? VimId { get; set; }
}

public class BuildingListConfiguration : EntityMappingConfiguration<BuildingListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<BuildingListItem> b)
    {
        b.ToTable(nameof(BuildingListView.ProjectionTables.BuildingList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.Property(p => p.VimId);

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
        b.HasIndex(x => x.VimId);
    }
}

public class BuildingListView :
    Projection<BuildingListView>,
    IEventHandler<BuildingCreated>,
    IEventHandler<BuildingUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        BuildingList
    }

    private readonly IEventStore _eventStore;

    public BuildingListView(
        ILogger<BuildingListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingCreated> message)
    {
        var building = new BuildingListItem
        {
            Id = message.Body.BuildingId,
            Name = message.Body.Name,
            VimId = message.Body.VimId
        };

        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            await context.BuildingList.AddAsync(building);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var building = context.BuildingList.SingleOrDefault(x => x.Id == message.Body.BuildingId);
            if (building == null)
                return; // TODO: Error?

            building.Name = message.Body.Name;
            building.VimId = message.Body.VimId;
            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
