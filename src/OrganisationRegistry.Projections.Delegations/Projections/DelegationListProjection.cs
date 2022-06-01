namespace OrganisationRegistry.Projections.Delegations.Projections;

using System;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;
using Body.Events;
using OrganisationRegistry.Infrastructure.Events;
using Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Organisation.Events;
using OrganisationRegistry.Infrastructure;
using SqlServer.Delegations;
using SqlServer.Infrastructure;
using SqlServer.Person;
using OrganisationRegistry.Infrastructure.AppSpecific;
using SeatType.Events;
using SqlServer;

public class DelegationListProjection :
    Projection<DelegationListProjection>,
    IEventHandler<AssignedFunctionTypeToBodySeat>,
    IEventHandler<AssignedOrganisationToBodySeat>,
    IEventHandler<ReassignedFunctionTypeToBodySeat>,
    IEventHandler<ReassignedOrganisationToBodySeat>,
    IEventHandler<PersonAssignedToDelegation>,
    IEventHandler<PersonAssignedToDelegationRemoved>,
    IEventHandler<OrganisationInfoUpdated>,
    IEventHandler<OrganisationNameUpdated>,
    IEventHandler<OrganisationInfoUpdatedFromKbo>,
    IEventHandler<OrganisationCouplingWithKboCancelled>,
    IEventHandler<BodyInfoChanged>,
    IEventHandler<BodySeatNumberAssigned>,
    IEventHandler<BodySeatUpdated>,
    IEventHandler<SeatTypeUpdated>,
    IEventHandler<BodyAssignedToOrganisation>,
    IEventHandler<BodyClearedFromOrganisation>,
    IEventHandler<BodyOrganisationUpdated>,
    IEventHandler<InitialiseProjection>
{
    protected override string[] ProjectionTableNames =>
        new[]
        {
            DelegationListConfiguration.TableName,
            OrganisationPerBodyListConfiguration.TableName,
            PersonMandateListConfiguration.TableName
        };

    public override string Schema => WellknownSchemas.BackofficeSchema;

    private readonly IEventStore _eventStore;
    private readonly IMemoryCaches _memoryCaches;

    public DelegationListProjection(
        ILogger<DelegationListProjection> logger,
        IEventStore eventStore,
        IMemoryCaches memoryCaches,
        IContextFactory contextFactory) : base(logger, contextFactory)
    {
        _eventStore = eventStore;
        _memoryCaches = memoryCaches;
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedFunctionTypeToBodySeat> message)
    {
        await using var context = ContextFactory.Create();
        var organisationForBody = await GetOrganisationForBodyFromCache(context, message.Body.BodyId);
        var bodySeat = await context.BodySeatCacheForBodyMandateList.SingleAsync(item => item.BodySeatId == message.Body.BodySeatId);

        var delegationListItem = new DelegationListItem
        {
            Id = message.Body.BodyMandateId,

            OrganisationId = message.Body.OrganisationId,
            OrganisationName = message.Body.OrganisationName,

            FunctionTypeId = message.Body.FunctionTypeId,
            FunctionTypeName = message.Body.FunctionTypeName,

            BodyId = message.Body.BodyId,
            BodyName = _memoryCaches.BodyNames[message.Body.BodyId],

            BodyOrganisationId = organisationForBody.OrganisationId,
            BodyOrganisationName = organisationForBody.OrganisationName,

            BodySeatId = message.Body.BodySeatId,
            BodySeatName = message.Body.BodySeatName,
            BodySeatNumber = message.Body.BodySeatNumber,
            BodySeatTypeId = bodySeat.SeatTypeId,
            BodySeatTypeName = bodySeat.SeatTypeName,

            IsDelegated = false,

            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo
        };

        context.DelegationList.Add(delegationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedOrganisationToBodySeat> message)
    {
        await using var context = ContextFactory.Create();
        var organisationForBody = await GetOrganisationForBodyFromCache(context, message.Body.BodyId);
        var bodySeat = await context.BodySeatCacheForBodyMandateList.SingleAsync(item => item.BodySeatId == message.Body.BodySeatId);

        var delegationListItem = new DelegationListItem
        {
            Id = message.Body.BodyMandateId,

            OrganisationId = message.Body.OrganisationId,
            OrganisationName = message.Body.OrganisationName,

            FunctionTypeId = null,
            FunctionTypeName = string.Empty,

            BodyId = message.Body.BodyId,
            BodyName = _memoryCaches.BodyNames[message.Body.BodyId],

            BodyOrganisationId = organisationForBody.OrganisationId,
            BodyOrganisationName = organisationForBody.OrganisationName,

            BodySeatId = message.Body.BodySeatId,
            BodySeatName = message.Body.BodySeatName,
            BodySeatNumber = message.Body.BodySeatNumber,
            BodySeatTypeId = bodySeat.SeatTypeId,
            BodySeatTypeName = bodySeat.SeatTypeName,

            IsDelegated = false,

            ValidFrom = message.Body.ValidFrom,
            ValidTo = message.Body.ValidTo
        };

        context.DelegationList.Add(delegationListItem);
        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedFunctionTypeToBodySeat> message)
    {
        await using var context = ContextFactory.Create();
        var organisationForBody = await GetOrganisationForBodyFromCache(context, message.Body.BodyId);
        var bodySeat = await context.BodySeatCacheForBodyMandateList.SingleAsync(item => item.BodySeatId == message.Body.BodySeatId);

        var delegationListItem = await context.DelegationList.SingleAsync(item => item.Id == message.Body.BodyMandateId);

        delegationListItem.OrganisationId = message.Body.OrganisationId;
        delegationListItem.OrganisationName = message.Body.OrganisationName;

        delegationListItem.FunctionTypeId = message.Body.FunctionTypeId;
        delegationListItem.FunctionTypeName = message.Body.FunctionTypeName;

        delegationListItem.BodyId = message.Body.BodyId;
        delegationListItem.BodyName = _memoryCaches.BodyNames[message.Body.BodyId];

        delegationListItem.BodyOrganisationId = organisationForBody.OrganisationId;
        delegationListItem.BodyOrganisationName = organisationForBody.OrganisationName;

        delegationListItem.BodySeatId = message.Body.BodySeatId;
        delegationListItem.BodySeatName = message.Body.BodySeatName;
        delegationListItem.BodySeatNumber = message.Body.BodySeatNumber;
        delegationListItem.BodySeatTypeId = bodySeat.SeatTypeId;
        delegationListItem.BodySeatTypeName = bodySeat.SeatTypeName;

        delegationListItem.ValidFrom = message.Body.ValidFrom;
        delegationListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedOrganisationToBodySeat> message)
    {
        await using var context = ContextFactory.Create();
        var organisationForBody = await GetOrganisationForBodyFromCache(context, message.Body.BodyId);
        var bodySeat = await context.BodySeatCacheForBodyMandateList.SingleAsync(item => item.BodySeatId == message.Body.BodySeatId);

        var delegationListItem = await context.DelegationList.SingleAsync(item => item.Id == message.Body.BodyMandateId);

        delegationListItem.OrganisationId = message.Body.OrganisationId;
        delegationListItem.OrganisationName = message.Body.OrganisationName;

        delegationListItem.BodyId = message.Body.BodyId;
        delegationListItem.BodyName = _memoryCaches.BodyNames[message.Body.BodyId];

        delegationListItem.BodyOrganisationId = organisationForBody.OrganisationId;
        delegationListItem.BodyOrganisationName = organisationForBody.OrganisationName;

        delegationListItem.BodySeatId = message.Body.BodySeatId;
        delegationListItem.BodySeatName = message.Body.BodySeatName;
        delegationListItem.BodySeatNumber = message.Body.BodySeatNumber;
        delegationListItem.BodySeatTypeId = bodySeat.SeatTypeId;
        delegationListItem.BodySeatTypeName = bodySeat.SeatTypeName;


        delegationListItem.ValidFrom = message.Body.ValidFrom;
        delegationListItem.ValidTo = message.Body.ValidTo;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItem = await context.DelegationList.SingleAsync(item => item.Id == message.Body.BodyMandateId);

        delegationListItem.NumberOfDelegationAssignments++;
        delegationListItem.IsDelegated = delegationListItem.NumberOfDelegationAssignments > 0;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItem = await context.DelegationList.SingleAsync(item => item.Id == message.Body.BodyMandateId);

        delegationListItem.NumberOfDelegationAssignments--;
        delegationListItem.IsDelegated = delegationListItem.NumberOfDelegationAssignments > 0;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
    {
        await UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationNameUpdated> message)
    {
        await UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
    {
        await UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
    {
        await UpdateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
    }

    private async Task UpdateOrganisationName(Guid organisationId, string organisationName)
    {
        await using var context = ContextFactory.Create();
        var organisationNames =
            context.DelegationList.Where(item => item.OrganisationId == organisationId);

        foreach (var delegationListItem in organisationNames)
            delegationListItem.OrganisationName = organisationName;

        var bodyOrganisationNames =
            context.DelegationList.Where(item => item.BodyOrganisationId == organisationId);

        foreach (var delegationListItem in bodyOrganisationNames)
            delegationListItem.BodyOrganisationName = organisationName;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItems = context.DelegationList.Where(item => item.BodyId == message.Body.BodyId);

        foreach (var delegationListItem in delegationListItems)
            delegationListItem.BodyName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatNumberAssigned> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItems = context.DelegationList.Where(item => item.BodySeatId == message.Body.BodySeatId);

        foreach (var delegationListItem in delegationListItems)
            delegationListItem.BodySeatNumber = message.Body.BodySeatNumber;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItems = context.DelegationList.Where(item => item.BodySeatId == message.Body.BodySeatId);

        foreach (var delegationListItem in delegationListItems)
            delegationListItem.BodySeatName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
    {
        await using var context = ContextFactory.Create();
        var body = context.OrganisationPerBodyList.SingleOrDefault(x => x.BodyId == message.Body.BodyId);
        if (body == null)
        {
            body = new OrganisationPerBody
            {
                BodyId = message.Body.BodyId,
                BodyOrganisationId = message.Body.BodyOrganisationId,
                OrganisationId = message.Body.OrganisationId,
                OrganisationName = message.Body.OrganisationName,
            };

            context.OrganisationPerBodyList.Add(body);
        }
        else
        {
            body.BodyOrganisationId = message.Body.BodyOrganisationId;
            body.OrganisationId = message.Body.OrganisationId;
            body.OrganisationName = message.Body.OrganisationName;
        }

        foreach (var delegationListItem in context.DelegationList.Where(item => item.BodyId == message.Body.BodyId))
        {
            delegationListItem.BodyOrganisationId = message.Body.OrganisationId;
            delegationListItem.BodyOrganisationName = message.Body.OrganisationName;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
    {
        await using var context = ContextFactory.Create();
        var body = await context
            .OrganisationPerBodyList
            .SingleOrDefaultAsync(x => x.BodyId == message.Body.BodyId);

        if (body == null)
            return;

        context.OrganisationPerBodyList.Remove(body);

        foreach (var delegationListItem in context.DelegationList.Where(item => item.BodyId == message.Body.BodyId))
        {
            delegationListItem.BodyOrganisationId = null;
            delegationListItem.BodyOrganisationName = string.Empty;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
    {
        if (message.Body.OrganisationId == message.Body.PreviousOrganisationId)
            return;

        await using var context = ContextFactory.Create();
        var body = await context
            .OrganisationPerBodyList
            .SingleOrDefaultAsync(x =>
                x.BodyId == message.Body.BodyId &&
                x.BodyOrganisationId == message.Body.BodyOrganisationId);

        if (body == null)
            return;

        body.OrganisationId = message.Body.OrganisationId;
        body.OrganisationName = message.Body.OrganisationName;

        foreach (var delegationListItem in context.DelegationList.Where(item => item.BodyId == message.Body.BodyId))
        {
            delegationListItem.BodyOrganisationId = message.Body.OrganisationId;
            delegationListItem.BodyOrganisationName = message.Body.OrganisationName;
        }

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<SeatTypeUpdated> message)
    {
        await using var context = ContextFactory.Create();
        var delegationListItems = context.DelegationList.Where(item => item.BodySeatTypeId == message.Body.SeatTypeId);

        foreach (var delegationListItem in delegationListItems)
            delegationListItem.BodySeatName = message.Body.Name;

        await context.SaveChangesAsync();
    }

    public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
    {
        if (message.Body.ProjectionName != typeof(DelegationListProjection).FullName)
            return;

        Logger.LogInformation("Clearing tables for {ProjectionName}", message.Body.ProjectionName);

        await using var context = ContextFactory.Create();
        await context.Database.DeleteAllRows(Schema, ProjectionTableNames);
    }

    public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
    {
        await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
    }

    private static async Task<CachedOrganisationBody> GetOrganisationForBodyFromCache(OrganisationRegistryContext context, Guid bodyId)
    {
        var organisationPerBody = await context
            .OrganisationPerBodyList
            .SingleOrDefaultAsync(x => x.BodyId == bodyId);

        return organisationPerBody != null
            ? CachedOrganisationBody.FromCache(organisationPerBody)
            : CachedOrganisationBody.Empty();
    }
}
