namespace OrganisationRegistry.Infrastructure.Events
{
    using System;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Messages;
    using Newtonsoft.Json;

    public interface IEvent<in T> : IEvent { }

    public interface IEvent : IMessage
    {
        /// <summary>
        /// The Version of the Aggregate which results from this event
        /// </summary>
        int Version { get; set; }

        /// <summary>
        /// The UTC time when this event occurred.
        /// </summary>
        [JsonConverter(typeof(TimestampConverter))]
        DateTimeOffset Timestamp { get; set; }
    }
}
