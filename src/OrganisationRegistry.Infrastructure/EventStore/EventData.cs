namespace OrganisationRegistry.Infrastructure.EventStore;

using System;
using Newtonsoft.Json;

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

#pragma warning disable CS8618
    public EventData()
#pragma warning restore CS8618
    {

    }

    [JsonConstructor]
    public EventData(Guid id, int number, int version, string name, DateTimeOffset timestamp, string data, string ip, string lastName, string firstName, string userId)
    {
        Id = id;
        Number = number;
        Version = version;
        Name = name;
        Timestamp = timestamp;
        Data = data;
        Ip = ip;
        LastName = lastName;
        FirstName = firstName;
        UserId = userId;
    }

    public EventData WithName(string name)
        => new EventData
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