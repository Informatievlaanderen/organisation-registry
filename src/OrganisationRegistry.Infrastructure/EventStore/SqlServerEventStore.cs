namespace OrganisationRegistry.Infrastructure.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;
    using Events;
    using Newtonsoft.Json;
    using System.Security.Claims;
    using AppSpecific;
    using Configuration;
    using Messages;
    using Microsoft.Extensions.Options;

    public class EventData
    {
        public Guid Id { get; private set; }
        public int Number { get; private set; }
        public int Version { get; private set; }
        public string Name { get; private set; }
        public DateTimeOffset Timestamp { get; private set; }
        public string Data { get; private set; }
        public string Ip { get; private set; }
        public string LastName { get; private set; }
        public string FirstName { get; private set; }
        public string UserId { get; private set; }

        public EventData WithName(string name)
        {
            return new EventData
            {
                Id = Id,
                Number = Number,
                Version = Version,
                Name = name,
                Timestamp = Timestamp,
                Data = Data,
                Ip = Ip,
                LastName = LastName,
                FirstName = FirstName,
                UserId = UserId
            };
        }
    }

    public class Rollback : IEvent<Rollback>
    {
        protected Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        Guid IMessage.Id
        {
            get => Id;
            set => Id = value;
        }

        public List<IEvent> Events { get; }

        public Rollback(IEnumerable<IEvent> events)
        {
            Events = events.ToList();
        }
    }

    // Scoped as SingleInstance()
    public class SqlServerEventStore : IEventStore
    {
        public const int NameLength = 200;
        public const int IpLength = 100;
        public const int LastNameLength = 200;
        public const int FirstNameLength = 200;
        public const int UserIdLength = 100;

        private readonly IEventPublisher _publisher;
        private readonly string _connectionString;
        private readonly string _administrationConnectionString;

        private static readonly string[] ExcludedEventTypes =
        {
            "OrganisationRegistry.Infrastructure.Events.RebuildProjection, OrganisationRegistry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"
        };

        private DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        private DbConnection GetAdministrationConnection()
        {
            return new SqlConnection(_administrationConnectionString);
        }

        public SqlServerEventStore(IOptions<InfrastructureConfiguration> infrastructureOptions, IEventPublisher publisher)
        {
            _connectionString = infrastructureOptions.Value.EventStoreConnectionString;
            _administrationConnectionString = infrastructureOptions.Value.EventStoreAdministrationConnectionString;
            _publisher = publisher;
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

        public void Save<T>(IEnumerable<IEvent> events)
        {
            var eventsToSave = events.ToList();
            if (eventsToSave.Count == 0)
                return;

            var principal = ClaimsPrincipal.Current;
            var ip = principal?.FindFirst("urn:be:vlaanderen:wegwijs:ip")?.Value;
            var firstName = principal?.FindFirst(ClaimTypes.GivenName)?.Value;
            var lastName = principal?.FindFirst(ClaimTypes.Name)?.Value;
            var userId = principal?.FindFirst("urn:be:vlaanderen:wegwijs:acmid")?.Value;

            var envelopes =
                eventsToSave
                    .Select(@event => @event.ToEnvelope(ip, lastName, firstName, userId))
                    .ToList();

            // Events are stored atomically
            using (var db = GetConnection())
            {
                db.Open();
                using (var tx = db.BeginTransaction(IsolationLevel.Serializable))
                {
                    try
                    {
                        foreach (var envelope in envelopes)
                        {
                            // Some type of events we dont want to store in the database, but we want to publish them however
                            if (ExcludedEventTypes.Contains(envelope.Body.GetType().AssemblyQualifiedName))
                                continue;

                            // These are the fields of EventData
                            var number = db.Query<int>(
@"INSERT INTO [OrganisationRegistry].[Events]
([Id], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId])
VALUES
(@Id, @Version, @Name, @Timestamp, @Data, @Ip, @LastName, @FirstName, @UserId);
SELECT CAST(SCOPE_IDENTITY() as int)",
                                new
                                {
                                    Id = envelope.Id.ToString("D"),
                                    Version = envelope.Version,
                                    Name = envelope.Body.GetType().AssemblyQualifiedName,
                                    Timestamp = envelope.Timestamp,
                                    Data = JsonConvert.SerializeObject(envelope.Body),
                                    Ip = envelope.Ip ?? string.Empty,
                                    LastName = envelope.LastName ?? string.Empty,
                                    FirstName = envelope.FirstName ?? string.Empty,
                                    UserId = envelope.UserId ?? string.Empty,
                                }, tx).Single();

                            envelope.Number = number;
                        }

                        foreach (var envelope in envelopes)
                            _publisher.Publish(db, tx, (dynamic)envelope);

                        tx.Commit();
                    }
                    catch
                    {
                        tx.Rollback();

                        var eventsInRollback = envelopes.Select(envelope => envelope.Body).ToList();
                        _publisher.Publish(null, null, new ResetMemoryCache(eventsInRollback).ToTypedEnvelope(ip, lastName, firstName, userId));
                        _publisher.Publish(null, null, new Rollback(eventsInRollback).ToTypedEnvelope(ip, lastName, firstName, userId));

                        throw;
                    }

                    foreach (var envelope in envelopes)
                        _publisher.ProcessReactions((dynamic)envelope);
                }
            }
        }

        public IEnumerable<IEvent> Get<T>(Guid aggregateId, int fromVersion)
        {
            List<EventData> events;

            using (var db = GetConnection())
            {
                db.Open();

                events = db.Query<EventData>(
@"SELECT [Id], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Id] = @Id
AND [Version] > @Version
ORDER BY Version ASC",
                    new
                    {
                        Id = aggregateId,
                        Version = fromVersion
                    }).ToList();
            }

            return events
                .Select(e =>
                {
                    try
                    {
                        var eventType = Type.GetType(e.Name);
                        return (IEvent)JsonConvert.DeserializeObject(e.Data, eventType);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidCastException($"Failed to cast '{e.Name}' with body: {e.Data}", ex);
                    }
                });
        }

        public int GetEventEnvelopeCount(DateTimeOffset? dateTimeOffset = null)
        {
            using (var db = GetConnection())
            {
                db.Open();

                return db.ExecuteScalar<int>(
                    @"SELECT count(*)
FROM [OrganisationRegistry].[Events]
WHERE [Timestamp] > @DateTime",
                    new
                    {
                        DateTime = dateTimeOffset ?? DateTimeOffset.MinValue
                    });
            }
        }

        public IEnumerable<IEnvelope> GetEventEnvelopes(params Type[] eventTypes)
        {
            List<EventData> events;

            using (var db = GetConnection())
            {
                db.Open();

                events = db.Query<EventData>(
@"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Name] IN @EventTypes
ORDER BY [Number] ASC",
                    new
                    {
                        EventTypes = eventTypes.Select(x => x.AssemblyQualifiedName).ToList()
                    }).ToList();
            }

            return ParseEventsIntoEnvelopes(events);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber)
        {
            List<EventData> events;

            using (var db = GetConnection())
            {
                db.Open();

                events = SelectEvents(eventNumber, db);
            }

            return ParseEventsIntoEnvelopes(events);
        }

        public IEnumerable<IEnvelope> GetEventEnvelopesAfter(int eventNumber, int maxEvents, params Type[] eventTypesToInclude)
        {
            List<EventData> events;

            using (var db = GetConnection())
            {
                db.Open();

                events = eventTypesToInclude.Any()
                    ? SelectMaxEvents(eventNumber, maxEvents, db, eventTypesToInclude)
                    : SelectMaxEvents(eventNumber, maxEvents, db);
            }

            return ParseEventsIntoEnvelopes(events);
        }

        public int GetLastEvent()
        {
            using (var db = GetConnection())
            {
                db.Open();

                return db.ExecuteScalar<int>(@"SELECT Max(Number) FROM [OrganisationRegistry].[Events]");
            }
        }

        private static IEnumerable<IEnvelope> ParseEventsIntoEnvelopes(IEnumerable<EventData> events)
        {
            return events
                .Select(e =>
                {
                    try
                    {
                        var eventType = Type.GetType(e.Name);
                        var @event = (IEvent)JsonConvert.DeserializeObject(e.Data, eventType);
                        return @event.ToEnvelope(e.Number, e.Ip, e.LastName, e.FirstName, e.UserId);
                    }
                    catch (InvalidCastException ex)
                    {
                        throw new InvalidCastException($"Failed to cast '{e.Name}' with body: {e.Data}", ex);
                    }
                });
        }

        private static List<EventData> SelectEvents(int eventNumber, IDbConnection db)
        {
            return db.Query<EventData>(
                @"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
ORDER BY [Number] ASC",
                new
                {
                    Number = eventNumber
                }).ToList();
        }

        private static List<EventData> SelectMaxEvents(int eventNumber, int maxEvents, IDbConnection db)
        {
            return db.Query<EventData>(
                @"SELECT TOP(@MaxEvents) [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
ORDER BY [Number] ASC",
                new
                {
                    MaxEvents = maxEvents,
                    Number = eventNumber
                }).ToList();
        }

        private static List<EventData> SelectMaxEvents(int eventNumber, int maxEvents, IDbConnection db, IEnumerable<Type> eventTypesToInclude)
        {
            return db.Query<EventData>(
@"SELECT TOP(@MaxEvents) [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
AND [Name] IN @EventTypesToInclude
ORDER BY [Number] ASC",
                new
                {
                    MaxEvents = maxEvents,
                    Number = eventNumber,
                    EventTypesToInclude = eventTypesToInclude.Select(x => x.AssemblyQualifiedName).ToList()
                }).ToList();
        }
    }

    // Example of an event upgrader
    //internal class RefactorMoveNamespace20161019
    //{
    //    public DateTime DefinedOn => new DateTime(2016, 10, 19);

    //    public IEnumerable<SqlServerEventStore.EventData> Upgrade(SqlServerEventStore.EventData e, Guid id)
    //    {
    //        if (e.Name.StartsWith("OrganisationRegistry.ReadModel."))
    //        {
    //            // old: OrganisationRegistry.ReadModel.Location.LocationCreated, OrganisationRegistry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null
    //            // new: OrganisationRegistry.Location.Events.LocationCreated, OrganisationRegistry, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null

    //            var className = e.Name.Substring(0, e.Name.IndexOf(',', 0));
    //            var parts = className.Split('.');
    //            var newName =
    //                string.Concat(
    //                    parts[0], ".", // OrganisationRegistry
    //                    parts[2], ".", // Location
    //                    "Events", ".", // Events
    //                    parts[3]); // LocationCreated

    //            yield return e.WithName(e.Name.Replace(className, newName));
    //            yield break;
    //        }

    //        yield return e;
    //    }
    //}
}
