namespace OrganisationRegistry.Infrastructure.Events
{
    using System;
    using Messages;

    public interface IEnvelope : IMessage
    {
        int Number { get; set; }
        int Version { get; }
        string Name { get; }
        DateTimeOffset Timestamp { get; }
        string Ip { get; }
        string LastName { get; }
        string FirstName { get; }
        string UserId { get; }
        IEvent Body { get; }
    }

    public abstract class Envelope : IEnvelope
    {
        public Guid Id { get; set; }

        public int Number { get; set; }
        public int Version { get; }
        public string Name { get; }
        public DateTimeOffset Timestamp { get; }
        public string Ip { get; }
        public string LastName { get; }
        public string FirstName { get; }
        public string UserId { get; }
        public IEvent Body { get; }

        protected Envelope(
            Guid id,
            int version,
            string name,
            DateTimeOffset timestamp,
            IEvent body,
            string ip,
            string lastName,
            string firstName,
            string userId)
        {
            Id = id;
            Version = version;
            Name = name;
            Timestamp = timestamp;
            Ip = ip;
            LastName = lastName;
            FirstName = firstName;
            UserId = userId;
            Body = body;
        }
    }

    public interface IEnvelope<out T> : IEnvelope where T : IEvent<T>
    {
        new T Body { get; }
    }

    public class Envelope<T> : Envelope, IEnvelope<T> where T : IEvent<T>
    {
        public new T Body { get; }

        public Envelope(Guid id,
            int version,
            string name,
            DateTimeOffset timestamp,
            T body,
            string ip,
            string lastName,
            string firstName,
            string userId)
            : base(id, version, name, timestamp, body, ip, lastName, firstName, userId)
            => Body = body;
    }

    public static class EnvelopeExtensions
    {
        public static IEnvelope ToEnvelope<T>(this T source)
            where T : IEvent
            => ToEnvelope(source, default, string.Empty, string.Empty, string.Empty, string.Empty);

        public static IEnvelope ToEnvelope<T>(
            this T source,
            string ip,
            string lastName,
            string firstName,
            string userId)
            where T : IEvent
            => ToEnvelope(source, default, ip, lastName, firstName, userId);

        public static IEnvelope ToEnvelope<T>(
            this T source,
            int number,
            string ip,
            string lastName,
            string firstName,
            string userId)
            where T : IEvent
        {
            var type = typeof(Envelope<>).MakeGenericType(source.GetType());
            var envelope = (IEnvelope)Activator.CreateInstance(
                type,
                source.Id,
                source.Version,
                source.GetType().AssemblyQualifiedName,
                source.Timestamp,
                source,
                ip,
                lastName,
                firstName,
                userId);

            envelope.Number = number;
            return envelope;
        }

        public static IEnvelope<T> ToTypedEnvelope<T>(this T source)
            where T : IEvent<T>
            => ToTypedEnvelope(source, default, string.Empty, string.Empty, string.Empty, string.Empty);

        public static IEnvelope<T> ToTypedEnvelope<T>(
            this T source,
            string ip,
            string lastName,
            string firstName,
            string userId)
            where T : IEvent<T>
            => ToTypedEnvelope(source, default, ip, lastName, firstName, userId);

        public static IEnvelope<T> ToTypedEnvelope<T>(
            this T source,
            int number,
            string ip,
            string lastName,
            string firstName,
            string userId)
            where T : IEvent<T>
            => new Envelope<T>(
                source.Id,
                source.Version,
                source.GetType().AssemblyQualifiedName,
                source.Timestamp,
                source,
                ip,
                lastName,
                firstName,
                userId)
            {
                Number = number
            };
    }
}
