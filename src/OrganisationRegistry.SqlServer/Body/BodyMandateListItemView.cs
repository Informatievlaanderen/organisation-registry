namespace OrganisationRegistry.SqlServer.Body;

using Function.Events;
using FunctionType;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Organisation;
using Person;
using SeatType;
using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using OrganisationRegistry.Body;
using OrganisationRegistry.Body.Events;
using OrganisationRegistry.Infrastructure;
using OrganisationRegistry.Infrastructure.Events;
using OrganisationRegistry.Organisation.Events;
using OrganisationRegistry.Person.Events;
using OrganisationRegistry.SeatType.Events;

public class BodyMandateListItem
{
    public Guid BodyMandateId { get; set; }
    public BodyMandateType BodyMandateType { get; set; }
    public Guid BodyId { get; set; }

    public Guid BodySeatId { get; set; }
    public string BodySeatNumber { get; set; } = null!;
    public string BodySeatName { get; set; } = null!;

    public Guid? BodySeatTypeId { get; set; }
    public string? BodySeatTypeName { get; set; }
    public int? BodySeatTypeOrder { get; set; }

    public Guid DelegatorId { get; set; }
    public string DelegatorName { get; set; } = null!;

    public Guid? DelegatedId { get; set; }
    public string? DelegatedName { get; set; }

    public Guid? AssignedToId { get; set; }
    public string? AssignedToName { get; set; }

    public string? ContactsJson { get; set; }

    public DateTime? ValidFrom { get; set; }
    public DateTime? ValidTo { get; set; }
}

public class BodySeatCacheItemForBodyMandateList
{
    public Guid BodySeatId { get; set; }

    public Guid SeatTypeId { get; set; }
    public string SeatTypeName { get; set; } = null!;
}

public class BodyMandateListConfiguration : EntityMappingConfiguration<BodyMandateListItem>
{
    public readonly int RepresentationLength = new[]
    {
        OrganisationListConfiguration.NameLength,
        FunctionTypeListConfiguration.NameLength,
        PersonListConfiguration.FullNameLength,
    }.Max();

    public override void Map(EntityTypeBuilder<BodyMandateListItem> b)
    {
        b.ToTable(nameof(BodyMandateListView.ProjectionTables.BodyMandateList), WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.BodyMandateId)
            .IsClustered(false);

        b.Property(p => p.BodyMandateType).IsRequired();

        b.Property(p => p.BodyId).IsRequired();

        b.Property(p => p.BodySeatId).IsRequired();

        b.Property(p => p.BodySeatNumber)
            .HasMaxLength(BodySeatListConfiguration.SeatNumberLength)
            .IsRequired();

        b.Property(p => p.BodySeatName)
            .HasMaxLength(BodySeatListConfiguration.NameLength)
            .IsRequired();

        b.Property(p => p.BodySeatTypeId);

        b.Property(p => p.BodySeatTypeName)
            .HasMaxLength(SeatTypeListConfiguration.NameLength);

        b.Property(p => p.BodySeatTypeOrder);

        b.Property(p => p.DelegatorId).IsRequired();
        b.Property(p => p.DelegatorName)
            .HasMaxLength(RepresentationLength)
            .IsRequired();

        b.Property(p => p.DelegatedId);
        b.Property(p => p.DelegatedName)
            .HasMaxLength(RepresentationLength);

        b.Property(p => p.AssignedToId);
        b.Property(p => p.AssignedToName)
            .HasMaxLength(PersonListConfiguration.FullNameLength);

        b.Property(p => p.ContactsJson);

        b.Property(p => p.ValidFrom);
        b.Property(p => p.ValidTo);

        b.HasIndex(x => x.DelegatorName).IsClustered();
        b.HasIndex(x => x.ValidFrom);
        b.HasIndex(x => x.ValidTo);
        b.HasIndex(x => x.BodySeatName);
    }
}

public class
    BodySeatCacheForBodyMandateListConfiguration : EntityMappingConfiguration<BodySeatCacheItemForBodyMandateList>
{
    public override void Map(EntityTypeBuilder<BodySeatCacheItemForBodyMandateList> b)
    {
        b.ToTable(
                nameof(BodyMandateListView.ProjectionTables.BodySeatCacheForBodyMandateList),
                WellknownSchemas.BackofficeSchema)
            .HasKey(p => p.BodySeatId)
            .IsClustered(false);

        b.Property(p => p.BodySeatId).IsRequired();
        b.Property(p => p.SeatTypeId).IsRequired();
        b.Property(p => p.SeatTypeName)
            .HasMaxLength(SeatTypeListConfiguration.NameLength)
            .IsRequired();
    }
}

public class BodyMandateListView :
    Projection<BodyMandateListView>,
    IEventHandler<AssignedPersonToBodySeat>,
    IEventHandler<AssignedFunctionTypeToBodySeat>,
    IEventHandler<AssignedOrganisationToBodySeat>,
    IEventHandler<ReassignedPersonToBodySeat>,
    IEventHandler<ReassignedFunctionTypeToBodySeat>,
    IEventHandler<ReassignedOrganisationToBodySeat>,
    IEventHandler<PersonUpdated>,
    IEventHandler<FunctionUpdated>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<BodySeatUpdated>,
    IEventHandler<BodySeatAdded>,
    IEventHandler<SeatTypeUpdated>,
    IEventHandler<BodySeatNumberAssigned>,
    IEventHandler<AssignedPersonAssignedToBodyMandate>,
    IEventHandler<AssignedPersonClearedFromBodyMandate>
{
    protected override string[] ProjectionTableNames
        => Enum.GetNames(typeof(ProjectionTables));

    public override string Schema
        => WellknownSchemas.BackofficeSchema;

    public enum ProjectionTables
    {
        BodyMandateList,
        BodySeatCacheForBodyMandateList,
    }

    private readonly IEventStore _eventStore;

    public BodyMandateListView(
        ILogger<BodyMandateListView> logger,
        IEventStore eventStore,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<AssignedPersonToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);

        var bodyMandateListItem = new BodyMandateListItem
        {
            BodyMandateId = message.Body.BodyMandateId,
            BodyMandateType = BodyMandateType.Person,
            BodyId = message.Body.BodyId,

            BodySeatId = message.Body.BodySeatId,
            BodySeatName = message.Body.BodySeatName,
            BodySeatNumber = message.Body.BodySeatNumber,

            BodySeatTypeId = bodySeat.SeatTypeId,
            BodySeatTypeName = bodySeat.SeatTypeName,
            BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

            DelegatorId = message.Body.PersonId,
            DelegatorName = FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName),
            DelegatedId = null,
            DelegatedName = null,

            ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts),

            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };

        await context.BodyMandateList.AddAsync(bodyMandateListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<AssignedFunctionTypeToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);

        var bodyMandateListItem = new BodyMandateListItem
        {
            BodyMandateId = message.Body.BodyMandateId,
            BodyMandateType = BodyMandateType.FunctionType,
            BodyId = message.Body.BodyId,

            BodySeatId = message.Body.BodySeatId,
            BodySeatName = message.Body.BodySeatName,
            BodySeatNumber = message.Body.BodySeatNumber,

            BodySeatTypeId = bodySeat.SeatTypeId,
            BodySeatTypeName = bodySeat.SeatTypeName,

            BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

            DelegatorId = message.Body.OrganisationId,
            DelegatorName = message.Body.OrganisationName,
            DelegatedId = message.Body.FunctionTypeId,
            DelegatedName = message.Body.FunctionTypeName,

            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };

        await context.BodyMandateList.AddAsync(bodyMandateListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<AssignedOrganisationToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);

        var bodyMandateListItem = new BodyMandateListItem
        {
            BodyMandateId = message.Body.BodyMandateId,
            BodyMandateType = BodyMandateType.Organisation,
            BodyId = message.Body.BodyId,

            BodySeatId = message.Body.BodySeatId,
            BodySeatName = message.Body.BodySeatName,
            BodySeatNumber = message.Body.BodySeatNumber,

            BodySeatTypeId = bodySeat.SeatTypeId,
            BodySeatTypeName = bodySeat.SeatTypeName,
            BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue,

            DelegatorId = message.Body.OrganisationId,
            DelegatorName = message.Body.OrganisationName,
            DelegatedId = null,
            DelegatedName = null,

            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo,
        };
        await context.BodyMandateList.AddAsync(bodyMandateListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<ReassignedPersonToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandateListItem =
            await context.BodyMandateList.SingleAsync(item => item.BodyMandateId == message.Body.BodyMandateId);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);

        bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
        bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
        bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

        bodyMandateListItem.BodySeatTypeId = bodySeat.SeatTypeId;
        bodyMandateListItem.BodySeatTypeName = bodySeat.SeatTypeName;
        bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

        bodyMandateListItem.DelegatorId = message.Body.PersonId;
        bodyMandateListItem.DelegatorName = FormatPersonName(message.Body.PersonFirstName, message.Body.PersonName);
        bodyMandateListItem.DelegatedId = null;
        bodyMandateListItem.DelegatedName = null;

        bodyMandateListItem.ContactsJson = JsonConvert.SerializeObject(message.Body.Contacts);
        bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
        bodyMandateListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<ReassignedFunctionTypeToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandateListItem =
            context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);

        bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
        bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
        bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

        bodyMandateListItem.BodySeatTypeId = bodySeat.SeatTypeId;
        bodyMandateListItem.BodySeatTypeName = bodySeat.SeatTypeName;
        bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

        bodyMandateListItem.DelegatorId = message.Body.OrganisationId;
        bodyMandateListItem.DelegatorName = message.Body.OrganisationName;
        bodyMandateListItem.DelegatedId = message.Body.FunctionTypeId;
        bodyMandateListItem.DelegatedName = message.Body.FunctionTypeName;

        bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
        bodyMandateListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<ReassignedOrganisationToBodySeat> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandateListItem =
            context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);
        var bodySeat =
            await context.BodySeatCacheForBodyMandateList.SingleAsync(
                item => item.BodySeatId == message.Body.BodySeatId);


        bodyMandateListItem.BodySeatId = message.Body.BodySeatId;
        bodyMandateListItem.BodySeatName = message.Body.BodySeatName;
        bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

        bodyMandateListItem.BodySeatTypeId = bodySeat.SeatTypeId;
        bodyMandateListItem.BodySeatTypeName = bodySeat.SeatTypeName;
        bodyMandateListItem.BodySeatTypeOrder = message.Body.BodySeatTypeOrder ?? int.MaxValue;

        bodyMandateListItem.DelegatorId = message.Body.OrganisationId;
        bodyMandateListItem.DelegatorName = message.Body.OrganisationName;
        bodyMandateListItem.DelegatedId = null;
        bodyMandateListItem.DelegatedName = null;

        bodyMandateListItem.ValidFrom = message.Body.ValidFrom;
        bodyMandateListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<AssignedPersonAssignedToBodyMandate> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandateListItem =
            context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

        bodyMandateListItem.AssignedToId = message.Body.PersonId;
        bodyMandateListItem.AssignedToName = message.Body.PersonFullName;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<AssignedPersonClearedFromBodyMandate> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandateListItem =
            context.BodyMandateList.Single(item => item.BodyMandateId == message.Body.BodyMandateId);

        bodyMandateListItem.AssignedToId = null;
        bodyMandateListItem.AssignedToName = string.Empty;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<PersonUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(
                item => item.BodyMandateType == BodyMandateType.Person &&
                        item.DelegatorId == message.Body.PersonId);

        foreach (var bodyMandateListItem in bodyMandates)
            bodyMandateListItem.DelegatorName = FormatPersonName(message.Body.FirstName, message.Body.Name);

        var bodyDelegationAssignments = context
            .BodyMandateList
            .Where(item => item.AssignedToId == message.Body.PersonId);

        foreach (var bodyDelegationAssignment in bodyDelegationAssignments)
            bodyDelegationAssignment.AssignedToName = FormatPersonName(message.Body.FirstName, message.Body.Name);

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<FunctionUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(
                item => item.BodyMandateType == BodyMandateType.FunctionType &&
                        item.DelegatedId == message.Body.FunctionId);

        foreach (var bodyMandateListItem in bodyMandates)
            bodyMandateListItem.DelegatedName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdated> message)
    {
        await UpdateDelegatorName(
            dbConnection,
            dbTransaction,
            ContextFactory,
            message.Body.OrganisationId,
            message.Body.Name);
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationNameUpdated> message)
    {
        await UpdateDelegatorName(
            dbConnection,
            dbTransaction,
            ContextFactory,
            message.Body.OrganisationId,
            message.Body.Name);
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        await UpdateDelegatorName(
            dbConnection,
            dbTransaction,
            ContextFactory,
            message.Body.OrganisationId,
            message.Body.Name);
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        await UpdateDelegatorName(
            dbConnection,
            dbTransaction,
            ContextFactory,
            message.Body.OrganisationId,
            message.Body.NameBeforeKboCoupling);
    }

    private static async Task UpdateDelegatorName(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IContextFactory contextFactory,
        Guid organisationId,
        string delegatorName)
    {
        await using var context = contextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(
                item =>
                    (item.BodyMandateType == BodyMandateType.Organisation ||
                     item.BodyMandateType == BodyMandateType.FunctionType) &&
                    item.DelegatorId == organisationId);

        foreach (var bodyMandateListItem in bodyMandates)
            bodyMandateListItem.DelegatorName = delegatorName;

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<BodySeatAdded> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodySeat = new BodySeatCacheItemForBodyMandateList
        {
            BodySeatId = message.Body.BodySeatId,
            SeatTypeId = message.Body.SeatTypeId,
            SeatTypeName = message.Body.SeatTypeName,
        };

        await context.BodySeatCacheForBodyMandateList.AddAsync(bodySeat);
        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<BodySeatUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(item => item.BodySeatId == message.Body.BodySeatId);

        foreach (var bodyMandateListItem in bodyMandates)
        {
            bodyMandateListItem.BodySeatName = message.Body.Name;
            bodyMandateListItem.BodySeatTypeId = message.Body.SeatTypeId;
            bodyMandateListItem.BodySeatTypeName = message.Body.SeatTypeName;
            bodyMandateListItem.BodySeatTypeOrder = message.Body.SeatTypeOrder ?? int.MaxValue;
        }

        foreach (var cacheItem in context.BodySeatCacheForBodyMandateList.Where(
                     item => item.BodySeatId == message.Body.BodySeatId))
        {
            cacheItem.SeatTypeId = message.Body.SeatTypeId;
            cacheItem.SeatTypeName = message.Body.SeatTypeName;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<SeatTypeUpdated> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(item => item.BodySeatTypeId == message.Body.SeatTypeId);

        foreach (var bodyMandateListItem in bodyMandates)
        {
            bodyMandateListItem.BodySeatTypeName = message.Body.Name;
            bodyMandateListItem.BodySeatTypeOrder = message.Body.Order ?? int.MaxValue;
        }

        foreach (var cachedItem in context.BodySeatCacheForBodyMandateList.Where(
                     x => x.SeatTypeId == message.Body.SeatTypeId))
        {
            cachedItem.SeatTypeName = message.Body.Name;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<BodySeatNumberAssigned> message)
    {
        await using var context = ContextFactory.CreateTransactional(dbConnection, dbTransaction);
        var bodyMandates = context
            .BodyMandateList
            .Where(item => item.BodySeatId == message.Body.BodySeatId);

        foreach (var bodyMandateListItem in bodyMandates)
            bodyMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

        await context.SaveChangesAsync();
    }

    public override async Task Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    private static string FormatPersonName(string firstName, string name)
        => $"{firstName} {name}";
}
