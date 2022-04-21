namespace OrganisationRegistry.Api.Backoffice.Parameters.ContactType.Queries
{
    using System.Collections.Generic;
    using System.Linq;
    using Be.Vlaanderen.Basisregisters.Api.Search.Helpers;
    using Infrastructure.Search;
    using Infrastructure.Search.Filtering;
    using Infrastructure.Search.Sorting;
    using SqlServer.ContactType;
    using SqlServer.Infrastructure;

    public class ContactTypeListQuery: Query<ContactTypeListItem>
    {
        private readonly OrganisationRegistryContext _context;

        protected override ISorting Sorting => new ContactTypeListSorting();

        public ContactTypeListQuery(OrganisationRegistryContext context)
        {
            _context = context;
        }

        protected override IQueryable<ContactTypeListItem> Filter(FilteringHeader<ContactTypeListItem> filtering)
        {
            var contactTypes = _context.ContactTypeList.AsQueryable();

            if (filtering.Filter is not { } filter)
                return contactTypes;

            if (!filter.Name.IsNullOrWhiteSpace())
                contactTypes = contactTypes.Where(x => x.Name.Contains(filtering.Filter.Name));

            return contactTypes;
        }

        private class ContactTypeListSorting : ISorting
        {
            public IEnumerable<string> SortableFields { get; } = new[]
            {
                nameof(ContactTypeListItem.Name)
            };

            public SortingHeader DefaultSortingHeader { get; } =
                new SortingHeader(nameof(ContactTypeListItem.Name), SortOrder.Ascending);
        }
    }
}
