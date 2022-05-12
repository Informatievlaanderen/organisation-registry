namespace OrganisationRegistry.Api.Backoffice.Body.Contact
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using OrganisationRegistry.Api.Infrastructure.Search;
    using OrganisationRegistry.Api.Infrastructure.Search.Filtering;
    using OrganisationRegistry.Api.Infrastructure.Search.Sorting;
    using OrganisationRegistry.SqlServer.Body;
    using OrganisationRegistry.SqlServer.Infrastructure;

    public class BodyContactListQuery : Query<BodyContactListItem, BodyContactListItemFilter, BodyContactListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new BodyContactListSorting();

        protected override Expression<Func<BodyContactListItem, BodyContactListQueryResult>> Transformation =>
            x => new BodyContactListQueryResult(
                x.BodyContactId,
                x.ContactTypeName,
                x.ContactValue,
                x.ValidFrom,
                x.ValidTo);

        public BodyContactListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<BodyContactListItem> Filter(FilteringHeader<BodyContactListItemFilter> filtering)
        {
            var organisationContacts = _context.BodyContactList
                .AsQueryable()
                .Where(x => x.BodyId == _organisationId).AsQueryable();

            if (filtering.Filter is not { } filter)
                return organisationContacts;

            if (filter.ActiveOnly)
                organisationContacts = organisationContacts.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationContacts;
        }

        private class BodyContactListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(BodyContactListItem.ContactTypeName),
                nameof(BodyContactListItem.ContactValue),
                nameof(BodyContactListItem.ValidFrom),
                nameof(BodyContactListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(BodyContactListItem.ContactTypeName), SortOrder.Ascending);
        }
    }

    public class BodyContactListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
