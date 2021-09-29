namespace OrganisationRegistry.Api.Backoffice.Body.Queries
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Body;
    using SqlServer.Infrastructure;

    public class BodySeatListQueryResult
    {
        public Guid BodySeatId { get; }
        public string BodySeatNumber { get; }
        public string Name { get; }
        public Guid SeatTypeId { get; }
        public string SeatTypeName{ get; }
        public bool PaidSeat { get; }
        public bool EntitledToVote { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public BodySeatListQueryResult(
            Guid bodySeatId,
            string bodySeatNumber,
            string name,
            Guid seatTypeId,
            string seatTypeName,
            bool paidSeat,
            bool entitledToVote,
            DateTime? validFrom,
            DateTime? validTo)
        {
            BodySeatId = bodySeatId;
            BodySeatNumber = bodySeatNumber;
            Name = name;
            SeatTypeId = seatTypeId;
            SeatTypeName = seatTypeName;
            PaidSeat = paidSeat;
            EntitledToVote = entitledToVote;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class BodySeatListQuery : Query<BodySeatListItem, BodySeatListItemFilter, BodySeatListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _bodyId;

        protected override ISorting Sorting => new BodySeatListSorting();

        protected override Expression<Func<BodySeatListItem, BodySeatListQueryResult>> Transformation =>
            x => new BodySeatListQueryResult(
                x.BodySeatId,
                x.BodySeatNumber,
                x.Name,
                x.SeatTypeId,
                x.SeatTypeName,
                x.PaidSeat,
                x.EntitledToVote,
                x.ValidFrom,
                x.ValidTo);

        public BodySeatListQuery(OrganisationRegistryContext context, Guid bodyId)
        {
            _context = context;
            _bodyId = bodyId;
        }

        protected override IQueryable<BodySeatListItem> Filter(FilteringHeader<BodySeatListItemFilter> filtering)
        {
            var bodySeats = _context.BodySeatList
                .AsQueryable()
                .Where(x => x.BodyId == _bodyId).AsQueryable();

            if (!filtering.ShouldFilter)
                return bodySeats;

            if (filtering.Filter.ActiveOnly)
                bodySeats = bodySeats.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return bodySeats;
        }

        private class BodySeatListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodySeatListItem.BodySeatNumber),
                nameof(BodySeatListItem.Name),
                nameof(BodySeatListItem.SeatTypeName),
                nameof(BodySeatListItem.PaidSeat),
                nameof(BodySeatListItem.EntitledToVote),
                nameof(BodySeatListItem.ValidFrom),
                nameof(BodySeatListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodySeatListItem.Name), SortOrder.Ascending);
        }
    }

    public class BodySeatListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
