namespace OrganisationRegistry.SqlServer.LocationType;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore;
using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.LocationType.Events;

public class LocationTypeListItem
{
    public Guid Id { get; set; }

    public string Name { get; set; } = null!;
}

public class LocationTypeListConfiguration : EntityMappingConfiguration<LocationTypeListItem>
{
    public const int NameLength = 500;

    public override void Map(EntityTypeBuilder<LocationTypeListItem> b)
    {
        b.ToTable(nameof(LocationTypeListView.ProjectionTables.LocationTypeList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.Id)
            .IsClustered(false);

        b.Property(p => p.Name)
            .HasMaxLength(NameLength)
            .IsRequired();

        b.HasIndex(x => x.Name).IsUnique().IsClustered();
    }
}

public class LocationTypeListView :
    Projection<LocationTypeListView>,
    IEventHandler<LocationTypeCreated>,
    IEventHandler<LocationTypeUpdated>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        LocationTypeList
    }

    private readonly IEventStore _eventStore;

    public LocationTypeListView(
        ILogger<LocationTypeListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationTypeCreated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var locationType = new LocationTypeListItem
            {
                Id = message.Body.LocationTypeId,
                Name = message.Body.Name,
            };

            await context.LocationTypeList.AddAsync(locationType);
            await context.SaveChangesAsync();
        }
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationTypeUpdated> message)
    {
        using (var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction))
        {
            var locationType = context.LocationTypeList.SingleOrDefault(x => x.Id == message.Body.LocationTypeId);
            if (locationType == null)
                return; // TODO: Error?

            locationType.Name = message.Body.Name;
            await context.SaveChangesAsync();
        }
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
