namespace OrganisationRegistry.Api.Backoffice.Admin.Events.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Runtime.Serialization;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Be.Vlaanderen.Basisregisters.Converters.Timestamp;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using Newtonsoft.Json;
    using OrganisationRegistry.Infrastructure.Events;
    using OrganisationRegistry.Infrastructure.EventStore;
    using SqlServer.Event;
    using SqlServer.Infrastructure;

    public class EventWithData
    {
        public Guid Id { get; }
        public int Number { get; }
        public int Version { get; }
        public string Name { get; }

        [JsonConverter(typeof(TimestampConverter))]
        public DateTime Timestamp { get; }

        public IEvent Data { get; }
        public string Ip { get; }
        public string LastName { get; }
        public string FirstName { get; }
        public string UserId { get; }

        public EventWithData(
            Guid id,
            int number,
            int version,
            string name,
            DateTime timestamp,
            string data,
            string ip,
            string lastName,
            string firstName,
            string userId)
        {
            var eventType = name.ToEventType();
            var eventData = (IEvent) JsonConvert.DeserializeObject(data, eventType);

            Id = id;
            Number = number;
            Version = version;
            Name = eventType.Name;
            Timestamp = timestamp;
            Data = eventData;
            Ip = ip;
            LastName = lastName;
            FirstName = firstName;
            UserId = userId;
        }

        public EventWithData(EventListItem x) : this(
            x.Id,
            x.Number,
            x.Version,
            x.Name,
            x.Timestamp,
            x.Data,
            x.Ip,
            x.LastName,
            x.FirstName,
            x.UserId)
        {
        }
    }

    public class EventListQuery: Query<EventListItem, EventListItemFilter, EventWithData>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new EventListSorting();

        protected override Expression<Func<EventListItem, EventWithData>> Transformation =>
            x => new EventWithData(
                x.Id,
                x.Number,
                x.Version,
                x.Name,
                x.Timestamp,
                x.Data,
                x.Ip,
                x.LastName,
                x.FirstName,
                x.UserId);

        public EventListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<EventListItem> Filter(FilteringHeader<EventListItemFilter> filtering)
        {
            var events = _context.Events.AsQueryable();

            if (!filtering.ShouldFilter)
                return events;

            if (filtering.Filter.EventNumber.HasValue && filtering.Filter.EventNumber > 0)
                events = events.Where(x => x.Number == filtering.Filter.EventNumber.Value);

            if (!filtering.Filter.Name.IsNullOrWhiteSpace())
                events = events.Where(x => x.Name.Contains(filtering.Filter.Name));

            if (!filtering.Filter.FirstName.IsNullOrWhiteSpace())
                events = events.Where(x => x.FirstName.Contains(filtering.Filter.FirstName));

            if (!filtering.Filter.LastName.IsNullOrWhiteSpace())
                events = events.Where(x => x.LastName.Contains(filtering.Filter.LastName));

            if (!filtering.Filter.Data.IsNullOrWhiteSpace())
                events = events.Where(x => x.Data.Contains(filtering.Filter.Data));

            if (!filtering.Filter.Ip.IsNullOrWhiteSpace())
                events = events.Where(x => x.Ip.Contains(filtering.Filter.Ip));

            return events;
        }

        private class EventListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(EventListItem.Number),
                nameof(EventListItem.Name),
                nameof(EventListItem.Timestamp),
                nameof(EventListItem.FirstName),
                nameof(EventListItem.LastName),
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(EventListItem.Number), SortOrder.Descending);
        }
    }

    public class EventListItemFilter
    {
        public int? EventNumber { get; set; }
        public string Name { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Data { get; set; }
        public string Ip { get; set; }
    }
}
