namespace OrganisationRegistry.ElasticSearch.Projections.People.Handlers
{
    using System;
    using System.Collections.Generic;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using Autofac.Features.OwnedInstances;
    using Capacity.Events;
    using Client;
    using Common;
    using ElasticSearch.People;
    using Function.Events;
    using Infrastructure;
    using Microsoft.EntityFrameworkCore;
    using Microsoft.Extensions.Logging;
    using Nest;
    using Organisation.Events;
    using SqlServer.ElasticSearchProjections;
    using SqlServer.Infrastructure;
    using OrganisationRegistry.Infrastructure.AppSpecific;
    using OrganisationRegistry.Infrastructure.Events;
    using SqlServer;

    public class PersonCapacity :
        Infrastructure.BaseProjection<PersonCapacity>,
        IEventHandler<OrganisationCapacityAdded>,
        IEventHandler<OrganisationCapacityUpdated>,
        IEventHandler<CapacityUpdated>,
        IEventHandler<FunctionUpdated>,
        IEventHandler<OrganisationCreated>,
        IEventHandler<OrganisationCreatedFromKbo>,
        IEventHandler<OrganisationInfoUpdated>,
        IEventHandler<OrganisationInfoUpdatedFromKbo>,
        IEventHandler<OrganisationCouplingWithKboCancelled>,
        IEventHandler<OrganisationCapacityBecameActive>,
        IEventHandler<OrganisationCapacityBecameInactive>,
        IEventHandler<OrganisationTerminated>
    {
        private readonly Elastic _elastic;
        private readonly IContextFactory _contextFactory;
        private readonly IMemoryCaches _memoryCaches;

        public PersonCapacity(
            ILogger<PersonCapacity> logger,
            Elastic elastic,
            IContextFactory contextFactory,
            IMemoryCaches memoryCaches) : base(logger)
        {
            _elastic = elastic;
            _contextFactory = contextFactory;
            _memoryCaches = memoryCaches;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<CapacityUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Capacities.Single().CapacityId, message.Body.CapacityId,
                    "capacities", "capacityId",
                    "capacityName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<FunctionUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Capacities.Single().FunctionId, message.Body.FunctionId,
                    "capacities", "functionId",
                    "functionName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreated> message)
        {
            AddCacheShowOnVlaamseOverheidSites(message.Body.OrganisationId, message.Body.ShowOnVlaamseOverheidSites);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCreatedFromKbo> message)
        {
            AddCacheShowOnVlaamseOverheidSites(message.Body.OrganisationId, false);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdated> message)
        {
            MassUpdateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);

            // If ShowOnVlaamseOverheid changed, update ShowOnVlaamseOverheid for all people needed
            if (message.Body.ShowOnVlaamseOverheidSites == message.Body.PreviouslyShownInVlaanderenBe)
                return;

            UpdateCacheShowOnVlaamseOverheidSites(message);

            _elastic.WriteClient.Indices.Refresh(Indices.Index<PersonDocument>());

            // TODO: Discuss: what if we get more than 10.000 people in a capacity for a certain organisation?
            // -> Then we implement scrolling API in this bit of code.
            var searchResponse = _elastic
                .TryGet(() => _elastic.WriteClient.Search<PersonDocument>(
                    search => search
                        .From(0)
                        .Size(Elastic.MaxResultWindow)
                        .Query(query => query.Nested(
                            nested => nested.Path(
                                path => path.Capacities).Query(
                                innerQuery => innerQuery.Bool(
                                    b => b.Must(
                                        must => must.Match(
                                            match => match
                                                .Field("capacities.organisationId")
                                                .Query(message.Body.OrganisationId.ToString())))))))))
                .ThrowOnFailure();

            foreach (var personDocument in searchResponse.Documents)
            {
                personDocument.ShowOnVlaamseOverheidSites = ShouldPersonBeShownOnVlaamseOverheidSites(personDocument);

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationInfoUpdatedFromKbo> message)
        {
            MassUpdateOrganisationName(message.Body.OrganisationId, message.Body.Name, message.Number, message.Timestamp);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            MassUpdateOrganisationName(message.Body.OrganisationId, message.Body.NameBeforeKboCoupling, message.Number, message.Timestamp);
        }

        private void MassUpdateOrganisationName(Guid organisationId, string name, int messageNumber, DateTimeOffset timestamp)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdatePerson(
                    x => x.Capacities.Single().OrganisationId, organisationId,
                    "capacities", "organisationId",
                    "organisationName", name,
                    messageNumber,
                    timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityAdded> message)
        {
            // Person is optional
            if (!message.Body.PersonId.HasValue)
                return;

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Capacities == null)
                personDocument.Capacities = new List<PersonDocument.PersonCapacity>();

            personDocument.Capacities.RemoveExistingListItems(x => x.PersonCapacityId == message.Body.OrganisationCapacityId);

            personDocument.Capacities.Add(
                new PersonDocument.PersonCapacity(
                    message.Body.OrganisationCapacityId,
                    message.Body.CapacityId,
                    message.Body.CapacityName,
                    message.Body.OrganisationId,
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            personDocument.ShowOnVlaamseOverheidSites = ShouldPersonBeShownOnVlaamseOverheidSites(personDocument);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityUpdated> message)
        {
            // If previous and current person are null, we dont care
            if (!message.Body.PreviousPersonId.HasValue && !message.Body.PersonId.HasValue)
                return;

            // If previous exists and current is different, we need to delete and add
            if (message.Body.PreviousPersonId.HasValue &&
                message.Body.PreviousPersonId != message.Body.PersonId)
            {
                var previousPersonDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PreviousPersonId).ThrowOnFailure().Source);
                previousPersonDocument.ChangeId = message.Number;
                previousPersonDocument.ChangeTime = message.Timestamp;

                if (previousPersonDocument.Capacities == null)
                    previousPersonDocument.Capacities = new List<PersonDocument.PersonCapacity>();

                previousPersonDocument.Capacities.RemoveExistingListItems(x => x.PersonCapacityId == message.Body.OrganisationCapacityId);

                previousPersonDocument.ShowOnVlaamseOverheidSites = ShouldPersonBeShownOnVlaamseOverheidSites(previousPersonDocument);

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousPersonDocument).ThrowOnFailure());
            }

            if (!message.Body.PersonId.HasValue)
                return;

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(message.Body.PersonId).ThrowOnFailure().Source);

            personDocument.ChangeId = message.Number;
            personDocument.ChangeTime = message.Timestamp;

            if (personDocument.Capacities == null)
                personDocument.Capacities = new List<PersonDocument.PersonCapacity>();

            personDocument.Capacities.RemoveExistingListItems(x => x.PersonCapacityId == message.Body.OrganisationCapacityId);

            personDocument.Capacities.Add(
                new PersonDocument.PersonCapacity(
                    message.Body.OrganisationCapacityId,
                    message.Body.CapacityId,
                    message.Body.CapacityName,
                    message.Body.OrganisationId,
                    _memoryCaches.OrganisationNames[message.Body.OrganisationId],
                    message.Body.FunctionId,
                    message.Body.FunctionName,
                    message.Body.Contacts.Select(x => new Contact(x.Key, _memoryCaches.ContactTypeNames[x.Key], x.Value)).ToList(),
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            personDocument.ShowOnVlaamseOverheidSites = ShouldPersonBeShownOnVlaamseOverheidSites(personDocument);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityBecameActive> message)
        {
            UpdateIsActivePerOrganisationCapacity(message.Body.OrganisationCapacityId, true);
            UpdateShouldBeShownOnVlaamseOverheidSites(message.Body.PersonId);
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCapacityBecameInactive> message)
        {
            UpdateIsActivePerOrganisationCapacity(message.Body.OrganisationCapacityId, false);
            UpdateShouldBeShownOnVlaamseOverheidSites(message.Body.PersonId);
        }

        private void UpdateShouldBeShownOnVlaamseOverheidSites(Guid? personId)
        {
            if (!personId.HasValue)
                return;

            var personDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<PersonDocument>(personId).ThrowOnFailure().Source);

            personDocument.ShowOnVlaamseOverheidSites = ShouldPersonBeShownOnVlaamseOverheidSites(personDocument);

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(personDocument).ThrowOnFailure());
        }

        private bool ShouldPersonBeShownOnVlaamseOverheidSites(PersonDocument personDocument)
        {
            using (var context = _contextFactory.Create())
            {
                var organisationIds =
                    personDocument.Capacities
                        .Select(capacity => capacity.OrganisationId)
                        .ToList();

                var organisationsInPersonCapacitiesShownOnVlaamseOverheidSites =
                    context.ShowOnVlaamseOverheidSitesPerOrganisationList
                        .AsNoTracking()
                        .Where(organisation => organisationIds.Contains(organisation.Id))
                        .Where(organisation => organisation.ShowOnVlaamseOverheidSites)
                        .Select(organisation => organisation.Id)
                        .ToList();

                var capacitiesWithOrganisationShownOnVlaamseOverheidSites =
                    personDocument.Capacities
                        .Where(capacity => organisationsInPersonCapacitiesShownOnVlaamseOverheidSites.Contains(capacity.OrganisationId))
                        .Select(capacity => capacity.PersonCapacityId)
                        .ToList();

                return context.IsActivePerOrganisationCapacityList
                    .AsNoTracking()
                    .Where(capacity => capacity.IsActive)
                    .Any(capacity => capacitiesWithOrganisationShownOnVlaamseOverheidSites.Contains(capacity.OrganisationCapacityId));
            }
        }

        private void AddCacheShowOnVlaamseOverheidSites(Guid organisationId, bool showOnVlaamseOverheidSites)
        {
            using (var context = _contextFactory.Create())
            {
                if (context.ShowOnVlaamseOverheidSitesPerOrganisationList.Any(x => x.Id == organisationId))
                    return;

                context
                    .ShowOnVlaamseOverheidSitesPerOrganisationList
                    .Add(new ShowOnVlaamseOverheidSitesPerOrganisation
                    {
                        Id = organisationId,
                        ShowOnVlaamseOverheidSites = showOnVlaamseOverheidSites
                    });

                context.SaveChanges();
            }
        }

        private void UpdateCacheShowOnVlaamseOverheidSites(IEnvelope<OrganisationInfoUpdated> message)
        {
            using (var context = _contextFactory.Create())
            {
                var showOnVlaamseOverheidSitesPerOrganisation =
                    context
                        .ShowOnVlaamseOverheidSitesPerOrganisationList
                        .Single(organisation => organisation.Id == message.Body.OrganisationId);

                showOnVlaamseOverheidSitesPerOrganisation.ShowOnVlaamseOverheidSites = message.Body.ShowOnVlaamseOverheidSites;

                context.SaveChanges();
            }
        }

        private void UpdateIsActivePerOrganisationCapacity(Guid organisationCapacityId, bool isActive)
        {
            using (var context = _contextFactory.Create())
            {
                var isActivePerOrganisationCapacity =
                    context
                        .IsActivePerOrganisationCapacityList
                        .SingleOrDefault(capacity => capacity.OrganisationCapacityId == organisationCapacityId);

                if (isActivePerOrganisationCapacity == null)
                {
                    context
                        .IsActivePerOrganisationCapacityList
                        .Add(new IsActivePerOrganisationCapacity
                        {
                            OrganisationCapacityId = organisationCapacityId,
                            IsActive = isActive
                        });
                }
                else
                {
                    isActivePerOrganisationCapacity.IsActive = isActive;
                }

                context.SaveChanges();
            }
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            foreach (var (key, value) in message.Body.CapacitiesToTerminate)
            {
                _elastic.Try(() => _elastic.WriteClient
                    .MassUpdatePerson(
                        queryFieldSelector: x => x.Capacities.Single().CapacityId, queryFieldValue: key,
                        listPropertyName: "capacities", idPropertyName: "capacityId",
                        namePropertyName: "validity.end", newName: value,
                        changeId: message.Number,
                        changeTime: message.Timestamp));
            }
        }
    }
}
