namespace OrganisationRegistry.ElasticSearch.Projections.Organisations;

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
using Location;

public class OrganisationLocation :
    BaseProjection<OrganisationLocation>,
    IElasticEventHandler<OrganisationLocationAdded>,
    IElasticEventHandler<KboRegisteredOfficeLocationIsMainLocationChanged>,
    IElasticEventHandler<KboRegisteredOfficeOrganisationLocationAdded>,
    IElasticEventHandler<KboRegisteredOfficeOrganisationLocationRemoved>,
    IElasticEventHandler<OrganisationCouplingWithKboCancelled>,
    IElasticEventHandler<OrganisationTerminationSyncedWithKbo>,
    IElasticEventHandler<OrganisationLocationUpdated>,
    IElasticEventHandler<OrganisationLocationRemoved>,
    IElasticEventHandler<LocationUpdated>,
    IElasticEventHandler<OrganisationTerminated>,
    IElasticEventHandler<OrganisationTerminatedV2>
{
    private readonly IEventStore _store;

    public OrganisationLocation(
        IEventStore store,
        ILogger<OrganisationLocation> logger) : base(logger)
    {
        _store = store;
    }

    public async Task<IElasticChange> Handle(
        DbConnection dbConnection,
        DbTransaction dbTransaction,
        IEnvelope<LocationUpdated> message)
    {
        return await new ElasticMassChange
        (
            elastic => elastic.TryAsync(
                () => elastic
                    .MassUpdateOrganisationLocationAsync(
                        x => x.Locations.Single().LocationId,
                        message.Body.LocationId,
                        message.Body,
                        message.Number,
                        message.Timestamp))
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationAdded> message)
        => await AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId, _store);

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeOrganisationLocationAdded> message)
        => await AddOrganisationLocation(message.Body.OrganisationId, message.Body.LocationId, message.Body.LocationFormattedAddress, message.Body.IsMainLocation, message.Body.LocationTypeId, message.Body.LocationTypeName, message.Body.ValidFrom, message.Body.ValidTo, message.Number, message.Timestamp, message.Body.OrganisationLocationId, _store);

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

        return await new ElasticPerDocumentChange<OrganisationDocument>(
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var registeredOfficeLocation = document.Locations.Single(
                    label =>
                        label.OrganisationLocationId == message.Body.RegisteredOfficeOrganisationLocationIdToTerminate);

                registeredOfficeLocation.Validity.End = message.Body.DateOfTermination;
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<KboRegisteredOfficeLocationIsMainLocationChanged> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                var registeredOfficeLocation = document.Locations.Single(
                    label =>
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
            message.Body.ValidTo,
            _store);

    private static async Task<IElasticChange> AddOrganisationLocation(Guid organisationId, Guid locationId, string locationFormattedAddress, bool isMainLocation, Guid? locationTypeId, string? locationTypeName, DateTime? validFrom, DateTime? validTo, int documentChangeId, DateTimeOffset timestamp, Guid organisationLocationId, IEventStore store)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            organisationId,
            document =>
            {
                document.ChangeId = documentChangeId;
                document.ChangeTime = timestamp;

                var eventEnvelopes = store.GetEventEnvelopes<Location>(locationId);

                var lastEvent = (IHasLocation)eventEnvelopes
                    .OrderBy(x => x.Number)
                    .Last()
                    .Body;

                document.Locations.RemoveExistingListItems(
                    x =>
                        x.OrganisationLocationId == organisationLocationId);

                document.Locations.Add(
                    new OrganisationDocument.OrganisationLocation(
                        organisationLocationId,
                        locationId,
                        locationFormattedAddress,
                        new OrganisationDocument.LocationComponents(
                            lastEvent.Street,
                            lastEvent.ZipCode,
                            lastEvent.City,
                            lastEvent.Country),
                        isMainLocation,
                        locationTypeId,
                        locationTypeName,
                        Period.FromDates(validFrom, validTo)));
            }
        ).ToAsyncResult();
    }

    private static async Task<IElasticChange> UpdateOrganisationLocation(
        Guid organisationId,
        int organisationDocumentChangeId,
        DateTimeOffset organisationDocumentChangeTime,
        Guid organisationLocationId,
        Guid locationId,
        string locationFormattedAddress,
        bool isMainLocation,
        Guid? locationTypeId,
        string? locationTypeName,
        DateTime? validFrom,
        DateTime? validTo,
        IEventStore store)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            organisationId,
            document =>
            {
                document.ChangeId = organisationDocumentChangeId;
                document.ChangeTime = organisationDocumentChangeTime;

                document.Locations.RemoveExistingListItems(
                    x =>
                        x.OrganisationLocationId == organisationLocationId);

                var eventEnvelopes = store.GetEventEnvelopes<Location>(locationId);

                var lastEvent = (IHasLocation)eventEnvelopes
                    .OrderBy(x => x.Number)
                    .Last()
                    .Body;

                document.Locations.Add(
                    new OrganisationDocument.OrganisationLocation(
                        organisationLocationId,
                        locationId,
                        locationFormattedAddress,
                        new OrganisationDocument.LocationComponents(
                            lastEvent.Street,
                            lastEvent.ZipCode,
                            lastEvent.City,
                            lastEvent.Country),
                        isMainLocation,
                        locationTypeId,
                        locationTypeName,
                        Period.FromDates(validFrom, validTo)));
            }
        ).ToAsyncResult();
    }

    private static async Task<IElasticChange> RemoveOrganisationLocation(Guid bodyOrganisationId, int organisationDocumentChangeId, DateTimeOffset organisationDocumentChangeTime, Guid bodyOrganisationLocationId)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            bodyOrganisationId,
            document =>
            {
                document.ChangeId = organisationDocumentChangeId;
                document.ChangeTime = organisationDocumentChangeTime;

                document.Locations.RemoveExistingListItems(
                    x =>
                        x.OrganisationLocationId == bodyOrganisationLocationId);
            }
        ).ToAsyncResult();
    }

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationTerminated> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
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
        return await new ElasticPerDocumentChange<OrganisationDocument>(
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

    public async Task<IElasticChange> Handle(DbConnection dbConnection, DbTransaction dbTransaction, IEnvelope<OrganisationLocationRemoved> message)
    {
        return await new ElasticPerDocumentChange<OrganisationDocument>(
            message.Body.OrganisationId,
            document =>
            {
                document.ChangeId = message.Number;
                document.ChangeTime = message.Timestamp;

                document.Locations.RemoveExistingListItems(
                    x =>
                        x.OrganisationLocationId == message.Body.OrganisationLocationId);
            }).ToAsyncResult();
    }
}
