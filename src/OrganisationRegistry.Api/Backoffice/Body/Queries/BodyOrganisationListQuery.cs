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

    public class BodyOrganisationListQueryResult
    {
        public Guid BodyOrganisationId { get; }
        public Guid OrganisationId { get; }
        public string OrganisationName { get; }
        public DateTime? ValidFrom { get; }
        public DateTime? ValidTo { get; }

        public bool IsActive { get; }

        public BodyOrganisationListQueryResult(
            Guid bodyOrganisationId,
            Guid organisationId, string organisationName,
            DateTime? validFrom, DateTime? validTo)
        {
            BodyOrganisationId = bodyOrganisationId;
            OrganisationId = organisationId;
            OrganisationName = organisationName;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class BodyOrganisationListQuery : Query<BodyOrganisationListItem, BodyOrganisationListItemFilter, BodyOrganisationListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _bodyId;

        protected override ISorting Sorting => new BodyOrganisationListSorting();

        protected override Expression<Func<BodyOrganisationListItem, BodyOrganisationListQueryResult>> Transformation =>
            x => new BodyOrganisationListQueryResult(
                x.BodyOrganisationId,
                x.OrganisationId,
                x.OrganisationName,
                x.ValidFrom,
                x.ValidTo);

        public BodyOrganisationListQuery(OrganisationRegistryContext context, Guid bodyId)
        {
            _context = context;
            _bodyId = bodyId;
        }

        protected override IQueryable<BodyOrganisationListItem> Filter(FilteringHeader<BodyOrganisationListItemFilter> filtering)
        {
            var bodyOrganisations = _context.BodyOrganisationList
                .AsQueryable()
                .Where(x => x.BodyId == _bodyId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return bodyOrganisations;

            if (filter.ActiveOnly)
                bodyOrganisations = bodyOrganisations.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return bodyOrganisations;
        }

        private class BodyOrganisationListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyOrganisationListItem.OrganisationName),
                nameof(BodyOrganisationListItem.ValidFrom),
                nameof(BodyOrganisationListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyOrganisationListItem.OrganisationName), SortOrder.Ascending);
        }
    }

    public class BodyOrganisationListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
