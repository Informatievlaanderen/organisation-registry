namespace OrganisationRegistry.ElasticSearch.Projections.Organisations
{
    using System;
    using System.Data.Common;
    using System.Linq;
    using System.Threading.Tasks;
    using ElasticSearch.Organisations;
    using OrganisationRegistry.Organisation.Events;
    using OrganisationRegistry.Infrastructure.Events;
    using Location.Events;
    using Infrastructure;
    using Microsoft.Extensions.Logging;
    using Common;
    using Infrastructure.Change;

    public class OrganisationLocation :
        BaseProjection<OrganisationLocation>,
        IElasticEventHandler<OrganisationLocationAdded>,
        IElasticEventHandler<KboRegisteredOfficeLocationIsMainLocationChanged>,
        IElasticEventHandler<KboRegisteredOfficeOrganisationLocationAdded>,
        IElasticEventHandler<KboRegisteredOfficeOrganisationLocationRemoved>,
        IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
        IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
        IElasticEventHandler<OrganisationLocationUpdated>,
        IElasticEventHandler<LocationUpdated>,
        IElasticEventHandler<OrganisationTerminated>,
        IElasticEventHandler<OrganisationTerminatedV2>
    {
        public OrganisationLocation(
            ILogger<OrganisationLocation> logger) : base(logger)
        {
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction,
            IEnvelope<LocationUpdated> message)
        {
            return await new ElasticMassChange
            (
                elastic => elastic.TryAsync(() => elastic
                    .MassUpdateOrganisationAsync(
                        x => x.Locations.Single().LocationId, message.Body.LocationId,
                        "locations", "locationId",
                        "formattedAddress", message.Body.FormattedAddress,
                        message.Number,
                        message.Timestamp))
            ).ToAsyncResult();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
            => await AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId);

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationAdded> message)
            => await AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId);

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationRemoved> message)
            => await RemoveOrganisationLocation(message.Body.OrganisationId, message.Number, message.Timestamp, message.Body.OrganisationLocationId);

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationCouplingWithKboCancelled> message)
        {
            if (message.Body.RegisteredOfficeOrganisationLocationIdToCancel == null)
                return new ElasticNoChange();

            return await RemoveOrganisationLocation(message.Body.OrganisationId, message.Number, message.Timestamp, message.Body.RegisteredOfficeOrganisationLocationIdToCancel.Value);
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminationSyncedWithKbo> message)
        {
            if (message.Body.RegisteredOfficeOrganisationLocationIdToTerminate == null)
                return await new ElasticNoChange().ToAsyncResult();

            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    var registeredOfficeLocation = document.Locations.Single(label =>
                        label.OrganisationLocationId == message.Body.RegisteredOfficeOrganisationLocationIdToTerminate);

                    registeredOfficeLocation.Validity.End = message.Body.DateOfTermination;
                }
            ).ToAsyncResult();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeLocationIsMainLocationChanged> message)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    var registeredOfficeLocation = document.Locations.Single(label =>
                        label.OrganisationLocationId == message.Body.OrganisationLocationId);

                    registeredOfficeLocation.IsMainLocation = message.Body.IsMainLocation;
                }
            ).ToAsyncResult();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationUpdated> message)
            => await UpdateOrganisationLocation(
                message.Body.OrganisationId,
                message.Number,
                message.Timestamp,
                message.Body.OrganisationLocationId,
                message.Body.LocationId,
                message.Body.LocationFormattedAddress,
                message.Body.IsMainLocation,
                message.Body.LocationTypeId,
                message.Body.LocationTypeName,
                message.Body.ValidFrom,
                message.Body.ValidTo);

        private static async Task<IElasticChange> AddOrganisationLocation(Guid organisationId, Guid locationId, string locationFormattedAddress, bool isMainLocation, Guid? locationTypeId, string? locationTypeName, DateTime? validFrom, DateTime? validTo, int documentChangeId, DateTimeOffset timestamp, Guid organisationLocationId)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                organisationId,
                document =>
                {
                    document.ChangeId = documentChangeId;
                    document.ChangeTime = timestamp;

                    document.Locations.RemoveExistingListItems(x =>
                        x.OrganisationLocationId == organisationLocationId);

                    document.Locations.Add(
                        new OrganisationDocument.OrganisationLocation(
                            organisationLocationId,
                            locationId,
                            locationFormattedAddress,
                            isMainLocation,
                            locationTypeId,
                            locationTypeName,
                            Period.FromDates(validFrom, validTo)));
                }
            ).ToAsyncResult();
        }

        private static async Task<IElasticChange> UpdateOrganisationLocation(
            Guid bodyOrganisationId,
            int organisationDocumentChangeId,
            DateTimeOffset organisationDocumentChangeTime,
            Guid bodyOrganisationLocationId,
            Guid bodyLocationId,
            string bodyLocationFormattedAddress,
            bool bodyIsMainLocation,
            Guid? bodyLocationTypeId,
            string? bodyLocationTypeName,
            DateTime? bodyValidFrom,
            DateTime? bodyValidTo)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                bodyOrganisationId,
                document =>
                {
                    document.ChangeId = organisationDocumentChangeId;
                    document.ChangeTime = organisationDocumentChangeTime;

                    document.Locations.RemoveExistingListItems(x =>
                        x.OrganisationLocationId == bodyOrganisationLocationId);

                    document.Locations.Add(
                        new OrganisationDocument.OrganisationLocation(
                            bodyOrganisationLocationId,
                            bodyLocationId,
                            bodyLocationFormattedAddress,
                            bodyIsMainLocation,
                            bodyLocationTypeId,
                            bodyLocationTypeName,
                            Period.FromDates(bodyValidFrom, bodyValidTo)));

                }
            ).ToAsyncResult();
        }

        private static async Task<IElasticChange> RemoveOrganisationLocation(Guid bodyOrganisationId, int organisationDocumentChangeId, DateTimeOffset organisationDocumentChangeTime, Guid bodyOrganisationLocationId)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                bodyOrganisationId,
                document =>
                {
                    document.ChangeId = organisationDocumentChangeId;
                    document.ChangeTime = organisationDocumentChangeTime;

                    document.Locations.RemoveExistingListItems(x =>
                        x.OrganisationLocationId == bodyOrganisationLocationId);
                }
            ).ToAsyncResult();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    var locationsToTerminate =
                        message.Body.FieldsToTerminate.Locations;

                    if (message.Body.KboFieldsToTerminate.RegisteredOffice.HasValue)
                        locationsToTerminate.Add(message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Key, message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Value);

                    foreach (var (key, value) in locationsToTerminate)
                    {
                        var organisationLocation =
                            document
                                .Locations
                                .Single(x => x.OrganisationLocationId == key);

                        organisationLocation.Validity.End = value;
                    }
                }
            ).ToAsyncResult();
        }

        public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminatedV2> message)
        {
            return await new ElasticPerDocumentChange<OrganisationDocument>
            (
                message.Body.OrganisationId,
                document =>
                {
                    document.ChangeId = message.Number;
                    document.ChangeTime = message.Timestamp;

                    var locationsToTerminate =
                        message.Body.FieldsToTerminate.Locations;

                    if (message.Body.KboFieldsToTerminate.RegisteredOffice.HasValue)
                        locationsToTerminate.Add(message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Key, message.Body.KboFieldsToTerminate.RegisteredOffice.Value.Value);

                    foreach (var (key, value) in locationsToTerminate)
                    {
                        var organisationLocation =
                            document
                                .Locations
                                .Single(x => x.OrganisationLocationId == key);

                        organisationLocation.Validity.End = value;
                    }
                }
            ).ToAsyncResult();
        }
    }
}
