namespace OrganisationRegistry.Infrastructure.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using Microsoft.Data.SqlClient;
    using System.Linq;
    using Dapper;
    using Events;
    using Newtonsoft.Json;
    using System.Security.Claims;
    using System.Threading.Tasks;
    using AppSpecific;
    using Authorization;
    using Configuration;
    using Infrastructure.Json;
    using Microsoft.Extensions.Options;

    // Scoped as SingleInstance()
    public class SqlServerEventStore : IEventStore
    {
        public const int NameLength = 200;
        public const int IpLength = 100;
        public const int LastNameLength = 200;
        public const int FirstNameLength = 200;
        public const int UserIdLength = 100;

        private readonly IEventPublisher _publisher;
        private readonly ISecurityService _securityService;
        private readonly IEventDataReader _eventDataReader;
        private readonly string _connectionString;
        private readonly string _administrationConnectionString;

        private readonly JsonSerializerSettings _jsonSerializerSettings;

        private static readonly string[] ExcludedEventTypes =
        {
            "OrganisationRegistry.Infrastructure.Events.RebuildProjection"
        };

        private DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private DbConnection GetAdministrationConnection()
        {
            return new SqlConnection(_administrationConnectionString);
        }

        public SqlServerEventStore(IOptions<InfrastructureConfiguration> infrastructureOptions, IEventPublisher publisher, ISecurityService securityService, IEventDataReader eventDataReader = null)
        {
            var options = infrastructureOptions.Value;

            _connectionString = options.EventStoreConnectionString;
            _administrationConnectionString = options.EventStoreAdministrationConnectionString;
            _publisher = publisher;
            _securityService = securityService;

            _jsonSerializerSettings = new JsonSerializerSettings().ConfigureForOrganisationRegistryEventStore();
            _eventDataReader = eventDataReader ?? new SqlServerEventReader(GetConnection);

            SqlMapper.Settings.CommandTimeout = options.EventStoreCommandTimeout;
        }

        public void InitaliseEventStore()
        {
            using (var db = GetAdministrationConnection())
            {
                db.Open();

                db.Execute($@"
IF NOT EXISTS (SELECT * FROM sys.schemas WHERE name = N'OrganisationRegistry')
    EXEC('CREATE SCHEMA [OrganisationRegistry] AUTHORIZATION [dbo]');

IF NOT EXISTS (SELECT * FROM sysobjects WHERE name='Events' AND xtype = 'U')
    CREATE TABLE [OrganisationRegistry].[Events] (
        [Number] [int] IDENTITY(1,1) NOT NULL,
	    [Id] [uniqueidentifier] NOT NULL,
	    [Version] [int] NOT NULL,
	    [Name] [varchar]({NameLength}) NOT NULL,
	    [Timestamp] [datetime] NOT NULL,
	    [Data] [nvarchar](max) NOT NULL,
        [Ip] [nvarchar]({IpLength}) NULL,
        [LastName] [nvarchar]({LastNameLength}) NULL,
        [FirstName] [nvarchar]({FirstNameLength}) NULL,
        [UserId] [nvarchar]({UserIdLength}) NULL
	    CONSTRAINT [PK_Events] PRIMARY KEY CLUSTERED ([Id] ASC, [Version] ASC)
    );

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Events_Name' AND object_id = OBJECT_ID('OrganisationRegistry.Events'))
    CREATE NONCLUSTERED INDEX [IX_Events_Name] ON [OrganisationRegistry].[Events] ([Name] ASC);

IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_Events_Number' AND object_id = OBJECT_ID('OrganisationRegistry.Events'))
    CREATE NONCLUSTERED INDEX [IX_Events_Number] ON [OrganisationRegistry].[Events] ([Number] ASC);
");
            }
        }

        public async Task Save<T>(IEnumerable<IEvent> events, IUser? user = null)
        {
            var eventsToSave = events.ToList();
            if (eventsToSave.Count == 0)
                return;

            user ??= _securityService.GetUser(ClaimsPrincipal.Current);
            var ip = user.Ip;
            var firstName = user.FirstName;
            var lastName = user.LastName;
            var userId = user.UserId;

            var envelopes =
                eventsToSave
                    .Select(@event => @event.ToEnvelope(ip, lastName, firstName, userId))
                    .ToList();

            // Events are stored atomically
            using (var db = GetConnection())
            {
                await db.OpenAsync();
                using (var tx = await db.BeginTransactionAsync(IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (var envelope in envelopes)
                        {
                            // Some type of events we dont want to store in the database, but we want to publish them however
                            if (ExcludedEventTypes.Contains(envelope.Body.GetType().FullName))
                                continue;

                            // These are the fields of EventData
                            var number = (await db.QueryAsync<int>(
@"INSERT INTO [OrganisationRegistry].[Events]
([Id], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId])
VALUES
(@Id, @Version, @Name, @Timestamp, @Data, @Ip, @LastName, @FirstName, @UserId);
SELECT CAST(SCOPE_IDENTITY() as int)",
                                new
                                {
                                    Id = envelope.Id.ToString("D"),
                                    Version = envelope.Version,
                                    Name = envelope.Body.GetType().FullName,
                                    Timestamp = envelope.Timestamp,
                                    Data = JsonConvert.SerializeObject(envelope.Body, _jsonSerializerSettings),
                                    Ip = envelope.Ip ?? string.Empty,
                                    LastName = envelope.LastName ?? string.Empty,
                                    FirstName = envelope.FirstName ?? string.Empty,
                                    UserId = envelope.UserId ?? string.Empty,
                                }, tx)).Single();

                            envelope.Number = number;
                        }

                        foreach (var envelope in envelopes)
                            await _publisher.Publish(db, tx, (dynamic)envelope);

                        await tx.CommitAsync();
                    }
                    catch
                    {
                        await tx.RollbackAsync();

                        var eventsInRollback = envelopes.Select(envelope => envelope.Body).ToList();
                        await _publisher.Publish(null, null, new ResetMemoryCache(eventsInRollback).ToTypedEnvelope(ip, lastName, firstName, userId));
                        await _publisher.Publish(null, null, new Rollback(eventsInRollback).ToTypedEnvelope(ip, lastName, firstName, userId));

                        throw;
                    }

                    foreach (var envelope in envelopes)
                        await _publisher.ProcessReactions((dynamic)envelope, user);
                }
            }
        }

        public IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion)
        {
            var events = _eventDataReader.GetEvents(aggregateId, fromVersion);

            return events
                .Select(e =>
                {
                    try
                    {
                        var eventType = e.Name.ToEventType();
                        return (IEvent)JsonConvert.DeserializeObject(e.Data, eventType, _jsonSerializerSettings);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidCastException($"Failed to cast '{e.Name}' with body: {e.Data}", ex);
                    }
                });
        }

        public int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null)
        {
            return _eventDataReader.GetEventCount(dateTimeOffset);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes)
        {
            var events = _eventDataReader.GetEvents(eventTypes);

            return ParseEventsIntoEnvelopes(events, _jsonSerializerSettings);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopes<T>(Guid aggregateId)
        {
            var events = _eventDataReader.GetEvents(aggregateId, FromVersion.Start);

            return ParseEventsIntoEnvelopes(events, _jsonSerializerSettings);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopesUntil<T>(Guid aggregateId, int untilEventNumber)
        {
            var events = _eventDataReader.GetEventsUntil(aggregateId, untilEventNumber);

            return ParseEventsIntoEnvelopes(events, _jsonSerializerSettings);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber)
        {
            var events = _eventDataReader.GetEventsAfter(eventNumber);

            return ParseEventsIntoEnvelopes(events, _jsonSerializerSettings);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventTypesToInclude)
        {
            var events = _eventDataReader.GetEventsAfter(eventNumber, maxEvents, eventTypesToInclude);

            return ParseEventsIntoEnvelopes(events, _jsonSerializerSettings);
        }


        public int GetLastEvent()
        {
            return _eventDataReader.GetLastEvent();
        }

        private static IEnumerable<IEnvelope> ParseEventsIntoEnvelopes(IEnumerable<EventData> events, JsonSerializerSettings settings)
        {
            return events
                .Select(e =>
                {
                    try
                    {
                        var eventType = e.Name.ToEventType();
                        var @event = (IEvent)JsonConvert.DeserializeObject(e.Data, eventType, settings);
                        return @event.ToEnvelope(e.Number, e.Ip, e.LastName, e.FirstName, e.UserId);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidCastException($"Failed to cast '{e.Name}' with body: {e.Data}", ex);
                    }
                });
        }
    }
}
