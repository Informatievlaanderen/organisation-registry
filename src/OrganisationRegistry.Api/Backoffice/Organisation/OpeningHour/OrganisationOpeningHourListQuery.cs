namespace OrganisationRegistry.Api.Backoffice.Organisation.OpeningHour
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;

    public class OrganisationOpeningHourListQueryResult
    {
        public Guid OrganisationOpeningHourId { get; set; }

        public string Opens { get; }

        public string Closes { get; }

        public string? DayOfWeek { get; }

        public DateTime? ValidFrom { get; }

        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public OrganisationOpeningHourListQueryResult(
            Guid organisationOpeningHourId,
            TimeSpan opens,
            TimeSpan closes,
            DayOfWeek? dayOfWeek,
            DateTime? validFrom,
            DateTime? validTo)
        {

            OrganisationOpeningHourId = organisationOpeningHourId;
            Opens = $"{opens.Hours:00}:{opens.Minutes:00}";
            Closes = $"{closes.Hours:00}:{closes.Minutes:00}";
            DayOfWeek = dayOfWeek.HasValue
                ? Thread.CurrentThread.CurrentCulture.DateTimeFormat.GetDayName(dayOfWeek.Value)
                : null;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationOpeningHourListQuery : Query<OrganisationOpeningHourListItem, OrganisationOpeningHourListItemFilter, OrganisationOpeningHourListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        public OrganisationOpeningHourListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationOpeningHourListItem> Filter(FilteringHeader<OrganisationOpeningHourListItemFilter> filtering)
        {
            var organisationOpeningHours = _context.OrganisationOpeningHourList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return organisationOpeningHours;

            if (filter.ActiveOnly)
                organisationOpeningHours = organisationOpeningHours.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationOpeningHours;
        }

        protected override Expression<Func<OrganisationOpeningHourListItem, OrganisationOpeningHourListQueryResult>> Transformation =>
            x => new OrganisationOpeningHourListQueryResult(
                x.OrganisationOpeningHourId,
                x.Opens,
                x.Closes,
                x.DayOfWeek,
                x.ValidFrom,
                x.ValidTo);

        protected override ISorting Sorting => new OrganisationOpeningHourListSorting();

        private class OrganisationOpeningHourListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationOpeningHourListItem.Opens),
                nameof(OrganisationOpeningHourListItem.Closes),
                nameof(OrganisationOpeningHourListItem.DayOfWeek),
                nameof(OrganisationOpeningHourListItem.ValidFrom),
                nameof(OrganisationOpeningHourListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationOpeningHourListItem.Opens), SortOrder.Ascending);
        }
    }

    public class OrganisationOpeningHourListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
