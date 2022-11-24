namespace OrganisationRegistry.SqlServer.Organisation;

using System;
using System.Data.Common;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using System.Linq;
using System.Threading.Tasks;
using Location;
using LocationType;
using Microsoft.Extensions.Logging;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Location.Events;
using OrganisationRegistry.LocationType.Events;

public class OrganisationLocationListItem
{
    public Guid OrganisationLocationId { get; set; }
    public Guid OrganisationId { get; set; }
    public Guid LocationId { get; set; }
    public string LocationName { get; set; } = null!;
    public Guid? LocationTypeId { get; set; }
    public string? LocationTypeName { get; set; }
    public bool IsMainLocation { get; set; }
    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
    public string? Source { get; set; }

    public bool IsKbo
        => Source == OrganisationRegistry.Organisation.LocationSource.Kbo;

    public OrganisationLocationListItem()
    {
    }

    public OrganisationLocationListItem(
        Guid organisationLocationId,
        Guid organisationId,
        Guid locationId,
        string locationName,
        bool isMainLocation,
        Guid? locationTypeId,
        string? locationTypeName,
        DateTime? validFrom,
        DateTime? validTo)
    {
        OrganisationLocationId = organisationLocationId;
        OrganisationId = organisationId;
        LocationId = locationId;
        LocationName = locationName;
        IsMainLocation = isMainLocation;
        LocationTypeId = locationTypeId;
        LocationTypeName = locationTypeName;
        ValidFrom = validFrom;
        ValidTo = validTo;
    }
}

public class OrganisationLocationListConfiguration : EntityMappingConfiguration<OrganisationLocationListItem>
{
    public override void Map(EntityTypeBuilder<OrganisationLocationListItem> b)
    {
        b.ToTable(nameof(OrganisationLocationListView.ProjectionTables.OrganisationLocationList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.OrganisationLocationId)
            .IsClustered(false);

        b.Property(p => p.OrganisationId).IsRequired();

        b.Property(p => p.LocationId).IsRequired();
        b.Property(p => p.LocationName).HasMaxLength(LocationListConfiguration.FormattedAddressLength).IsRequired();

        b.Property(p => p.LocationTypeId);
        b.Property(p => p.LocationTypeName).HasMaxLength(LocationTypeListConfiguration.NameLength);

        b.Property(p => p.IsMainLocation);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.Property(p => p.Source);

        b.HasIndex(x => x.LocationName).IsClustered();
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
        b.HasIndex(x => x.IsMainLocation);
    }
}

public class OrganisationLocationListView :
    Projection<OrganisationLocationListView>,
    IEventHandler<LocationUpdated>,
    IEventHandler<OrganisationLocationAdded>,
    IEventHandler<KboRegisteredOfficeLocationIsMainLocationChanged>,
    IEventHandler<KboRegisteredOfficeOrganisationLocationAdded>,
    IEventHandler<KboRegisteredOfficeOrganisationLocationRemoved>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<OrganisationTerminationSyncedWithKbo>,
    IEventHandler<OrganisationLocationUpdated>,
    IEventHandler<OrganisationLocationRemoved>,
    IEventHandler<LocationTypeUpdated>,
    IEventHandler<OrganisationTerminated>,
    IEventHandler<OrganisationTerminatedV2>
{
    private readonly IEventStore _eventStore;

    protected override string[] ProjectionTableNames
        => Enum.GetNames(typeof(ProjectionTables));

    public override string Schema
        => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        OrganisationLocationList,
    }

    public OrganisationLocationListView(
        ILogger<OrganisationLocationListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocations = context.OrganisationLocationList.Where(x => x.LocationId == message.Body.LocationId);
        if (!organisationLocations.Any())
            return;

        foreach (var organisationLocation in organisationLocations)
            organisationLocation.LocationName = message.Body.FormattedAddress;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationTypeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocations = context.OrganisationLocationList.Where(x => x.LocationTypeId == message.Body.LocationTypeId);
        if (!organisationLocations.Any())
            return;

        foreach (var organisationLocation in organisationLocations)
            organisationLocation.LocationTypeName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
    {
        var organisationLocationListItem = new OrganisationLocationListItem(
            message.Body.OrganisationLocationId,
            message.Body.OrganisationId,
            message.Body.LocationId,
            message.Body.LocationFormattedAddress,
            message.Body.IsMainLocation,
            message.Body.LocationTypeId,
            message.Body.LocationTypeName,
            message.Body.ValidFrom,
            message.Body.ValidTo);

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationLocationList.AddAsync(organisationLocationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeLocationIsMainLocationChanged> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocationListItem = context.OrganisationLocationList.Single(b => b.OrganisationLocationId == message.Body.OrganisationLocationId);

        organisationLocationListItem.IsMainLocation = message.Body.IsMainLocation;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationAdded> message)
    {
        var organisationLocationListItem = new OrganisationLocationListItem(
            message.Body.OrganisationLocationId,
            message.Body.OrganisationId,
            message.Body.LocationId,
            message.Body.LocationFormattedAddress,
            message.Body.IsMainLocation,
            message.Body.LocationTypeId,
            message.Body.LocationTypeName,
            message.Body.ValidFrom,
            message.Body.ValidTo);

        organisationLocationListItem.Source = OrganisationRegistry.Organisation.LocationSource.Kbo;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        await context.OrganisationLocationList.AddAsync(organisationLocationListItem);
        await context.SaveChangesAsync();
    }


    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocationListItem = context.OrganisationLocationList.Single(
            b =>
                b.OrganisationLocationId == message.Body.OrganisationLocationId);

        context.OrganisationLocationList.Remove(organisationLocationListItem);

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        if (message.Body.RegisteredOfficeOrganisationLocationIdToCancel == null)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocationListItem = context.OrganisationLocationList.Single(
            b =>
                b.OrganisationLocationId == message.Body.RegisteredOfficeOrganisationLocationIdToCancel);

        context.OrganisationLocationList.Remove(organisationLocationListItem);

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
    {
        if (message.Body.RegisteredOfficeOrganisationLocationIdToTerminate == null)
            return;

        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var registeredOfficeLocation = context.OrganisationLocationList.Single(
            b =>
                b.OrganisationLocationId == message.Body.RegisteredOfficeOrganisationLocationIdToTerminate);

        registeredOfficeLocation.ValidTo = message.Body.DateOfTermination;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocationListItem = context.OrganisationLocationList.Single(b => b.OrganisationLocationId == message.Body.OrganisationLocationId);

        organisationLocationListItem.IsMainLocation = message.Body.IsMainLocation;
        organisationLocationListItem.LocationId = message.Body.LocationId;
        organisationLocationListItem.LocationName = message.Body.LocationFormattedAddress;
        organisationLocationListItem.LocationTypeId = message.Body.LocationTypeId;
        organisationLocationListItem.LocationTypeName = message.Body.LocationTypeName;
        organisationLocationListItem.ValidFrom = message.Body.ValidFrom;
        organisationLocationListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var locations = context.OrganisationLocationList.Where(
            item =>
                message.Body.FieldsToTerminate.Locations.Keys.Contains(item.OrganisationLocationId));

        foreach (var location in locations)
            location.ValidTo = message.Body.FieldsToTerminate.Locations[location.OrganisationLocationId];

        if (message.Body.KboFieldsToTerminate.RegisteredOffice.HasValue)
        {
            var kboOrganisationRegisteredOffice =
                await context.OrganisationLocationList.SingleAsync(
                    item =>
                        message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Key == item.OrganisationLocationId);

            kboOrganisationRegisteredOffice.ValidTo = message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Value;
        }


        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var locations = context.OrganisationLocationList.Where(
            item =>
                message.Body.FieldsToTerminate.Locations.Keys.Contains(item.OrganisationLocationId));

        foreach (var location in locations)
            location.ValidTo = message.Body.FieldsToTerminate.Locations[location.OrganisationLocationId];

        if (message.Body.KboFieldsToTerminate.RegisteredOffice.HasValue)
        {
            var kboOrganisationRegisteredOffice =
                await context.OrganisationLocationList.SingleAsync(
                    item =>
                        message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Key == item.OrganisationLocationId);

            kboOrganisationRegisteredOffice.ValidTo = message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Value;
        }


        await context.SaveChangesAsync();
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationRemoved> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var organisationLocationListItem = context.OrganisationLocationList.Single(b => b.OrganisationLocationId == message.Body.OrganisationLocationId);

        context.Remove(organisationLocationListItem);

        await context.SaveChangesAsync();
    }
}
