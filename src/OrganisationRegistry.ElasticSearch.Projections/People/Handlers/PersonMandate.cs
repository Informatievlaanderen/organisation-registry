namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Cache;
    using Client;
    using Common;
    using ElasticSearch.People;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Organisation.Events;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Body.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class PersonMandate :
        Infrastructure.BaseProjection<PersonMandate>,
        IEventHandler<AssignedPersonToBodySeat>,
        IEventHandler<ReassignedPersonToBodySeat>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodyOrganisationUpdated>,
        IEventHandler<BodySeatUpdated>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<PersonAssignedToDelegation>,
        IEventHandler<PersonAssignedToDelegationUpdated>,
        IEventHandler<PersonAssignedToDelegationRemoved>,
        IEventHandler<BodyAssignedToOrganisation>,
        IEventHandler<BodyClearedFromOrganisation>
    {
        private readonly Elastic _elastic;
        private readonly IContextFactory _contextFactory;

        public PersonMandate(
            ILogger<PersonMandate> logger,
            Elastic elastic,
            IContextFactory contextFactory) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
        }


        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<AssignedPersonToBodySeat> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var bodySeat = await organisationRegistryContext.BodySeatCache.SingleAsync(x => x.Id == message.Body.BodySeatId);

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Mandates == null)
                personDocument.Mandates = new List<PersonDocument.PersonMandate>();

            personDocument.Mandates.RemoveExistingListItems(x =>
                x.BodyMandateId == message.Body.BodyMandateId &&
                x.DelegationAssignmentId == null);

            var organisationForBody = GetOrganisationForBodyFromCache(message.Body.BodyId);

            personDocument.Mandates.Add(
                new PersonDocument.PersonMandate(
                    message.Body.BodyMandateId,
                    null,
                    message.Body.BodyId,
                    bodySeat.Name,
                    organisationForBody.OrganisationId,
                    organisationForBody.OrganisationName,
                    message.Body.BodySeatId,
                    message.Body.BodySeatName,
                    message.Body.BodySeatNumber,
                    bodySeat.IsPaid,
                    new Period(
                        message.Body.ValidFrom,
                        message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<ReassignedPersonToBodySeat> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var bodySeat = await organisationRegistryContext.BodySeatCache.SingleAsync(x => x.Id == message.Body.BodySeatId);

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                var previousPersonDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PreviousPersonId).ThrowOnFailure().Source);
                previousPersonDocument.ChangeId = message.Number;
                previousPersonDocument.ChangeTime = message.Timestamp;

                if (previousPersonDocument.Mandates == null)
                    previousPersonDocument.Mandates = new List<PersonDocument.PersonMandate>();

                previousPersonDocument.Mandates.RemoveExistingListItems(x =>
                    x.BodyMandateId == message.Body.BodyMandateId &&
                    x.DelegationAssignmentId == null);

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousPersonDocument).ThrowOnFailure());
            }

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Mandates == null)
                personDocument.Mandates = new List<PersonDocument.PersonMandate>();

            personDocument.Mandates.RemoveExistingListItems(x =>
                x.BodyMandateId == message.Body.BodyMandateId &&
                x.DelegationAssignmentId == null);

            var organisationForBody = GetOrganisationForBodyFromCache(message.Body.BodyId);

            personDocument.Mandates.Add(
                new PersonDocument.PersonMandate(
                    message.Body.BodyMandateId,
                    null,
                    message.Body.BodyId,
                    bodySeat.Name,
                    organisationForBody.OrganisationId,
                    organisationForBody.OrganisationName,
                    message.Body.BodySeatId,
                    message.Body.BodySeatName,
                    message.Body.BodySeatNumber,
                    bodySeat.IsPaid,
                    new Period(
                        message.Body.ValidFrom,
                        message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "bodyId",
                    "bodyName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "bodyId",
                    "bodyOrganisationId", message.Body.OrganisationId,
                    message.Number,
                    message.Timestamp));

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "bodyId",
                    "bodyOrganisationName", organisation.Id,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodySeatUpdated> message)
        {
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodySeatId, message.Body.BodySeatId,
                    "mandates", "bodySeatId",
                    "bodySeatName", message.Body.Name,
                    message.Number,
                    message.Timestamp));

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodySeatId, message.Body.BodySeatId,
                    "mandates", "bodySeatId",
                    "paidSeat", message.Body.PaidSeat,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            MassUpdateMandateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private void MassUpdateMandateOrganisationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyOrganisationId, organisationId,
                    "mandates", "bodyOrganisationId",
                    "bodyOrganisationName", name,
                    messageNumber,
                    timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegation> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var bodySeat = await organisationRegistryContext.BodySeatCache.SingleAsync(x => x.Id == message.Body.BodySeatId);
            var body = await organisationRegistryContext.BodyCache.SingleAsync(x => x.Id == message.Body.BodyId);

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Mandates == null)
                personDocument.Mandates = new List<PersonDocument.PersonMandate>();

            personDocument.Mandates.RemoveExistingListItems(x =>
                x.BodyMandateId == message.Body.BodyMandateId &&
                x.DelegationAssignmentId == message.Body.DelegationAssignmentId);

            var organisationForBody = GetOrganisationForBodyFromCache(message.Body.BodyId);

            personDocument.Mandates.Add(
                new PersonDocument.PersonMandate(
                    message.Body.BodyMandateId,
                    message.Body.DelegationAssignmentId,
                    message.Body.BodyId,
                    body.Name,
                    organisationForBody.OrganisationId,
                    organisationForBody.OrganisationName,
                    message.Body.BodySeatId,
                    bodySeat.Name,
                    bodySeat.Number,
                    bodySeat.IsPaid,
                    new Period(
                        message.Body.ValidFrom,
                        message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationUpdated> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var bodySeat = await organisationRegistryContext.BodySeatCache.SingleAsync(x => x.Id == message.Body.BodySeatId);
            var body = await organisationRegistryContext.BodyCache.SingleAsync(x => x.Id == message.Body.BodyId);

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId != message.Body.PersonId)
            {
                var previousPersonDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PreviousPersonId).ThrowOnFailure().Source);
                previousPersonDocument.ChangeId = message.Number;
                previousPersonDocument.ChangeTime = message.Timestamp;

                if (previousPersonDocument.Mandates == null)
                    previousPersonDocument.Mandates = new List<PersonDocument.PersonMandate>();

                previousPersonDocument.Mandates.RemoveExistingListItems(x =>
                    x.BodyMandateId == message.Body.BodyMandateId &&
                    x.DelegationAssignmentId == message.Body.DelegationAssignmentId);

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousPersonDocument).ThrowOnFailure());
            }

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Mandates == null)
                personDocument.Mandates = new List<PersonDocument.PersonMandate>();

            personDocument.Mandates.RemoveExistingListItems(x =>
                x.BodyMandateId == message.Body.BodyMandateId &&
                x.DelegationAssignmentId == message.Body.DelegationAssignmentId);

            var organisationForBody = GetOrganisationForBodyFromCache(message.Body.BodyId);

            personDocument.Mandates.Add(
                new PersonDocument.PersonMandate(
                    message.Body.BodyMandateId,
                    message.Body.DelegationAssignmentId,
                    message.Body.BodyId,
                    body.Name,
                    organisationForBody.OrganisationId,
                    organisationForBody.OrganisationName,
                    message.Body.BodySeatId,
                    bodySeat.Name,
                    bodySeat.Number,
                    bodySeat.IsPaid,
                    new Period(
                        message.Body.ValidFrom,
                        message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<PersonAssignedToDelegationRemoved> message)
        {
            var previousPersonDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PreviousPersonId).ThrowOnFailure().Source);

            previousPersonDocument.ChangeId = message.Number;
            previousPersonDocument.ChangeTime = message.Timestamp;

            if (previousPersonDocument.Mandates == null)
                previousPersonDocument.Mandates = new List<PersonDocument.PersonMandate>();

            RemoveMandate(previousPersonDocument, message.Body.BodyMandateId, message.Body.DelegationAssignmentId);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousPersonDocument).ThrowOnFailure());
        }

        private static void RemoveMandate(PersonDocument previousPersonDocument, Guid bodyMandateId, Guid delegationAssignmentId)
        {
            previousPersonDocument.Mandates.RemoveExistingListItems(x =>
                x.BodyMandateId == bodyMandateId &&
                x.DelegationAssignmentId == delegationAssignmentId);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyAssignedToOrganisation> message)
        {
            await using var organisationRegistryContext = _contextFactory.Create();
            var organisation = await organisationRegistryContext.OrganisationCache.SingleAsync(x => x.Id == message.Body.OrganisationId);

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "BodyId",
                    "bodyOrganisationId", message.Body.OrganisationId,
                    message.Number,
                    message.Timestamp));

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "BodyId",
                    "bodyOrganisationName", organisation.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyClearedFromOrganisation> message)
        {
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "bodyId",
                    "bodyOrganisationId", null,
                    message.Number,
                    message.Timestamp));

            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Mandates.Single().BodyId, message.Body.BodyId,
                    "mandates", "bodyId",
                    "bodyOrganisationName", null,
                    message.Number,
                    message.Timestamp));
        }

        private CachedOrganisationBody GetOrganisationForBodyFromCache(Guid bodyId)
        {
            using (var context = _contextFactory.Create())
            {
                var organisationPerBody =
                    context
                        .OrganisationPerBodyListForES
                        .SingleOrDefault(x => x.BodyId == bodyId);

                return organisationPerBody != null
                    ? CachedOrganisationBody.FromCache(organisationPerBody)
                    : CachedOrganisationBody.Empty();
            }
        }
    }
}
