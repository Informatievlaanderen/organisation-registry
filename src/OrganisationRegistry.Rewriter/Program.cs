namespace OrganisationRegistry.Rewriter
{
    using System;
    using System.Collections.Generic;
    using System.Data;
    using System.Data.Common;
    using System.Data.SqlClient;
    using System.Linq;
    using Dapper;
    using Infrastructure.EventStore;
    using Infrastructure.Infrastructure.Json;
    using Newtonsoft.Json;
    using Organisation.Events;

    public class Program
    {
        private static string _connectionString;

        private static DbConnection GetConnection()
        {
            return new SqlConnection(_connectionString);
        }

        // We halen OrganisationFunctionAdded en Updated op en steken ze per aggregate samen in juiste volgorde
        // Voor elke Updated, doen we een SQL UPDATE die de previous erbij zet
        public static void Main(string[] args)
        {
            JsonConvert.DefaultSettings =
                () => JsonSerializerSettingsProvider.CreateSerializerSettings().ConfigureForOrganisationRegistry();

            _connectionString = "Server=.;Database=OrganisationRegistry;Trusted_Connection=True;";

            using (var db = GetConnection())
            {
                db.Open();
                var tx = db.BeginTransaction();

                var events = GetEventEnvelopes(db, tx, typeof(OrganisationFunctionAdded), typeof(OrganisationFunctionUpdated));
                var eventsByOrganisationFunctions = events.GroupBy(x =>
                {
                    if (x.Name == typeof(OrganisationFunctionAdded).AssemblyQualifiedName)
                    {
                        var x2 = JsonConvert.DeserializeObject<OrganisationFunctionAdded>(x.Data);
                        return x2.OrganisationFunctionId;
                    }

                    if (x.Name == typeof(OrganisationFunctionUpdated).AssemblyQualifiedName)
                    {
                        var x2 = JsonConvert.DeserializeObject<OrganisationFunctionUpdated>(x.Data);
                        return x2.OrganisationFunctionId;
                    }

                    throw new Exception("Unk.");
                });

                var numberOfEditedEvents = 0;
                foreach (var eventsByOrganisationFunction in eventsByOrganisationFunctions)
                {
                    var organisationFunctionId = eventsByOrganisationFunction.Key;
                    Console.WriteLine($"[{organisationFunctionId}] Processing...");

                    if (eventsByOrganisationFunction.All(x => x.Name != typeof(OrganisationFunctionUpdated).AssemblyQualifiedName))
                    {
                        Console.WriteLine($"[{organisationFunctionId}] No update events found for {organisationFunctionId}.");
                        continue;
                    }

                    // Hier hebben we een aggregate id met minstents een updated functie event
                    EventData previousEvent = null;
                    foreach (var functionEvent in eventsByOrganisationFunction)
                    {
                        if (functionEvent.Name == typeof(OrganisationFunctionAdded).AssemblyQualifiedName)
                        {
                            previousEvent = functionEvent;
                        }
                        else if (functionEvent.Name == typeof(OrganisationFunctionUpdated).AssemblyQualifiedName)
                        {
                            var organisationFunctionUpdated =
                                JsonConvert.DeserializeObject<OrganisationFunctionUpdated>(functionEvent.Data);

                            // go and save updated json
                            Console.WriteLine($"[{organisationFunctionId}] About to update {functionEvent.Number}!");
                            var updatedEvent = BuildUpdatedEvent(previousEvent, organisationFunctionUpdated);
                            UpdateEvent(db, tx, functionEvent.Number, updatedEvent);
                            numberOfEditedEvents++;

                            previousEvent = functionEvent;
                        }
                    }
                }

                Console.WriteLine($"About to update {numberOfEditedEvents} events! Fingers crossed!");

                tx.Commit();

                Console.WriteLine($"Updated {numberOfEditedEvents} events! Hooray!");
            }
        }

        private static string BuildUpdatedEvent(EventData previousEvent, OrganisationFunctionUpdated organisationFunctionUpdated)
        {
            if (previousEvent.Name == typeof(OrganisationFunctionAdded).AssemblyQualifiedName)
            {
                var previous = JsonConvert.DeserializeObject<OrganisationFunctionAdded>(previousEvent.Data);
                var updatedEvent = new OrganisationFunctionUpdated(
                    organisationFunctionUpdated.OrganisationId,
                    organisationFunctionUpdated.OrganisationFunctionId,
                    organisationFunctionUpdated.FunctionId,
                    organisationFunctionUpdated.FunctionName,
                    organisationFunctionUpdated.PersonId,
                    organisationFunctionUpdated.PersonFullName,
                    organisationFunctionUpdated.Contacts,
                    organisationFunctionUpdated.ValidFrom,
                    organisationFunctionUpdated.ValidTo,
                    previous.FunctionId,
                    previous.FunctionName,
                    previous.PersonId,
                    previous.PersonFullName,
                    previous.Contacts,
                    previous.ValidFrom,
                    previous.ValidTo);

                updatedEvent.Version = organisationFunctionUpdated.Version;
                updatedEvent.Timestamp = organisationFunctionUpdated.Timestamp;

                return JsonConvert.SerializeObject(updatedEvent);
            }

            if (previousEvent.Name == typeof(OrganisationFunctionUpdated).AssemblyQualifiedName)
            {
                var previous = JsonConvert.DeserializeObject<OrganisationFunctionUpdated>(previousEvent.Data);
                var updatedEvent = new OrganisationFunctionUpdated(
                    organisationFunctionUpdated.OrganisationId,
                    organisationFunctionUpdated.OrganisationFunctionId,
                    organisationFunctionUpdated.FunctionId,
                    organisationFunctionUpdated.FunctionName,
                    organisationFunctionUpdated.PersonId,
                    organisationFunctionUpdated.PersonFullName,
                    organisationFunctionUpdated.Contacts,
                    organisationFunctionUpdated.ValidFrom,
                    organisationFunctionUpdated.ValidTo,
                    previous.FunctionId,
                    previous.FunctionName,
                    previous.PersonId,
                    previous.PersonFullName,
                    previous.Contacts,
                    previous.ValidFrom,
                    previous.ValidTo);

                updatedEvent.Version = organisationFunctionUpdated.Version;
                updatedEvent.Timestamp = organisationFunctionUpdated.Timestamp;

                return JsonConvert.SerializeObject(updatedEvent);
            }

            throw new Exception("Huh.");
        }

        private static IEnumerable<EventData> GetEventEnvelopes(IDbConnection db, IDbTransaction tx, params Type[] eventTypes)
        {
            return db.Query<EventData>(
                @"SELECT [Id], [Number], [Version], [Name], [Timestamp], [Data], [Ip], [LastName], [FirstName], [UserId]
FROM [OrganisationRegistry].[Events]
WHERE [Name] IN @EventTypes
ORDER BY [Number] ASC",
                new
                {
                    EventTypes = eventTypes.Select(x => x.AssemblyQualifiedName).ToList()
                },
                tx).ToList();
        }

        private static void UpdateEvent(IDbConnection db, IDbTransaction tx, int number, string newJson)
        {
            var bla = db.Execute(
                @"UPDATE [OrganisationRegistry].[Events] SET [Data] = @Data WHERE [Number] = @Number",
                new
                {
                    Data = newJson,
                    Number = number
                },
                tx);

            if (bla != 1)
                throw new Exception("1 is not 1!");
        }
    }
}
