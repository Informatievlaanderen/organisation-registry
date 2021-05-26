namespace OrganisationRegistry.Projections.Delegations.Projections
{
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
    using SqlServer.Delegations;
    using SqlServer.Infrastructure;
    using SqlServer.Person;
    using OrganisationRegistry.Infrastructure.AppSpecific;
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
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodySeatNumberAssigned>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<InitialiseProjection>
    {
        public override string[] ProjectionTableNames =>
            new[]
            {
                DelegationListConfiguration.TableName,
                OrganisationPerBodyListConfiguration.TableName,
                PersonMandateListConfiguration.TableName
            };

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
            using (var context = ContextFactory.Create())
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

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

                    IsDelegated = false,

                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo
                };

                context.DelegationList.Add(delegationListItem);
                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedOrganisationToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

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

                    IsDelegated = false,

                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo
                };

                context.DelegationList.Add(delegationListItem);
                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedFunctionTypeToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var delegationListItem = context.DelegationList.SingleOrDefault(item => item.Id == message.Body.BodyMandateId);

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

                delegationListItem.ValidFrom = message.Body.ValidFrom;
                delegationListItem.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedOrganisationToBodySeat> message)
        {
            using (var context = ContextFactory.Create())
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var delegationListItem = context.DelegationList.SingleOrDefault(item => item.Id == message.Body.BodyMandateId);

                delegationListItem.OrganisationId = message.Body.OrganisationId;
                delegationListItem.OrganisationName = message.Body.OrganisationName;

                delegationListItem.BodyId = message.Body.BodyId;
                delegationListItem.BodyName = _memoryCaches.BodyNames[message.Body.BodyId];

                delegationListItem.BodyOrganisationId = organisationForBody.OrganisationId;
                delegationListItem.BodyOrganisationName = organisationForBody.OrganisationName;

                delegationListItem.BodySeatId = message.Body.BodySeatId;
                delegationListItem.BodySeatName = message.Body.BodySeatName;
                delegationListItem.BodySeatNumber = message.Body.BodySeatNumber;

                delegationListItem.ValidFrom = message.Body.ValidFrom;
                delegationListItem.ValidTo = message.Body.ValidTo;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var delegationListItem = context.DelegationList.SingleOrDefault(item => item.Id == message.Body.BodyMandateId);

                delegationListItem.NumberOfDelegationAssignments++;
                delegationListItem.IsDelegated = delegationListItem.NumberOfDelegationAssignments > 0;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            using (var context = ContextFactory.Create())
            {
                var delegationListItem = context.DelegationList.SingleOrDefault(item => item.Id == message.Body.BodyMandateId);

                delegationListItem.NumberOfDelegationAssignments--;
                delegationListItem.IsDelegated = delegationListItem.NumberOfDelegationAssignments > 0;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.Name);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            UpdateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling);
        }

        private void UpdateOrganisationName(Guid organisationId, string organisationName)
        {
            using (var context = ContextFactory.Create())
            {
                var organisationNames =
                    context.DelegationList.Where(item => item.OrganisationId == organisationId);

                foreach (var delegationListItem in organisationNames)
                    delegationListItem.OrganisationName = organisationName;

                var bodyOrganisationNames =
                    context.DelegationList.Where(item => item.BodyOrganisationId == organisationId);

                foreach (var delegationListItem in bodyOrganisationNames)
                    delegationListItem.BodyOrganisationName = organisationName;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            using (var context = ContextFactory.Create())
            {
                var delegationListItems = context.DelegationList.Where(item => item.BodyId == message.Body.BodyId);

                foreach (var delegationListItem in delegationListItems)
                    delegationListItem.BodyName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatNumberAssigned> message)
        {
            using (var context = ContextFactory.Create())
            {
                var delegationListItems = context.DelegationList.Where(item => item.BodySeatId == message.Body.BodySeatId);

                foreach (var delegationListItem in delegationListItems)
                    delegationListItem.BodySeatNumber = message.Body.BodySeatNumber;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            using (var context = ContextFactory.Create())
            {
                var delegationListItems = context.DelegationList.Where(item => item.BodySeatId == message.Body.BodySeatId);

                foreach (var delegationListItem in delegationListItems)
                    delegationListItem.BodySeatName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var body = new OrganisationPerBody
                {
                    BodyId = message.Body.BodyId,
                    BodyOrganisationId = message.Body.BodyOrganisationId,
                    OrganisationId = message.Body.OrganisationId,
                    OrganisationName = message.Body.OrganisationName,
                };

                context.OrganisationPerBodyList.Add(body);

                foreach (var delegationListItem in context.DelegationList.Where(item => item.BodyId == message.Body.BodyId))
                {
                    delegationListItem.BodyOrganisationId = message.Body.OrganisationId;
                    delegationListItem.BodyOrganisationName = message.Body.OrganisationName;
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            using (var context = ContextFactory.Create())
            {
                var body = context
                    .OrganisationPerBodyList
                    .SingleOrDefault(x => x.BodyId == message.Body.BodyId);

                if (body == null)
                    return;

                context.OrganisationPerBodyList.Remove(body);

                foreach (var delegationListItem in context.DelegationList.Where(item => item.BodyId == message.Body.BodyId))
                {
                    delegationListItem.BodyOrganisationId = null;
                    delegationListItem.BodyOrganisationName = string.Empty;
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            if (message.Body.OrganisationId == message.Body.PreviousOrganisationId)
                return;

            using (var context = ContextFactory.Create())
            {
                var body = context
                    .OrganisationPerBodyList
                    .SingleOrDefault(x =>
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

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<InitialiseProjection> message)
        {
            if (message.Body.ProjectionName != typeof(DelegationListProjection).FullName)
                return;

            Logger.LogInformation("Clearing tables for {ProjectionName}.", message.Body.ProjectionName);

            using (var context = ContextFactory.Create())
                context.Database.ExecuteSqlRaw(
                    string.Concat(ProjectionTableNames.Select(tableName => $"DELETE FROM [OrganisationRegistry].[{tableName}];")));
        }

        public override async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<RebuildProjection> message)
        {
            await RebuildProjection(_eventStore, dbConnection, dbTransaction, message);
        }

        private static CachedOrganisationBody GetOrganisationForBodyFromCache(OrganisationRegistryContext context, Guid bodyId)
        {
            var organisationPerBody = context
                .OrganisationPerBodyList
                .SingleOrDefault(x => x.BodyId == bodyId);

            return organisationPerBody != null
                ? CachedOrganisationBody.FromCache(organisationPerBody)
                : CachedOrganisationBody.Empty();
        }
    }
}
