namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Linq;
using System.Data.Common;
using System.Threading.Tasks;
using Building;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using Infrastructure;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Building.Events;
using OrganisationRegistry.Infrastructure;

public class OrganisationBuildingListItem
{
    public Guid OrganisationBuildingId { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid BuildingId { get; set; }
    public string BuildingName { get; set; }
    public bool IsMainBuilding { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }

    public OrganisationBuildingListItem()
    {
        BuildingName = string.Empty;
    }

    public OrganisationBuildingListItem(Guid organisationBuildingId, Guid organisationId, Guid buildingId, string buildingName, bool isMainBuilding, DateTime? validFrom, DateTime? validTo)
    {
        OrganisationBuildingId = organisationBuildingId;
        OrganisationId = organisationId;
        BuildingId = buildingId;
        BuildingName = buildingName;
        IsMainBuilding = isMainBuilding;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}

public class OrganisationBuildingListConfiguration : EntityMappingConfiguration<OrganisationBuildingListItem>
{
    public override void Map(EntityTypeBuilder<OrganisationBuildingListItem> b)
    {
        b.ToTable(nameof(OrganisationBuildingListView.ProjectionTables.OrganisationBuildingList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationBuildingId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.BuildingId).IsRequired();
        b.Property(p => p.BuildingName).HasMaxLength(BuildingListConfiguration.NameLength).IsRequired();

        b.Property(p => p.IsMainBuilding);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.BuildingName).IsClustered();
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
        b.HasIndex(x => x.IsMainBuilding);
    }
}

public class OrganisationBuildingListView :
    Projection<OrganisationBuildingListView>,
    IEventHandler<BuildingUpdated>,
    IEventHandler<OrganisationBuildingAdded>,
    IEventHandler<OrganisationBuildingUpdated>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>
{
    protected override string[] ProjectionTableNames => Enum.GetNames(typeof(ProjectionTables));
    public override string Schema => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationBuildingList,
    }

    private readonly IEventStore _eventStore;

    public OrganisationBuildingListView(
        ILogger<OrganisationBuildingListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationBuildings = context.OrganisationBuildingList.Where(x => x.BuildingId == message.Body.BuildingId);
        if (!organisationBuildings.Any())
            return;

        foreach (var organisationBuilding in organisationBuildings)
            organisationBuilding.BuildingName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingAdded> message)
    {
        var organisationBuildingListItem = new OrganisationBuildingListItem(
            message.Body.OrganisationBuildingId,
            message.Body.OrganisationId,
            message.Body.BuildingId,
            message.Body.BuildingName,
            message.Body.IsMainBuilding,
            message.Body.ValidFrom,
            message.Body.ValidTo);

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationBuildingList.AddAsync(organisationBuildingListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationBuildingListItem = context.OrganisationBuildingList.Single(b => b.OrganisationBuildingId == message.Body.OrganisationBuildingId);

        organisationBuildingListItem.IsMainBuilding = message.Body.IsMainBuilding;
        organisationBuildingListItem.BuildingId = message.Body.BuildingId;
        organisationBuildingListItem.BuildingName = message.Body.BuildingName;
        organisationBuildingListItem.ValidFrom = message.Body.ValidFrom;
        organisationBuildingListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var buildings = context.OrganisationBuildingList.Where(item =>
            message.Body.FieldsToTerminate.Buildings.Keys.Contains(item.OrganisationBuildingId));

        foreach (var building in buildings)
            building.ValidTo = message.Body.FieldsToTerminate.Buildings[building.OrganisationBuildingId];

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var buildings = context.OrganisationBuildingList.Where(item =>
            message.Body.FieldsToTerminate.Buildings.Keys.Contains(item.OrganisationBuildingId));

        foreach (var building in buildings)
            building.ValidTo = message.Body.FieldsToTerminate.Buildings[building.OrganisationBuildingId];

        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }
}
