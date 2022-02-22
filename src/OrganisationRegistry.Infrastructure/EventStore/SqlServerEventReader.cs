namespace OrganisationRegistry.Infrastructure.EventStore
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Immutable;
    using System.Data;
    using System.Data.Common;
    using System.Linq;
    using Dapper;

    public interface IEventDataReader
    {
        List<EventData> GetEvents(Guid aggregateId, int fromVersion);
        int GetEventCount(DateTimeOffset? dateTimeOffset = null);
        List<EventData> GetEvents(params Type[] eventTypes);
        List<EventData> GetEventsUntil(Guid aggregateId, int untilEventNumber);
        List<EventData> GetEventsAfter(int eventNumber);
        List<EventData> GetEventsAfter(int eventNumber, int maxEvents, params Type[] eventTypesToInclude);
        int GetLastEvent();
    }

    public class SqlServerEventReader : IEventDataReader
    {
        private readonly Func<DbConnection> _getConnection;

        public SqlServerEventReader(Func<DbConnection> getConnection)
        {
            _getConnection = getConnection;
        }

        public List<EventData> GetEvents(Guid aggregateId, int fromVersion)
        {
            List<EventData> events;

            using (var db = _getConnection())
            {
                db.Open();

                events = db.Query<EventData>(
                    @$"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Id] = @Id
AND [Version] > @Version
AND NOT {"[Name]".IsIgnoredEvent()}
ORDER BY Version ASC",
                    new
                    {
                        Id = aggregateId,
                        Version = fromVersion
                    }).ToList();
            }

            return events;
        }

        // TODO: check if unused ?
        public int GetEventCount(DateTimeOffset? dateTimeOffset = null)
        {
            using (var db = _getConnection())
            {
                db.Open();

                return db.ExecuteScalar<int>(
                    @$"SELECT count(*)
FROM [OrganisationRegistry].[Events]
AND NOT {"[Name]".IsIgnoredEvent()}
WHERE [Timestamp] > @DateTime",
                    new
                    {
                        DateTime = dateTimeOffset ?? DateTimeOffset.MinValue
                    });
            }
        }

        public List<EventData> GetEvents(params Type[] eventTypes)
        {
            using (var db = _getConnection())
            {
                db.Open();

                return db.Query<EventData>(
                    @$"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Name] IN @EventTypes
AND NOT {"[Name]".IsIgnoredEvent()}
ORDER BY [Number] ASC",
                    new
                    {
                        EventTypes = eventTypes.GetEventTypeNames()
                    }).ToList();
            }
        }

        public List<EventData> GetEventsUntil(Guid aggregateId, int untilEventNumber)
        {
            List<EventData> events;

            using (var db = _getConnection())
            {
                db.Open();

                events = db.Query<EventData>(
                    @$"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Id] = @Id
AND [Number] <= @Number
AND NOT {"[Name]".IsIgnoredEvent()}
ORDER BY Version ASC",
                    new
                    {
                        Id = aggregateId,
                        Number = untilEventNumber
                    }).ToList();
            }

            return events;
        }

        public List<EventData> GetEventsAfter(int eventNumber)
        {
            using (var db = _getConnection())
            {
                db.Open();

                return SelectEvents(eventNumber, db);
            }
        }

        public List<EventData> GetEventsAfter(int eventNumber, int maxEvents, params Type[] eventTypesToInclude)
        {
            using (var db = _getConnection())
            {
                db.Open();

                return eventTypesToInclude.Any()
                    ? SelectMaxEvents(eventNumber, maxEvents, db, eventTypesToInclude)
                    : SelectMaxEvents(eventNumber, maxEvents, db);
            }
        }

        public int GetLastEvent()
        {
            using (var db = _getConnection())
            {
                db.Open();

                return db.ExecuteScalar<int>(@"SELECT Max(Number) FROM [OrganisationRegistry].[Events]");
            }
        }

        private static List<EventData> SelectEvents(int eventNumber, IDbConnection db)
        {
            return db.Query<EventData>(
                @$"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
AND NOT {"[Name]".IsIgnoredEvent()}
ORDER BY [Number] ASC",
                new
                {
                    Number = eventNumber
                }).ToList();
        }

        private static List<EventData> SelectMaxEvents(int eventNumber, int maxEvents, IDbConnection db)
        {
            return db.Query<EventData>(
                @$"SELECT TOP(@MaxEvents) [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
AND NOT {"[Name]".IsIgnoredEvent()}
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
                @$"SELECT TOP(@MaxEvents) [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Number] > @Number
AND [Name] IN @EventTypesToInclude
AND NOT {"[Name]".IsIgnoredEvent()}
ORDER BY [Number] ASC",
                new
                {
                    MaxEvents = maxEvents,
                    Number = eventNumber,
                    EventTypesToInclude = eventTypesToInclude.GetEventTypeNames()
                }).ToList();
        }
    }

    public static class SqlExtensions
    {
        private static readonly ImmutableList<string> IgnoredEvents =
            ImmutableList.Create<string>("OrganisationRegistry.Day.Events.DayHasPassed");

        public static string IsIgnoredEvent(this string field) =>
            !IgnoredEvents.Any()
                ? " 1=0 " // empty ignoredEvents list implies not ignored -> false
                : $" {field} IN ({ToCommaSeparatedString(IgnoredEvents)}) ";

        private static string ToCommaSeparatedString(IEnumerable<string> list) =>
            list.Aggregate("", (aggregated, item) => aggregated.Length > 0 ? $"{aggregated}, '{item}'" : $"'{item}'");
    }
}
