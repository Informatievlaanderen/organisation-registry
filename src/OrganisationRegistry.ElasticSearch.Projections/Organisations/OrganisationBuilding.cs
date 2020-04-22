namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Building.Events;
    using Client;
    using ElasticSearch.Organisations;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationBuilding :
        BaseProjection<OrganisationBuilding>,
        IEventHandler<OrganisationBuildingAdded>,
        IEventHandler<OrganisationBuildingUpdated>,
        IEventHandler<BuildingUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationBuilding(
            ILogger<OrganisationBuilding> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<BuildingUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Buildings.Single().BuildingId, message.Body.BuildingId,
                    "buildings", "buildingId",
                    "buildingName", message.Body.Name,
                    message.Number,
                    message.Timestamp));
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingAdded> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            if (organisationDocument.Buildings == null)
                organisationDocument.Buildings = new List<OrganisationDocument.OrganisationBuilding>();

            organisationDocument.Buildings.RemoveExistingListItems(x => x.OrganisationBuildingId == message.Body.OrganisationBuildingId);

            organisationDocument.Buildings.Add(
                new OrganisationDocument.OrganisationBuilding(
                    message.Body.OrganisationBuildingId,
                    message.Body.BuildingId,
                    message.Body.BuildingName,
                    message.Body.IsMainBuilding,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public async Task Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationBuildingUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Buildings.RemoveExistingListItems(x => x.OrganisationBuildingId == message.Body.OrganisationBuildingId);

            organisationDocument.Buildings.Add(
                new OrganisationDocument.OrganisationBuilding(
                    message.Body.OrganisationBuildingId,
                    message.Body.BuildingId,
                    message.Body.BuildingName,
                    message.Body.IsMainBuilding,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
