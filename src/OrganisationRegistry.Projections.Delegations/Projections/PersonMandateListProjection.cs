namespace OrganisationRegistry.Projections.Delegations.Projections
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Body.Events;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using SqlServer.Infrastructure;
    using SqlServer.Person;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;

    public class PersonMandateListProjection :
        BaseProjection<PersonMandateListProjection>,
        IEventHandler<AssignedPersonToBodySeat>,
        IEventHandler<ReassignedPersonToBodySeat>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<PersonAssignedToDelegation>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<PersonAssignedToDelegationRemoved>
    {
        private readonly Func<Owned<OrganisationRegistryContext>> _contextFactory;

        private readonly IMemoryCaches _memoryCaches;

        public PersonMandateListProjection(
            ILogger<PersonMandateListProjection> logger,
            Func<Owned<OrganisationRegistryContext>> contextFactory,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _contextFactory = contextFactory;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            using (var context = _contextFactory().Value)
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var personMandateListItem = new PersonMandateListItem
                {
                    PersonMandateId = Guid.NewGuid(),

                    BodyMandateId = message.Body.BodyMandateId,
                    DelegationAssignmentId = null,
                    BodyId = message.Body.BodyId,
                    BodyName = _memoryCaches.BodyNames[message.Body.BodyId],

                    BodySeatId = message.Body.BodySeatId,
                    BodySeatName = message.Body.BodySeatName,
                    BodySeatNumber = message.Body.BodySeatNumber,

                    PersonId = message.Body.PersonId,

                    PaidSeat = _memoryCaches.IsSeatPaid[message.Body.BodySeatId],

                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo,

                    BodyOrganisationId = organisationForBody.OrganisationId,
                    BodyOrganisationName = organisationForBody.OrganisationName
                };

                context.PersonMandateList.Add(personMandateListItem);
                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            using (var context = _contextFactory().Value)
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var personMandateListItem =
                    context.PersonMandateList.SingleOrDefault(item =>
                        item.BodyMandateId == message.Body.BodyMandateId &&
                        item.DelegationAssignmentId == null);

                personMandateListItem.BodyId = message.Body.BodyId;
                personMandateListItem.BodyName = _memoryCaches.BodyNames[message.Body.BodyId];

                personMandateListItem.BodySeatId = message.Body.BodySeatId;
                personMandateListItem.BodySeatName = message.Body.BodySeatName;
                personMandateListItem.BodySeatNumber = message.Body.BodySeatNumber;

                personMandateListItem.PersonId = message.Body.PersonId;

                personMandateListItem.PaidSeat = _memoryCaches.IsSeatPaid[message.Body.BodySeatId];

                personMandateListItem.ValidFrom = message.Body.ValidFrom;
                personMandateListItem.ValidTo = message.Body.ValidTo;

                personMandateListItem.BodyOrganisationId = organisationForBody.OrganisationId;
                personMandateListItem.BodyOrganisationName = organisationForBody.OrganisationName;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            using (var context = _contextFactory().Value)
            {
                var personMandates = context.PersonMandateList.Where(x => x.BodyId == message.Body.BodyId);
                if (!personMandates.Any())
                    return;

                foreach (var personMandate in personMandates)
                    personMandate.BodyName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            using (var context = _contextFactory().Value)
            {
                var personMandates = context.PersonMandateList.Where(x => x.BodySeatId == message.Body.BodySeatId);
                if (!personMandates.Any())
                    return;

                foreach (var personMandate in personMandates)
                {
                    personMandate.BodySeatName = message.Body.Name;
                    personMandate.PaidSeat = message.Body.PaidSeat;
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            if (message.Body.OrganisationId == message.Body.PreviousOrganisationId)
                return;

            using (var context = _contextFactory().Value)
            {
                var mandatesForUpdatedBody = context.PersonMandateList.Where(item => item.BodyId == message.Body.BodyId);

                foreach (var mandateListItem in mandatesForUpdatedBody)
                {
                    mandateListItem.BodyOrganisationId = message.Body.OrganisationId;
                    mandateListItem.BodyOrganisationName = message.Body.OrganisationName;
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            using (var context = _contextFactory().Value)
            {
                var organisationNames = context.PersonMandateList.Where(item => item.OrganisationId == message.Body.OrganisationId);

                foreach (var delegationListItem in organisationNames)
                    delegationListItem.OrganisationName = message.Body.Name;

                var bodyOrganisationNames = context.PersonMandateList.Where(item => item.BodyOrganisationId == message.Body.OrganisationId);

                foreach (var delegationListItem in bodyOrganisationNames)
                    delegationListItem.BodyOrganisationName = message.Body.Name;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            using (var context = _contextFactory().Value)
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var personMandateListItem = new PersonMandateListItem
                {
                    PersonMandateId = Guid.NewGuid(),

                    BodyMandateId = message.Body.BodyMandateId,
                    DelegationAssignmentId = message.Body.DelegationAssignmentId,
                    BodyId = message.Body.BodyId,
                    BodyName = _memoryCaches.BodyNames[message.Body.BodyId],

                    BodySeatId = message.Body.BodySeatId,
                    BodySeatName = _memoryCaches.BodySeatNames[message.Body.BodySeatId],
                    BodySeatNumber = _memoryCaches.BodySeatNumbers[message.Body.BodySeatId],

                    PersonId = message.Body.PersonId,

                    PaidSeat = _memoryCaches.IsSeatPaid[message.Body.BodySeatId],

                    ValidFrom = message.Body.ValidFrom,
                    ValidTo = message.Body.ValidTo,

                    BodyOrganisationId = organisationForBody.OrganisationId,
                    BodyOrganisationName = organisationForBody.OrganisationName
                };

                context.PersonMandateList.Add(personMandateListItem);
                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            using (var context = _contextFactory().Value)
            {
                var organisationForBody = GetOrganisationForBodyFromCache(context, message.Body.BodyId);

                var personMandateListItem =
                    context.PersonMandateList.SingleOrDefault(item =>
                        item.BodyMandateId == message.Body.BodyMandateId &&
                        item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                personMandateListItem.BodyId = message.Body.BodyId;
                personMandateListItem.BodyName = _memoryCaches.BodyNames[message.Body.BodyId];

                personMandateListItem.BodySeatId = message.Body.BodySeatId;
                personMandateListItem.BodySeatName = _memoryCaches.BodySeatNames[message.Body.BodySeatId];
                personMandateListItem.BodySeatNumber = _memoryCaches.BodySeatNumbers[message.Body.BodySeatId];

                personMandateListItem.PersonId = message.Body.PersonId;

                personMandateListItem.PaidSeat = _memoryCaches.IsSeatPaid[message.Body.BodySeatId];

                personMandateListItem.ValidFrom = message.Body.ValidFrom;
                personMandateListItem.ValidTo = message.Body.ValidTo;

                personMandateListItem.BodyOrganisationId = organisationForBody.OrganisationId;
                personMandateListItem.BodyOrganisationName = organisationForBody.OrganisationName;

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            using (var context = _contextFactory().Value)
            {
                var personMandateListItem =
                    context.PersonMandateList.SingleOrDefault(item =>
                        item.BodyMandateId == message.Body.BodyMandateId &&
                        item.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                context.PersonMandateList.Remove(personMandateListItem);
                context.SaveChanges();
            }
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

