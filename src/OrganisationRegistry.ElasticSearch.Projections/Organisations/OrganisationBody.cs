namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Infrastructure.Events;
    using Client;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;
    using OrganisationRegistry.Body.Events;

    public class OrganisationBody :
        BaseProjection<OrganisationBody>,
        IEventHandler<BodyInfoChanged>,
        IEventHandler<BodyOrganisationAdded>,
        IEventHandler<BodyOrganisationUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationBody(
            ILogger<OrganisationBody> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyInfoChanged> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Bodies.Single().BodyId, message.Body.BodyId,
                    "bodies", "bodyId",
                    "bodyName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Bodies == null)
                organisationDocument.Bodies = new List<OrganisationDocument.OrganisationBody>();

            organisationDocument.Bodies.RemoveExistingListItems(x => x.BodyOrganisationId == message.Body.BodyOrganisationId);

            organisationDocument.Bodies.Add(
                new OrganisationDocument.OrganisationBody(
                    message.Body.BodyOrganisationId,
                    message.Body.BodyId,
                    message.Body.BodyName,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BodyOrganisationUpdated> message)
        {
            var previousOrganisationDocument =
                _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.PreviousOrganisationId).ThrowOnFailure().Source);

            previousOrganisationDocument.ChangeId = message.Number;
            previousOrganisationDocument.ChangeTime = message.Timestamp;

            var bodyName = previousOrganisationDocument.Bodies.First(x => x.BodyOrganisationId == message.Body.BodyOrganisationId).BodyName;
            previousOrganisationDocument.Bodies.RemoveExistingListItems(x => x.BodyOrganisationId == message.Body.BodyOrganisationId);

            if (message.Body.PreviousOrganisationId == message.Body.OrganisationId)
            {
                previousOrganisationDocument.Bodies.Add(
                    new OrganisationDocument.OrganisationBody(
                        message.Body.BodyOrganisationId,
                        message.Body.BodyId,
                        bodyName,
                        new Period(message.Body.ValidFrom, message.Body.ValidTo)));
            }
            else
            {
                var newOrganisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

                newOrganisationDocument.ChangeId = message.Number;
                newOrganisationDocument.ChangeTime = message.Timestamp;

                if (newOrganisationDocument.Bodies == null)
                    newOrganisationDocument.Bodies = new List<OrganisationDocument.OrganisationBody>();

                newOrganisationDocument.Bodies.Add(
                    new OrganisationDocument.OrganisationBody(
                        message.Body.BodyOrganisationId,
                        message.Body.BodyId,
                        bodyName,
                        new Period(message.Body.ValidFrom, message.Body.ValidTo)));

                _elastic.Try(() => _elastic.WriteClient.IndexDocument(newOrganisationDocument).ThrowOnFailure());
            }

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(previousOrganisationDocument).ThrowOnFailure());
        }
    }
}
