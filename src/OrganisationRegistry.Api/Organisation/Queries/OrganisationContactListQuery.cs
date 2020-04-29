namespace OrganisationRegistry.Api.Organisation.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.Infrastructure;
    using SqlServer.Organisation;
    using System;
    using System.Linq.Expressions;

    public class OrganisationContactListQueryResult
    {
        public Guid OrganisationContactId { get; set; }
        public string ContactTypeName { get; set; }
        public string ContactValue { get; set; }
        public DateTime? ValidFrom { get; set; }
        public DateTime? ValidTo { get; set; }

        public bool IsActive { get; }

        public OrganisationContactListQueryResult(
            Guid organisationContactId,
            string contactTypeName,
            string contactValue,
            DateTime? validFrom,
            DateTime? validTo)
        {
            OrganisationContactId = organisationContactId;
            ContactTypeName = contactTypeName;
            ContactValue = contactValue;
            ValidFrom = validFrom;
            ValidTo = validTo;

            IsActive = new Period(new ValidFrom(validFrom), new ValidTo(validTo)).OverlapsWith(DateTime.Today);
        }
    }

    public class OrganisationContactListQuery : Query<OrganisationContactListItem, OrganisationContactListItemFilter, OrganisationContactListQueryResult>
    {
        private readonly OrganisationRegistryContext _context;
        private readonly Guid _organisationId;

        protected override ISorting Sorting => new OrganisationContactListSorting();

        protected override Expression<Func<OrganisationContactListItem, OrganisationContactListQueryResult>> Transformation =>
            x => new OrganisationContactListQueryResult(
                x.OrganisationContactId,
                x.ContactTypeName,
                x.ContactValue,
                x.ValidFrom,
                x.ValidTo);

        public OrganisationContactListQuery(OrganisationRegistryContext context, Guid organisationId)
        {
            _context = context;
            _organisationId = organisationId;
        }

        protected override IQueryable<OrganisationContactListItem> Filter(FilteringHeader<OrganisationContactListItemFilter> filtering)
        {
            var organisationContacts = _context.OrganisationContactList
                .AsQueryable()
                .Where(x => x.OrganisationId == _organisationId).AsQueryable();

            if (!filtering.ShouldFilter)
                return organisationContacts;

            if (filtering.Filter.ActiveOnly)
                organisationContacts = organisationContacts.Where(x =>
                    (!x.ValidFrom.HasValue || x.ValidFrom <= DateTime.Today) &&
                    (!x.ValidTo.HasValue || x.ValidTo >= DateTime.Today));

            return organisationContacts;
        }

        private class OrganisationContactListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(OrganisationContactListItem.ContactTypeName),
                nameof(OrganisationContactListItem.ContactValue),
                nameof(OrganisationContactListItem.ValidFrom),
                nameof(OrganisationContactListItem.ValidTo)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(OrganisationContactListItem.ContactTypeName), SortOrder.Ascending);
        }
    }

    public class OrganisationContactListItemFilter
    {
        public bool ActiveOnly { get; set; }
    }
}
