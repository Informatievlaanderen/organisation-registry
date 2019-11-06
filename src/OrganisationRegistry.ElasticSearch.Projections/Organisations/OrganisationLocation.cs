namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Collections.Generic;
    using System.Linq;
    using Client;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Location.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;

    public class OrganisationLocation :
        BaseProjection<OrganisationLocation>,
        IEventHandler<OrganisationLocationAdded>,
        IEventHandler<KboRegisteredOfficeOrganisationLocationAdded>,
        IEventHandler<OrganisationLocationUpdated>,
        IEventHandler<LocationUpdated>
    {
        private readonly Elastic _elastic;

        public OrganisationLocation(
            ILogger<OrganisationLocation> logger,
            Elastic elastic) : base(logger)
        {
            _elastic = elastic;
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<LocationUpdated> message)
        {
            // Update all which use this type, and put the changeId on them too!
            _elastic.Try(() => _elastic.WriteClient
                .MassUpdateOrganisation(
                    x => x.Locations.Single().LocationId, message.Body.LocationId,
                    "locations", "locationId",
                    "formattedAddress", message.Body.FormattedAddress,
                    message.Number,
                    message.Timestamp));
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
        {
            AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId);
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationAdded> message)
        {
            AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId);
        }

        private void AddOrganisationLocation(Guid organisationId, Guid locationId, string locationFormattedAddress, bool isMainLocation, Guid? locationTypeId, string locationTypeName, DateTime? validFrom, DateTime? validTo, int documentChangeId, DateTimeOffset timestamp, Guid organisationLocationId)
        {
            var organisationDocument = _elastic.TryGet(() =>
                _elastic.WriteClient.Get<OrganisationDocument>(organisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = documentChangeId;
            organisationDocument.ChangeTime = timestamp;

            if (organisationDocument.Locations == null)
                organisationDocument.Locations = new List<OrganisationDocument.OrganisationLocation>();

            organisationDocument.Locations.RemoveExistingListItems(x =>
                x.OrganisationLocationId == organisationLocationId);

            organisationDocument.Locations.Add(
                new OrganisationDocument.OrganisationLocation(
                    organisationLocationId,
                    locationId,
                    locationFormattedAddress,
                    isMainLocation,
                    locationTypeId,
                    locationTypeName,
                    new Period(validFrom, validTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }

        public void Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationUpdated> message)
        {
            var organisationDocument = _elastic.TryGet(() => _elastic.WriteClient.Get<OrganisationDocument>(message.Body.OrganisationId).ThrowOnFailure().Source);

            organisationDocument.ChangeId = message.Number;
            organisationDocument.ChangeTime = message.Timestamp;

            organisationDocument.Locations.RemoveExistingListItems(x => x.OrganisationLocationId == message.Body.OrganisationLocationId);

            organisationDocument.Locations.Add(
                new OrganisationDocument.OrganisationLocation(
                    message.Body.OrganisationLocationId,
                    message.Body.LocationId,
                    message.Body.LocationFormattedAddress,
                    message.Body.IsMainLocation,
                    message.Body.LocationTypeId,
                    message.Body.LocationTypeName,
                    new Period(message.Body.ValidFrom, message.Body.ValidTo)));

            _elastic.Try(() => _elastic.WriteClient.IndexDocument(organisationDocument).ThrowOnFailure());
        }
    }
}
