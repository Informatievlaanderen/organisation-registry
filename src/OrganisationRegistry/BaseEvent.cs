namespace OrganisationRegistry
{
    using System;
    using Infrastructure.Events;
    using Infrastructure.Messages;

    public class BaseEvent<T> : IEvent<T>
    {
        protected Guid Id { get; set; }

        public int Version { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        Guid IMessage.Id
        {
            get => Id;
            set => Id = value;
        }
    }
}
